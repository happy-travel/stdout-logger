﻿using System;
using Microsoft.Extensions.Logging;

namespace HappyTravel.StdOutLogger.Internals
{
    internal class NullExternalScopeProvider : IExternalScopeProvider
    {
        private NullExternalScopeProvider()
        {
        }


        public static IExternalScopeProvider Instance { get; } = new NullExternalScopeProvider();


        void IExternalScopeProvider.ForEachScope<TState>(Action<object, TState> callback, TState state)
        {
        }


        IDisposable IExternalScopeProvider.Push(object state)
        {
            return NullScope.Instance;
        }
    }
}