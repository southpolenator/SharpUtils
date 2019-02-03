using System.Collections;

namespace SharpUtils
{
    /// <summary>
    /// Interface for all caching structures.
    /// IEnumerable implementation is responsible for returning all
    /// cached objects in this cache.
    /// </summary>
    public interface ICache : IEnumerable
    {
        /// <summary>
        /// Invalidates this cache.
        /// </summary>
        void InvalidateCache();
    }
}
