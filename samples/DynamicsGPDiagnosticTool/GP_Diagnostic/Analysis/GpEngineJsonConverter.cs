namespace Microsoft.GP.MigrationDiagnostic.Analysis;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class GpEngineJsonConverter : JsonConverter<GpEngine>
{
    public override GpEngine? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

    public override void Write(Utf8JsonWriter writer, GpEngine value, JsonSerializerOptions options)
    {
        if (value != null)
        {
            writer.WriteStartObject();

            writer.WriteString(GetPropertyName(nameof(value.Name), options), value.Name);

            writer.WriteStartArray(GetPropertyName(nameof(value.TaskGroups), options));

            foreach (var item in value.TaskGroups)
            {
                writer.WriteRawValue(item switch
                {
                    IMultiCompanyDiagnosticTaskGroup companyTaskGroup => JsonSerializer.Serialize<object>(companyTaskGroup, options),
                    IDiagnosticTaskGroup taskGroup => JsonSerializer.Serialize(taskGroup, options),
                    _ => throw new JsonException($"Unsupported IDiagnosticTaskGroup subtype."),
                });
            }

            writer.WriteEndArray();

            writer.WriteEndObject();
        }
        else
        {
            writer.WriteNullValue();
        }
    }

    private static string GetPropertyName(string propertyName, JsonSerializerOptions options)
    {
        return options.PropertyNamingPolicy?.ConvertName(propertyName) ?? propertyName;
    }
}
