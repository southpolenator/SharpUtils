using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace SharpUtilities
{
    /// <summary>
    /// Helper class for caching results - it is being used as lazy evaluation
    /// </summary>
    public static class SimpleCache
    {
        /// <summary>
        /// Creates a new instance of the <see cref="SimpleCache{T}" /> class.
        /// </summary>
        /// <typeparam name="T">Type to be cached</typeparam>
        /// <param name="populateAction">The function that populates the cache on demand.</param>
        /// <returns>Simple cache of &lt;T&gt;</returns>
        public static SimpleCache<T> Create<T>(Func<T> populateAction)
        {
            return new SimpleCache<T>(populateAction);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SimpleCacheStruct{T}" /> class.
        /// </summary>
        /// <typeparam name="T">Type to be cached</typeparam>
        /// <param name="populateAction">The function that populates the cache on demand.</param>
        /// <returns>Simple cache struct of &lt;T&gt;</returns>
        public static SimpleCacheStruct<T> CreateStruct<T>(Func<T> populateAction)
        {
            return new SimpleCacheStruct<T>(populateAction);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SimpleCacheStruct{T}" /> class.
        /// </summary>
        /// <typeparam name="T">Type to be cached</typeparam>
        /// <typeparam name="ContextType">Type of the context.</typeparam>
        /// <param name="context">The context object</param>
        /// <param name="populateAction">The function that populates the cache on demand.</param>
        /// <returns>Simple cache with context of &lt;T&gt;</returns>
        public static SimpleCacheWithContext<T, ContextType> CreateWithContext<T, ContextType>(ContextType context, Func<ContextType, T> populateAction)
            where ContextType : class
        {
            return new SimpleCacheWithContext<T, ContextType>(context, populateAction);
        }
    }

    /// <summary>
    /// Helper class for caching results - it is being used as lazy evaluation
    /// </summary>
    /// <typeparam name="T">Type to be cached</typeparam>
    public class SimpleCache<T> : ICache<T>
    {
        /// <summary>
        /// The populate action
        /// </summary>
        private readonly Func<T?> populateAction;

        /// <summary>
        /// The value that is cached
        /// </summary>
        private T? value;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleCache{T}"/> class.
        /// </summary>
        /// <param name="populateAction">The function that populates the cache on demand.</param>
        public SimpleCache(Func<T?> populateAction)
        {
            this.populateAction = populateAction;
            value = default(T);
        }

        /// <summary>
        /// Gets a value indicating whether value is cached.
        /// </summary>
        /// <value>
        ///   <c>true</c> if cached; otherwise, <c>false</c>.
        /// </value>
        public bool Cached { get; private set; }

        /// <summary>
        /// Gets or sets the value. The value will be populated if it wasn't cached.
        /// </summary>
        public T? Value
        {
            get
            {
                if (!Cached)
                    lock (this)
                        if (!Cached)
                        {
                            value = populateAction();
                            Cached = true;
                        }
                return value;
            }

            set
            {
                lock (populateAction)
                {
                    this.value = value;
                    Cached = true;
                }
            }
        }

        /// <summary>
        /// Gets enumerator for all the cached objects in this cache.
        /// </summary>
        /// <returns></returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            if (Cached)
#pragma warning disable 414, CS8603
                yield return value;
#pragma warning restore CS8603
        }

        /// <summary>
        /// Gets enumerator for all the cached objects in this cache.
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            if (Cached)
                yield return value;
        }

        /// <summary>
        /// Invalidate cache entry.
        /// </summary>
        public void InvalidateCache()
        {
            if (typeof(IDisposable).IsAssignableFrom(typeof(T)))
            {
                if (Cached)
                    lock (this)
                        if (Cached)
                        {
#pragma warning disable 414, CS8600
                            ((IDisposable)value)?.Dispose();
#pragma warning restore CS8600
                            Cached = false;
                        }
            }
            else
                Cached = false;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            InvalidateCache();
        }
    }

    /// <summary>
    /// Helper structure for caching results - it is being used as lazy evaluation
    /// </summary>
    /// <remarks>
    /// This variant is meant to be used when smaller amount of memory is supposed to be used or less allocations.
    /// </remarks>
    /// <typeparam name="T">Type to be cached</typeparam>
    public struct SimpleCacheStruct<T> : ICache<T>
    {
        /// <summary>
        /// The populate action
        /// </summary>
        private readonly Func<T?> populateAction;

        /// <summary>
        /// The value that is cached
        /// </summary>
        private T? value;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleCacheStruct{T}"/> class.
        /// </summary>
        /// <param name="populateAction">The function that populates the cache on demand.</param>
        public SimpleCacheStruct(Func<T?> populateAction)
        {
            this.populateAction = populateAction;
#pragma warning disable 414, CS8653
            value = default(T);
#pragma warning restore CS8653
            Cached = false;
        }

        /// <summary>
        /// Gets a value indicating whether value is cached.
        /// </summary>
        /// <value>
        ///   <c>true</c> if cached; otherwise, <c>false</c>.
        /// </value>
        public bool Cached { get; private set; }

        /// <summary>
        /// Gets or sets the value. The value will be populated if it wasn't cached.
        /// </summary>
        public T? Value
        {
            get
            {
                if (!Cached)
                    lock (populateAction)
                        if (!Cached)
                        {
                            value = populateAction();
                            Cached = true;
                        }
                return value;
            }

            set
            {
                lock (populateAction)
                {
                    this.value = value;
                    Cached = true;
                }
            }
        }

        /// <summary>
        /// Invalidate cache entry.
        /// </summary>
        public void InvalidateCache()
        {
            if (typeof(IDisposable).IsAssignableFrom(typeof(T)))
            {
                if (Cached)
                    lock (populateAction)
                        if (Cached)
                        {
#pragma warning disable 414, CS8600
                            ((IDisposable)value)?.Dispose();
#pragma warning restore CS8600
                            Cached = false;
                        }
            }
            else
                Cached = false;
        }

        /// <summary>
        /// Gets enumerator for all the cached objects in this cache.
        /// </summary>
        /// <returns></returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            if (Cached)
#pragma warning disable 414, CS8603
                yield return value;
#pragma warning restore CS8603
        }

        /// <summary>
        /// Gets enumerator for all the cached objects in this cache.
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            if (Cached)
                yield return value;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            InvalidateCache();
        }
    }

    /// <summary>
    /// Helper structure for caching results - it is being used as lazy evaluation.
    /// </summary>
    /// <remarks>
    /// This variant is meant to be used when minimal memory supposed to be used - when lots of objects
    /// of the same time should be created in the runtime.
    /// </remarks>
    /// <example>
    /// <code>
    /// class Test
    /// {
    ///     private Func&lt;Test, int&gt; staticDelegate = (t) => t.LazyInitialization();
    ///     private SimpleCacheWithContext&lt;int, Test&gt; cache;
    ///
    ///     public Test()
    ///     {
    ///         cache = SimpleCache.CreateWithContext(staticDelegate, this);
    ///     }
    ///
    ///     public int Data => cache.Value;
    ///
    ///     private int LazyInitialization()
    ///     {
    ///         // Some complex long lasting lazy evaluation
    ///         return 42;
    ///     }
    /// }
    /// </code>
    /// </example>
    /// <typeparam name="T">Type to be cached</typeparam>
    /// <typeparam name="ContextType">Type of the context.</typeparam>
    public struct SimpleCacheWithContext<T, ContextType> : ICache<T>
        where ContextType : class
    {
        /// <summary>
        /// The populate action
        /// </summary>
        private readonly Func<ContextType, T?> populateAction;

        /// <summary>
        /// The context object.
        /// </summary>
        private readonly ContextType context;

        /// <summary>
        /// The value that is cached
        /// </summary>
        private T? value;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleCacheWithContext{T, ContextType}"/> class.
        /// </summary>
        /// <param name="context">The context object</param>
        /// <param name="populateAction">The function that populates the cache on demand.</param>
        public SimpleCacheWithContext(ContextType context, Func<ContextType, T?> populateAction)
        {
            this.populateAction = populateAction;
            this.context = context;
#pragma warning disable 414, CS8653
            value = default(T);
#pragma warning restore CS8653
            Cached = false;
        }

        /// <summary>
        /// Gets a value indicating whether value is cached.
        /// </summary>
        /// <value>
        ///   <c>true</c> if cached; otherwise, <c>false</c>.
        /// </value>
        public bool Cached { get; private set; }

        /// <summary>
        /// Gets or sets the value. The value will be populated if it wasn't cached.
        /// </summary>
        public T? Value
        {
            get
            {
                if (!Cached)
                    lock (context ?? (object)populateAction)
                        if (!Cached)
                        {
#pragma warning disable 414, CS8604
                            value = populateAction(context);
#pragma warning restore CS8604
                            Cached = true;
                        }
                return value;
            }

            set
            {
                lock (context ?? (object)populateAction)
                {
                    this.value = value;
                    Cached = true;
                }
            }
        }

        /// <summary>
        /// Invalidate cache entry.
        /// </summary>
        public void InvalidateCache()
        {
            if (typeof(IDisposable).IsAssignableFrom(typeof(T)))
            {
                if (Cached)
                    lock (context ?? (object)populateAction)
                        if (Cached)
                        {
#pragma warning disable 414, CS8600
                            ((IDisposable)value)?.Dispose();
#pragma warning restore CS8600
                            Cached = false;
                        }
            }
            else
                Cached = false;
        }

        /// <summary>
        /// Gets enumerator for all the cached objects in this cache.
        /// </summary>
        /// <returns></returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            if (Cached)
#pragma warning disable 414, CS8603
                yield return value;
#pragma warning restore CS8603
        }

        /// <summary>
        /// Gets enumerator for all the cached objects in this cache.
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            if (Cached)
                yield return value;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            InvalidateCache();
        }
    }
}
