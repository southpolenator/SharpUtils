﻿using System;
using System.Collections;

namespace SharpUtilities
{
    /// <summary>
    /// Interface for all caching structures.
    /// <see cref="IEnumerable"/> implementation is responsible for returning all cached objects in this cache.
    /// <see cref="IDisposable"/> implementation is responsible for clearing the cache.
    /// All caches are aware of <see cref="IDisposable"/> object that can be stored inside and will dispose them on clear.
    /// </summary>
    public interface ICache : IEnumerable, IDisposable
    {
        /// <summary>
        /// Invalidates this cache.
        /// </summary>
        void InvalidateCache();
    }
}
