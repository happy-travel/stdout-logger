using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HappyTravel.StdOutLogger.Models;
using HappyTravel.StdOutLogger.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HappyTravel.StdOutLogger.Internals
{
    internal class StdOutLogger : ILogger
    {
        public StdOutLogger(string name, LoggerProcessor loggerProcessor, IHttpContextAccessor httpContextAccessor)
        {
            _name = name;
            _loggerProcessor = loggerProcessor;
            _httpContextAccessor = httpContextAccessor;
        }

        
        public void Log<TState>(
            LogLevel logLevel, 
            EventId eventId, 
            TState state, 
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));
            
            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message))
                return;
            
            var messageBuilder = new StringBuilder();
            
            GetScopedInformation(messageBuilder);
            
            messageBuilder.Append(message);

            if (exception != null)
                messageBuilder.AppendLine("Exception: ").Append(exception);
            
            var requestId = string.Empty;

            if (_httpContextAccessor.HttpContext?.Request != null &&
                _httpContextAccessor.HttpContext.Request.Headers.TryGetValue(Options.RequestIdHeader, out var requestIdString))
                requestId = requestIdString.FirstOrDefault();
           
            var logEntry = new LogEntry(
                Options.UseUtcTimestamp ? DateTime.UtcNow : DateTime.Now,
                eventId,
                _name,
                logLevel,
                requestId,
                messageBuilder.ToString());
            
            _loggerProcessor.Log(logEntry.GetJson());
        }

        
        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;


        public IDisposable BeginScope<TState>(TState state) => ScopeProvider?.Push(state) ?? NullScope.Instance;
        
    
        private void GetScopedInformation(StringBuilder stringBuilder)
        {
            if (!Options.IncludeScopes)
                return;

            var scopedProvider = ScopeProvider;
            scopedProvider?.ForEachScope((scope, sb) =>
                {
                    if (scope is IEnumerable<KeyValuePair<string, object>> properties)
                    {
                        foreach (var pair in properties)
                        {
                            sb.Append(pair.Key).Append(": ").AppendLine(pair.Value?.ToString());
                        }
                    }
                    else if (scope != null)
                    {
                        sb.AppendLine(scope.ToString());
                    }
                },
                stringBuilder);
            stringBuilder.AppendLine();
        }

        
        public StdOutLoggerOptions Options { get; set; }
        internal IExternalScopeProvider ScopeProvider { get; set; }
        
        
        private readonly string _name;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LoggerProcessor _loggerProcessor;
    }
}