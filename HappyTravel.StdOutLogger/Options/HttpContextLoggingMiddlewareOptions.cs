using System.Collections.Generic;
using System.Linq;

namespace HappyTravel.StdOutLogger.Options
{
    public class HttpContextLoggingMiddlewareOptions
    {
        public HashSet<string> IgnoredPaths
        {
            get => _ignoredPaths.Select(i => i.ToUpperInvariant()).ToHashSet();
            set => _ignoredPaths = value;
        }

        public HashSet<string> IgnoredMethods
        {
            get => _ignoredMethods.Select(i => i.ToUpperInvariant()).ToHashSet();
            set => _ignoredMethods = value;
        }


        private HashSet<string> _ignoredPaths = new() {"/health"};
        private HashSet<string> _ignoredMethods = new();
    }
}