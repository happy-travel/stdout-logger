﻿using System.Collections.Concurrent;
using HappyTravel.StdOutLogger.Internals;
using HappyTravel.StdOutLogger.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HappyTravel.StdOutLogger
{
    public class StdOutLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        public StdOutLoggerProvider(StdOutLoggerOptions options, IHttpContextAccessor httpContextAccessor)
        {
            _options = options;
            _loggers = new ConcurrentDictionary<string, Internals.StdOutLogger>();
            _loggerProcessor = new LoggerProcessor();
            _httpContextAccessor = httpContextAccessor;
        }


        public ILogger CreateLogger(string name)
            => _loggers.GetOrAdd(name, new Internals.StdOutLogger(name, _loggerProcessor, _httpContextAccessor)
            {
                Options = _options,
                ScopeProvider = _scopeProvider,
            });


        public void Dispose()
        {
            _loggerProcessor.Dispose();
        }


        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;

            foreach (var logger in _loggers)
                logger.Value.ScopeProvider = _scopeProvider;
        }


        private readonly LoggerProcessor _loggerProcessor;
        private readonly StdOutLoggerOptions _options;
        private readonly ConcurrentDictionary<string, Internals.StdOutLogger> _loggers;
        private IExternalScopeProvider _scopeProvider = NullExternalScopeProvider.Instance;
        private readonly IHttpContextAccessor _httpContextAccessor;
    }
}