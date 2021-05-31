using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using HappyTravel.StdOutLogger.Models;
using HappyTravel.StdOutLogger.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HappyTravel.StdOutLogger.Internals
{
    internal class StdOutLogger : ILogger
    {
        public StdOutLogger(string name, LoggerProcessor loggerProcessor, IHttpContextAccessor httpContextAccessor, StdOutLoggerOptions options)
        {
            _name = name;
            _loggerProcessor = loggerProcessor;
            _httpContextAccessor = httpContextAccessor;
            _options = options;
        }

        
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            if (formatter is null)
                throw new ArgumentNullException(nameof(formatter));
            
            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message))
                return;
            
            var messageBuilder = new StringBuilder();

            messageBuilder.Append(message);

            if (exception != null)
                messageBuilder.AppendLine("Exception: ").Append(exception);
            
            var requestId = string.Empty;

            try
            {
                if (_httpContextAccessor.HttpContext?.Request != null &&
                    _httpContextAccessor.HttpContext.Request.Headers.TryGetValue(_options.RequestIdHeader, out var requestIdString))
                    requestId = requestIdString.FirstOrDefault();
            }
            catch (ObjectDisposedException)
            {
                // resume normal operation of writing log without request data 
            }

            var spanId = string.Empty;
            var parentId = string.Empty; 
            var traceId = string.Empty;

            if (Activity.Current != null)
            {
                spanId = Activity.Current.SpanId.ToString(); 
                parentId = Activity.Current.ParentId;
                traceId = Activity.Current.RootId;
            }
            
            var createdAt = _options.UseUtcTimestamp ? DateTime.UtcNow : DateTime.Now;
            var messageTemplate = GetMessageTemplate(state);

            var logEntry = new LogEntry(createdAt, eventId, _name, logLevel, requestId, traceId, parentId, spanId, messageBuilder.ToString(), messageTemplate);
            if (exception?.Data != null)
            {
                foreach (DictionaryEntry entry in exception.Data!)
                {
                    if (entry.Value is null)
                        continue;

                    logEntry.Data.Add(entry.Key.ToString()!, entry.Value ?? "null");
                }
            }
            
            AddScopedInformation(logEntry, messageBuilder);
            AddMessageVariables(logEntry, state);
            
            _loggerProcessor.Log(logEntry.GetJson());
        }

        
        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;


        public IDisposable BeginScope<TState>(TState state) => ScopeProvider?.Push(state) ?? NullScope.Instance;


        private void AddScopedInformation(LogEntry logEntry, StringBuilder stringBuilder)
        {
            if (!_options.IncludeScopes)
                return;

            var scopedProvider = ScopeProvider;
            scopedProvider?.ForEachScope((scope, sb) =>
                {
                    if (scope is IEnumerable<KeyValuePair<string, object>> properties)
                    {
                        foreach (var (key, value) in properties)
                            logEntry.Scope.Add(key, value?.ToString() ?? string.Empty);
                    }
                    else if (scope != null)
                    {
                        sb.AppendLine(scope.ToString());
                    }
                },
                stringBuilder);
        }


        private static string GetMessageTemplate<TState>(TState state)
        {
            if (state is not IReadOnlyList<KeyValuePair<string, object>> values) 
                return string.Empty;

            return values.Where(v => v.Key == MessageTemplateKey)
                .Select(v => v.Value)
                .SingleOrDefault()?.ToString() ?? string.Empty;
        }


        private static void AddMessageVariables<TState>(LogEntry logEntry, TState state)
        {
            if (state is not IReadOnlyList<KeyValuePair<string, object>> values) 
                return;
            
            foreach (var (key, value) in values.Where(v => v.Key != MessageTemplateKey))
                logEntry.Renderings.Add(key, value);
        }


        private const string MessageTemplateKey = "{OriginalFormat}";


        internal IExternalScopeProvider? ScopeProvider { get; set; }

        private readonly StdOutLoggerOptions _options;
        private readonly string _name;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LoggerProcessor _loggerProcessor;
    }
}