using HappyTravel.StdOutLogger.Models;
using Microsoft.Extensions.Logging;

namespace HappyTravel.StdOutLogger.Extensions
{
    public static class StdOutLoggerMessageExtensions
    {
        public static void LogInformationToJson(this ILogger logger, EventId eventId, string message,
            HttpContextLogEntry httpContextContextLogEntry = default)
        {
            logger.Log(LogLevel.Information,
                eventId,
                new {httpContextContextLogEntry.TraceId, HttpContext = httpContextContextLogEntry, Message = message},
                null,
                (state, ex) => string.Empty);
        }
    }
}