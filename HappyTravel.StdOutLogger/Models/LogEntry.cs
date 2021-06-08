using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
            Scope = new Dictionary<string, object>();
            Renderings = new Dictionary<string, object>();
        }


        public string GetJson()
        {
            var stringBuilder = new StringBuilder();
            var stringWriter = new StringWriter(stringBuilder);

            using var jsonWriter = new JsonTextWriter(stringWriter)
            {
                Formatting = Formatting.None,
                Culture = CultureInfo.InvariantCulture,
                DateFormatHandling = DateFormatHandling.IsoDateFormat
            };

            jsonWriter.WriteStartObject();

            jsonWriter.WritePropertyName("request_id");
            jsonWriter.WriteValue(RequestId);

            jsonWriter.WritePropertyName("created_at");
            jsonWriter.WriteValue(CreatedAt);

            jsonWriter.WritePropertyName("event_id");
            jsonWriter.WriteValue(EventId.Id);

            jsonWriter.WritePropertyName("event_name");
            jsonWriter.WriteValue(EventId.Name ?? string.Empty);

            jsonWriter.WritePropertyName("log_name");
            jsonWriter.WriteValue(LogName);

            jsonWriter.WritePropertyName("log_level");
            jsonWriter.WriteValue(GetLogName(LogLevel));

            jsonWriter.WritePropertyName("message");
            jsonWriter.WriteValue(Message);
            
            jsonWriter.WritePropertyName("messageTemplate");
            jsonWriter.WriteValue(MessageTemplate);
            
            WriteDictionary(Renderings, "renderings", jsonWriter);
            WriteDictionary(Data, "data", jsonWriter);
            WriteDictionary(Scope, "scope", jsonWriter);

            jsonWriter.WriteEndObject();
            return stringBuilder.ToString();
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

        private static void WriteDictionary(Dictionary<string, object> dictionary, string propertyName, JsonWriter jsonWriter)
        {
            if (!dictionary.Any()) return;
            
            jsonWriter.WritePropertyName(propertyName);
            jsonWriter.WriteStartObject();

            foreach (var (key, value) in dictionary)
            {
                jsonWriter.WritePropertyName(key);
                
                switch (value)
                {
                    case string str:
                        jsonWriter.WriteValue(str);
                        break;
                    
                    case IDictionary<string, string> dict:
                        jsonWriter.WriteValue(JsonConvert.SerializeObject(dict));
                        break;
                    
                    default:
                        jsonWriter.WriteValue(value?.ToString());
                        break;
                }
            }

            jsonWriter.WriteEndObject();
        }


        [JsonProperty("timestamp")]
        public DateTime CreatedAt { get; }

        [JsonProperty("event_id")]
        public EventId EventId { get; }

        [JsonProperty("log_name")]
        public string LogName { get; }

        [JsonProperty("log_level")]
        public LogLevel LogLevel { get; }

        [JsonProperty("request_id")]
        public string RequestId { get; }

        [JsonProperty("trace_id")]
        public string TraceId { get; }

        [JsonProperty("parent_span_id")]
        public string ParentSpanId { get; }

        [JsonProperty("span_id")]
        public string SpanId { get; }

        [JsonProperty("message")]
        public string Message { get; }
        
        [JsonProperty("messageTemplate")]
        public string MessageTemplate { get; }

        [JsonProperty("data")]
        public Dictionary<string, object> Data { get; }
        
        [JsonProperty("scope")]
        public Dictionary<string, object> Scope { get; }
        
        [JsonProperty("renderings")]
        public Dictionary<string, object> Renderings { get; }
    }
}