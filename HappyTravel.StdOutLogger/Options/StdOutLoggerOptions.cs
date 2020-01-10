using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace HappyTravel.StdOutLogger.Options
{
    public class StdOutLoggerOptions
    {
        public bool UseUtcTimestamp { get; set; } = true;

        public bool IncludeScopes { get; set; }

        public string RequestIdHeader {get; set;} = "x-request-id";
    }
}