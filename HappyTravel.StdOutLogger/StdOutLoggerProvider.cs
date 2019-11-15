using System.Collections.Concurrent;
using HappyTravel.StdOutLogger.Internals;
using HappyTravel.StdOutLogger.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HappyTravel.StdOutLogger
{
    public class StdOutLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        public StdOutLoggerProvider(IOptions<StdOutLoggerOptions> options)
        {
            _options = options.Value;
            _loggers = new ConcurrentDictionary<string, Internals.StdOutLogger>();
            _loggerProcessor = new LoggerProcessor();
        }


        public ILogger CreateLogger(string name)
        {
            return _loggers.GetOrAdd(name, new Internals.StdOutLogger(name, _loggerProcessor)
            {
                Options = _options,
                ScopeProvider = _scopeProvider
            });
        }


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
    }
}