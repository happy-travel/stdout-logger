using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HappyTravel.StdOutLogger.Models
{
    internal readonly struct LogEntry
    {
        [JsonConstructor]
        public LogEntry(
            DateTime timestamp,
            EventId eventId,
            string logName,
            LogLevel logLevel,
            string requestId,
            string message)
        {
            Timestamp = timestamp;
            EventId = eventId;
            LogName = logName;
            LogLevel = logLevel;
            RequestId = requestId;
            Message = message;
        }


        public string GetJson()
        {
            var stringBuilder = new StringBuilder();
            var stringWriter = new StringWriter(stringBuilder);

            using (var jsonWriter = new JsonTextWriter(stringWriter))
            {
                jsonWriter.Formatting = Formatting.None;
                jsonWriter.Culture = CultureInfo.InvariantCulture;
                jsonWriter.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                
                jsonWriter.WriteStartObject();
                
                jsonWriter.WritePropertyName("request_id");
                jsonWriter.WriteValue(RequestId);
                
                jsonWriter.WritePropertyName("timestamp");
                jsonWriter.WriteValue(Timestamp);
                
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
                
                jsonWriter.WriteEndObject();
            }
            return stringBuilder.ToString();
        }


        private string GetLogName(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace: return "Trace";
                case LogLevel.Debug: return "Debug";
                case LogLevel.Information: return "Information";
                case LogLevel.Warning: return "Warning";
                case LogLevel.Error: return "Error";
                case LogLevel.Critical: return "Critical";
                case LogLevel.None: return "None";
                default:
                    throw new InvalidEnumArgumentException($"{nameof(logLevel)}");
            }
        }
        
        
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; }
        
        [JsonProperty("event_id")]
        public EventId EventId { get; }
        
        [JsonProperty("log_name")]
        public string LogName { get; }

        [JsonProperty("log_level")]
        public LogLevel LogLevel { get; }

        [JsonProperty("request_id")]
        public string RequestId { get; }

        [JsonProperty("message")]
        public string Message { get; }
    }
}