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
            get => _ignoredPaths.Select(i => i.ToUpperInvariant()).ToHashSet();
            set => _ignoredPaths = value;
        }
    }
}