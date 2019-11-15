using System;

namespace HappyTravel.StdOutLogger.Internals
{
    internal class NullScope : IDisposable
    {
        private NullScope()
        {
        }


        public static NullScope Instance { get; } = new NullScope();


        public void Dispose()
        {
        }
    }
}