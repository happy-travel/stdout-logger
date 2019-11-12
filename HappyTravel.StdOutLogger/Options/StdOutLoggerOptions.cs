using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace HappyTravel.StdOutLogger.Options
{
    public class StdOutLoggerOptions
    {
        public JsonSerializerSettings JsonSerializerSettings { get; set; } = new JsonSerializerSettings
        {
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
            Converters = new List<JsonConverter>
                {new StringEnumConverter {NamingStrategy = new CamelCaseNamingStrategy()}},
            MaxDepth = 3
        };


        public HashSet<string> SkippedJsonParameters { get; set; } =
            new HashSet<string> {"MethodInfo", "{OriginalFormat}"};


        public bool UseUtcTimestamp { get; set; } = true;
        public bool IncludeScopes { get; set; } = false;
    }
}