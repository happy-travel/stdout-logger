using static HappyTravel.StdOutLogger.Infrastructure.Constants;

namespace HappyTravel.StdOutLogger.Options
{
    public class StdOutLoggerOptions
    {
        public bool UseUtcTimestamp { get; set; } = true;

        public bool IncludeScopes { get; set; }

        public string RequestIdHeader {get; set;} = DefaultRequestIdHeader;
    }
}