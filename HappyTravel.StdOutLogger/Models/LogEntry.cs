using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using HappyTravel.StdOutLogger.Extensions;
using Microsoft.Extensions.Logging;

namespace HappyTravel.StdOutLogger.Models
{
    internal readonly struct LogEntry
    {
        [JsonConstructor]
        public LogEntry(DateTime createdAt, EventId eventId, string logName, LogLevel logLevel, string requestId, string traceId, string parentSpanId,
            string spanId, string message, string messageTemplate)
        {
            CreatedAt = createdAt;
            EventId = eventId;
            LogName = logName;
            LogLevel = logLevel;
            RequestId = requestId;
            TraceId = traceId;
            ParentSpanId = parentSpanId;
            SpanId = spanId;
            Message = message;
            MessageTemplate = messageTemplate;

            Data = new Dictionary<string, object>();
            Scope = new List<KeyValuePair<string, object>>();
            Renderings = new Dictionary<string, object>();
        }


        public string GetJson()
        {
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);

            writer.WriteStartObject();
            writer.WriteString("request_id", RequestId);
            writer.WriteString("created_at", CreatedAt);
            writer.WriteNumber("event_id", EventId.Id);
            writer.WriteString("event_name", EventId.Name);
            writer.WriteString("log_name", LogName);
            writer.WriteString("log_level", GetLogName(LogLevel));
            writer.WriteString("message", Message);
            writer.WriteString("messageTemplate", MessageTemplate);
            writer.WriteCollection("renderings", Renderings);
            writer.WriteCollection("data", Data);
            writer.WriteCollection("scope", Scope);
            writer.WriteEndObject();
            writer.Flush();
            
            return Encoding.UTF8.GetString(stream.ToArray());
        }


        private static string GetLogName(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Trace => "Trace",
                LogLevel.Debug => "Debug",
                LogLevel.Information => "Information",
                LogLevel.Warning => "Warning",
                LogLevel.Error => "Error",
                LogLevel.Critical => "Critical",
                LogLevel.None => "None",
                _ => throw new InvalidEnumArgumentException($"{nameof(logLevel)}")
            };
        }


        [JsonPropertyName("timestamp")]
        public DateTime CreatedAt { get; }

        [JsonPropertyName("event_id")]
        public EventId EventId { get; }

        [JsonPropertyName("log_name")]
        public string LogName { get; }

        [JsonPropertyName("log_level")]
        public LogLevel LogLevel { get; }

        [JsonPropertyName("request_id")]
        public string RequestId { get; }

        [JsonPropertyName("trace_id")]
        public string TraceId { get; }

        [JsonPropertyName("parent_span_id")]
        public string ParentSpanId { get; }

        [JsonPropertyName("span_id")]
        public string SpanId { get; }

        [JsonPropertyName("message")]
        public string Message { get; }
        
        [JsonPropertyName("messageTemplate")]
        public string MessageTemplate { get; }

        [JsonPropertyName("data")]
        public Dictionary<string, object> Data { get; }
        
        [JsonPropertyName("scope")]
        public List<KeyValuePair<string, object>> Scope { get; }
        
        [JsonPropertyName("renderings")]
        public Dictionary<string, object> Renderings { get; }
    }
}