using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace CustomTelemetryHandler
{
    public static class CustomTelemetryHandlerFunction
    {
        private static readonly string[] AppInsightsConnectionStrings = System.Environment.GetEnvironmentVariable("BCApplicationInsightsConnectionStrings", EnvironmentVariableTarget.Process)?.Split("|");
        private static readonly string[] ExcludeEventTypes = System.Environment.GetEnvironmentVariable("ExcludeEventTypes", EnvironmentVariableTarget.Process)?.Split(";");
        private static readonly string[] ExcludeEventIds = System.Environment.GetEnvironmentVariable("ExcludeEventIdList", EnvironmentVariableTarget.Process)?.Split(";");
        private static readonly string EnableLogsSetting = System.Environment.GetEnvironmentVariable("EnableLogs", EnvironmentVariableTarget.Process);
        private static bool EnableLogs = false;

        [FunctionName("CustomTelemetryHandlerFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "{*restOfPath}")] 
            HttpRequest req,
            string restOfPath, 
            ILogger log)
        {
            if (EnableLogs)
                log.LogInformation($"Processing request to {restOfPath}");

            if ((AppInsightsConnectionStrings == null) || (AppInsightsConnectionStrings.Length == 0))
            {
                log.LogError("BC Application Insights connection string is missing. Please specify setting BCApplicationInsightsConnectionString");
                return new NotFoundResult();
            }
            if (!string.IsNullOrEmpty(EnableLogsSetting))
            {
                bool.TryParse(EnableLogsSetting, out EnableLogs);
            }

            TelemetryConfiguration telemetryConfiguration;

            var requestBody = await req.GetBodyAsStringAsync();
            if (EnableLogs)
                log.LogInformation(requestBody);

            IList<JObject> telemetryItems = ReadItems(requestBody, out telemetryConfiguration, log);
            if (telemetryItems.Count != 0)
            {
                SendToApplicationInsights(telemetryItems, telemetryConfiguration, restOfPath);
            }

            if (EnableLogs)
                log.LogInformation($"Forwarding {telemetryItems.Count} events.");

            if (EnableLogs)
                log.LogInformation($"Finished request.");

            return new OkResult();
        }

        private static IList<JObject> ReadItems(string json, out TelemetryConfiguration telemetryConfiguration, ILogger log)
        {
            var items = new List<JObject>();
            telemetryConfiguration = null;

            var reader = new JsonTextReader(new StringReader(json))
            {
                SupportMultipleContent = true
            };

            while (true)
            {
                JToken item = JToken.ReadFrom(reader);

                switch (item.Type)
                {
                    case JTokenType.Object:
                        if (IncludeTelemetryItem((JObject)item, ref telemetryConfiguration, log))
                            items.Add((JObject)item);
                        break;
                    case JTokenType.Array:
                        foreach (JObject obj in item)
                        {
                            if (IncludeTelemetryItem(obj, ref telemetryConfiguration, log))
                                items.Add(obj);
                        }
                        break;
                }

                if (!reader.Read())
                    break;
            }

            return items;
        }

        private static bool IncludeTelemetryItem(JObject item, ref TelemetryConfiguration telemetryConfiguration, ILogger log)
        {
            if (item == null) return false;

            var name = item.Property("name")?.Value.ToString();
            if (string.IsNullOrEmpty(name) || (ExcludeEventTypes != null && ExcludeEventTypes.Length > 0 && ExcludeEventTypes.Contains(name)))
            {
                if (EnableLogs && !string.IsNullOrEmpty(name))
                    log.LogInformation($"Excluding event type {name}");
                return false;
            }

            if (name == "AppTraces")
            {
                var eventId = item.SelectToken("$.data.baseData.properties.eventId")?.ToString();
                if (string.IsNullOrEmpty(eventId) || (ExcludeEventIds != null && ExcludeEventIds.Length > 0 && ExcludeEventIds.Any(id => Regex.IsMatch(eventId, WildCardToRegular(id), RegexOptions.IgnoreCase))))
                {
                    if (EnableLogs && !string.IsNullOrEmpty(eventId))
                        log.LogInformation($"Excluding event id {eventId}");
                    return false;
                }
                else
                {
                    if (EnableLogs)
                        log.LogInformation($"Including event id {eventId}");
                }
            }

            var iKey = item.Property("iKey")?.Value.ToString();
            if (string.IsNullOrEmpty(iKey))
            {
                if (EnableLogs)
                    log.LogWarning($"Event data does not contain an instrumentation key. Event will be ignored.");
                return false;
            }

            var connectionString = AppInsightsConnectionStrings.FirstOrDefault<string>(c => c.Contains(iKey));

            if (string.IsNullOrEmpty(connectionString))
            {
                if (EnableLogs)
                    log.LogWarning($"Unexpected Instrumentation Key {iKey}. Event will be ignored.");
                return false;
            }

            if (telemetryConfiguration == null)
            {
                telemetryConfiguration = new TelemetryConfiguration()
                {
                    ConnectionString = connectionString
                };
            }

            return true;
        }

        private static string WildCardToRegular(string value)
        {
            return "^" + Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*") + "$";
        }

        private static void SendToApplicationInsights(IEnumerable<JObject> items, TelemetryConfiguration telemetryConfiguration, string path)
        {
            if (items == null || !items.Any())
                return;

            byte[] data = Serialize(items);

            string formattedIngestionEndpoint = new Uri(telemetryConfiguration.EndpointContainer.Ingestion, path).AbsoluteUri;
            var endpoint = new Uri(formattedIngestionEndpoint);
            Transmission transmission = new Transmission(endpoint, data, Microsoft.ApplicationInsights.Extensibility.Implementation.JsonSerializer.ContentType, Microsoft.ApplicationInsights.Extensibility.Implementation.JsonSerializer.CompressionType);
            var t = transmission.SendAsync();
            t.Wait();
        }

        private static byte[] Serialize(IEnumerable<JObject> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var ms = new MemoryStream();
            using Stream compressedStream = CreateCompressedStream(ms);
            using StreamWriter streamWriter = new StreamWriter(compressedStream, new UTF8Encoding(false));
            SerializeToStream(items, streamWriter);
            return ms.ToArray();
        }

        private static Stream CreateCompressedStream(Stream stream)
        {
            return new GZipStream(stream, CompressionMode.Compress);
        }

        private static void SerializeToStream(IEnumerable<JObject> items, TextWriter textWriter)
        {
            using JsonWriter jsonWriter = new JsonTextWriter(textWriter);

            int count = 0;
            foreach (JObject item in items)
            {
                if (count++ > 0)
                    textWriter.Write(Environment.NewLine);

                item.WriteTo(jsonWriter);
            }
        }
    }
}
