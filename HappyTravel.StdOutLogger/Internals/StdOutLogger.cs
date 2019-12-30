using System;
using System.Collections.Generic;
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


        public StdOutLoggerOptions Options
        {
            get => _options;
            set
            {
                _options = value;
                _options.JsonSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            }
        }


        internal IExternalScopeProvider ScopeProvider { get; set; }


        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            var message = string.Empty;

            if (formatter != null)
                message = formatter(state, exception);

            var parameters = GetParameters(state);

            var requestId = string.Empty;

            if (_httpContextAccessor.HttpContext?.Request != null &&
                _httpContextAccessor.HttpContext.Request.Headers.TryGetValue(_options.RequestIdHeader, out var requestIdString))
                requestId = requestIdString.FirstOrDefault();

            var jsonLogEntry = JObject.FromObject(new LogEntry(
                requestId,
                _name,
                logLevel,
                eventId,
                Options.UseUtcTimestamp ? DateTime.UtcNow : DateTime.Now,
                string.IsNullOrWhiteSpace(message) ? null : message,
                parameters,
                exception,
                GetScopeData()
            ), JsonSerializer.Create(Options.JsonSerializerSettings)).ToString(Formatting.None);

            _loggerProcessor.Log(jsonLogEntry);
        }


        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;


        public IDisposable BeginScope<TState>(TState state) => ScopeProvider?.Push(state) ?? NullScope.Instance;


        private object GetParameters<TState>(TState state)
        {
            if (state is IEnumerable<KeyValuePair<string, object>> parameters)
            {
                var result = parameters.Where(p => !Options.SkippedJsonParameters.Contains(p.Key))
                    .ToDictionary(i => i.Key, i => i.Value);
                
                if (!result.Any())
                    return null;
            }

            return state;
        }

        
        private object GetScopeData()
        {
            var scopeStorage = new List<object>();
            var scopedProvider = ScopeProvider;
            if (Options.IncludeScopes)
                scopedProvider?.ForEachScope((scope, storage) =>
                {
                    storage.Add(scope); 
                }, scopeStorage);
            
            return scopeStorage;
        }


        private void GetScopeMessage(StringBuilder stringBuilder)
        {
            var scopedProvider = ScopeProvider;
            if (Options.IncludeScopes)
                scopedProvider?.ForEachScope((scope, builder) =>
                {
                    builder.Append("=> ");
                    builder.Append(scope);
                }, stringBuilder);
        }


        private readonly string _name;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LoggerProcessor _loggerProcessor;
        private StdOutLoggerOptions _options;
    }
}