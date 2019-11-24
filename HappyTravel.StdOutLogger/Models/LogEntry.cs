using System;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HappyTravel.StdOutLogger.Models
{
    internal readonly struct LogEntry
    {
        [JsonConstructor]
        public LogEntry(string requestId,
            string logName,
            LogLevel logLevel,
            EventId eventId,
            DateTime createdAt,
            string message,
            object parameters = null,
            Exception exception = null,
            object scopes = null)
        {
            RequestId = requestId;
            LogName = logName;
            LogLevel = logLevel;
            EventId = eventId;
            CreatedAt = createdAt;
            Message = message;
            Parameters = parameters;
            Exception = exception?.ToString();
            Scopes = scopes;
        }


        [JsonProperty("request_id")]
        public string RequestId { get; }

        [JsonProperty("log_name")]
        public string LogName { get; }

        [JsonProperty("log_level")]
        public LogLevel LogLevel { get; }

        [JsonProperty("event_id")]
        public EventId EventId { get; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; }

        [JsonProperty("parameters")]
        public object Parameters { get; }

        [JsonProperty("message")]
        public string Message { get; }

        [JsonProperty("exception")]
        public string Exception { get; }

        [JsonProperty("scopes")]
        public object Scopes { get; }
    }
}