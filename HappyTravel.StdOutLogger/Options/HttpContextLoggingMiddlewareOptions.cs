using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace HappyTravel.StdOutLogger.Options
{
    public class HttpContextLoggingMiddlewareOptions
    {
        private HashSet<string> _ignoredPaths = new HashSet<string> {"/health"};

        public HashSet<string> IgnoredPaths
        {
            get => _ignoredPaths.Select(i => i.ToLower(CultureInfo.InvariantCulture)).ToHashSet();
            set => _ignoredPaths = value;
        }

        public bool CollectRequestResponseLog { get; set; } = false;

        public string RequestIdHeader { get; set; } = "x-request-id";
    }
}