using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SharpUtilities
{
    /// <summary>
    /// Helper class for caching objects inside the array. New object will be cached on the request.
    /// </summary>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    public class ArrayCache<TValue> : ICache<TValue>
        where TValue : class
    {
        /// <summary>
        /// The populate action
        /// </summary>
        private readonly Func<int, TValue> populateAction;

        /// <summary>
        /// The cached values
        /// </summary>
        private TValue[] values;

        /// <summary>
        /// Disables locking during value population.
        /// </summary>
        private readonly bool disableLocking;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayCache{TValue}"/> class.
        /// </summary>
        /// <param name="length">Length of the array.</param>
        /// <param name="populateAction">The populate action.</param>
        public ArrayCache(int length, Func<int, TValue> populateAction)
            : this(length, false, populateAction)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayCache{TValue}"/> class.
        /// </summary>
        /// <param name="length">Length of the array.</param>
        /// <param name="disableLocking">Disables locking during value population. It is not recommended to disable locking when using <see cref="IDisposable"/> values.</param>
        /// <param name="populateAction">The populate action.</param>
        public ArrayCache(int length, bool disableLocking, Func<int, TValue> populateAction)
        {
            this.populateAction = populateAction;
            this.disableLocking = disableLocking;
            values = new TValue[length];
        }

        /// <summary>
        /// Removes all items from this cache.
        /// </summary>
        public void Clear()
        {
            TValue[] valuesToBeDisposed = values;

            values = new TValue[values.Length];
            if (typeof(IDisposable).IsAssignableFrom(typeof(TValue)))
                lock (this)
                    foreach (IDisposable value in valuesToBeDisposed)
                        value.Dispose();
        }

        /// <summary>
        /// Gets the number of entries inside the cache.
        /// </summary>
        public int Count => values.Length;

        /// <summary>
        /// Gets or sets the &lt;TValue&gt; with the specified index.
        /// </summary>
        /// <param name="index">The array index.</param>
        public TValue this[int index]
        {
            get
            {
                TValue value = values[index];

                if (value == null)
                    if (disableLocking)
                        value = values[index] = populateAction(index);
                    else
                        lock (this)
                        {
                            value = values[index];
                            if (value == null)
                                values[index] = value = populateAction(index);
                        }

                return value;
            }

            set
            {
                values[index] = value;
            }
        }

        /// <summary>
        /// Returns all cached values in this cache.
        /// </summary>
        /// <returns>IEnumerator of all the cache values.</returns>
        public IEnumerator<TValue> GetEnumerator()
        {
            return values.Where(v => v != null).GetEnumerator();
        }

        /// <summary>
        /// Returns all cached values in this cache.
        /// </summary>
        /// <returns>IEnumerator of all the cache values.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return values.Where(v => v != null).GetEnumerator();
        }

        /// <summary>
        /// Invalidates this cache.
        /// </summary>
        public void InvalidateCache()
        {
            Clear();
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
