// Copyright (c) Microsoft Corporation. All rights reserved.

using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace BcAdminMcpProxy;

/// <summary>
/// Writes structured log entries to stderr, and optionally to a JSON log file
/// in %LOCALAPPDATA%\BcMcpProxy\logs\ when Debug mode is enabled.
/// </summary>
public sealed class TelemetryLogger : IDisposable
{
    private const string LogDirectoryName = "BcMcpProxy";
    private const string LogSubDirectory = "logs";

    private readonly bool fileLoggingEnabled;
    private readonly StreamWriter? writer;
    private readonly string? logFilePath;
    private bool disposed;

    public TelemetryLogger(bool enableFileLogging)
    {
        this.fileLoggingEnabled = enableFileLogging;

        if (this.fileLoggingEnabled)
        {
            var logDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                LogDirectoryName,
                LogSubDirectory);

            Directory.CreateDirectory(logDir);

            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            this.logFilePath = Path.Combine(logDir, $"bcmcpproxy_{timestamp}.log");

            this.writer = new StreamWriter(this.logFilePath, append: true)
            {
                AutoFlush = true,
            };

            this.Info("LogStarted", new { logFile = this.logFilePath });
        }
    }

    public string? LogFilePath => this.logFilePath;

    public void Info(string eventName, object? properties = null)
    {
        this.Write("Info", eventName, properties);
    }

    public void Warning(string eventName, object? properties = null)
    {
        this.Write("Warning", eventName, properties);
    }

    public void Error(string eventName, object? properties = null)
    {
        this.Write("Error", eventName, properties);
    }

    public void Error(string eventName, Exception ex, object? properties = null)
    {
        var merged = new JsonObject
        {
            ["exception"] = ex.GetType().Name,
            ["message"] = ex.Message,
        };

        if (properties is not null)
        {
            var propsJson = JsonSerializer.SerializeToNode(properties);
            if (propsJson is JsonObject propsObj)
            {
                foreach (var kvp in propsObj)
                {
                    merged[kvp.Key] = kvp.Value?.DeepClone();
                }
            }
        }

        this.Write("Error", eventName, merged);
    }

    public void ToolCall(string toolName, string? tenantId, long durationMs, bool isError)
    {
        this.Info("ToolCall", new
        {
            tool = toolName,
            tenantId = tenantId ?? string.Empty,
            durationMs,
            isError,
        });
    }

    public void RemoteCall(string method, int? httpStatus, long durationMs)
    {
        this.Info("RemoteCall", new
        {
            method,
            httpStatus,
            durationMs,
        });
    }

    public void TokenAcquired(string tenantId, string acquireMethod, long durationMs)
    {
        this.Info("TokenAcquired", new
        {
            tenantId,
            method = acquireMethod,
            durationMs,
        });
    }

    private void Write(string level, string eventName, object? properties)
    {
        var entry = new JsonObject
        {
            ["timestamp"] = DateTime.UtcNow.ToString("o"),
            ["level"] = level,
            ["event"] = eventName,
        };

        if (properties is not null)
        {
            if (properties is JsonNode node)
            {
                entry["properties"] = node.DeepClone();
            }
            else
            {
                entry["properties"] = JsonSerializer.SerializeToNode(properties);
            }
        }

        var line = entry.ToJsonString();

        // Write to log file (only if enabled)
        if (this.fileLoggingEnabled && this.writer is not null)
        {
            try
            {
                this.writer.WriteLine(line);
            }
            catch
            {
                // Don't crash the proxy if logging fails
            }
        }

        // Echo to stderr
        Console.Error.WriteLine($"[BcMcpProxy] [{level}] {eventName}{(properties is not null ? $" {JsonSerializer.Serialize(properties)}" : string.Empty)}");
    }

    public void Dispose()
    {
        if (!this.disposed)
        {
            this.disposed = true;
            if (this.fileLoggingEnabled)
            {
                this.Info("LogStopped", null);
            }

            this.writer?.Dispose();
        }
    }
}
