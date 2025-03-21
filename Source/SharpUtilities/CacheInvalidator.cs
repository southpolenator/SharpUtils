using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace SharpUtilities
{
    /// <summary>
    /// Helper class that is used to collect caches that needs to be invalidated on particular event.
    /// </summary>
    public class CacheInvalidator : ICache
    {
        /// <summary>
        /// Collected caches that should be invalidated.
        /// </summary>
        public ConcurrentBag<ICache> Caches { get; private set; } = new ConcurrentBag<ICache>();

        /// <summary>
        /// Creates <see cref="DictionaryCache{TKey, TValue}"/> that is added to <see cref="Caches"/> when it is populated.
        /// </summary>
        /// <typeparam name="TKey">Dictionary cache key type.</typeparam>
        /// <typeparam name="TValue">Dictionary cache value type.</typeparam>
        /// <param name="populateAction">Function that will populate dictionary cache entries.</param>
        public DictionaryCache<TKey, TValue> CreateDictionaryCache<TKey, TValue>(Func<TKey, TValue> populateAction)
            where TKey : notnull
        {
            DictionaryCache<TKey, TValue>? dictionaryCache = null;
            Func<TKey, TValue> cachedPopulateAction = (key) =>
            {
                if (dictionaryCache?.Count == 0)
                    Caches.Add(dictionaryCache);
                return populateAction(key);
            };
            dictionaryCache = new DictionaryCache<TKey, TValue>(cachedPopulateAction);
            return dictionaryCache;
        }

        /// <summary>
        /// Creates <see cref="SimpleCache{T}"/> that is added to <see cref="Caches"/> when it is populated.
        /// </summary>
        /// <typeparam name="T">Simple cache value type.</typeparam>
        /// <param name="populateAction">Function that will populate simple cache.</param>
        public SimpleCache<T> CreateSimpleCache<T>(Func<T> populateAction)
        {
            SimpleCache<T>? simpleCache = null;
            Func<T> cachedPopulateAction = () =>
            {
                if (simpleCache != null)
                    Caches.Add(simpleCache);
                return populateAction();
            };
            simpleCache = SimpleCache.Create(cachedPopulateAction);
            return simpleCache;
        }

        /// <summary>
        /// Invalidate cache entry.
        /// </summary>
        public void InvalidateCache()
        {
            ConcurrentBag<ICache> oldCaches = Caches;
            Caches = new ConcurrentBag<ICache>();
            foreach (ICache cache in oldCaches)
                cache.InvalidateCache();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            InvalidateCache();
        }

        /// <summary>
        /// Gets enumerator for all the cached objects in this cache.
        /// </summary>
        public IEnumerator GetEnumerator()
        {
            return Caches.GetEnumerator();
        }

        /// <summary>
        /// Invalidates all the instances of type <see cref="ICache" /> which are fields of
        /// class given as root object and any fields of the same type in child fields recursively.
        /// Use it when there are massive changes and all the caches need to be invalidated.
        /// </summary>
        /// <param name="root">Root object from which we recursively drop the caches.</param>
        public static void InvalidateCaches<T>(T? root)
            where T : class
        {
            CacheInvalidator<T>.Invalidate(root);
        }

        /// <summary>
        /// Invalidates all the instances of type <see cref="ICache" /> which are fields of
        /// struct given as root struct and any fields of the same type in child fields recursively.
        /// Use it when there are massive changes and all the caches need to be invalidated.
        /// </summary>
        /// <param name="root">Root struct from which we recursively drop the caches.</param>
        public static void InvalidateCaches<T>(ref T root)
            where T : struct
        {
            StructCacheInvalidator<T>.Invalidate(ref root);
        }

        /// <summary>
        /// Invalidates cache entries of the <see cref="ICache{T}"/> object.
        /// </summary>
        /// <param name="root">Cache we are invalidating.</param>
        internal static void InvalidateCacheEntries<T>(ICache<T> root)
            where T : class
        {
            foreach (T value in root)
                InvalidateCaches<T>(value);
        }

        /// <summary>
        /// Generates IL code for type invalidation delegate. See helper classes that are using it.
        /// </summary>
        /// <param name="ilGenerator">IL generator.</param>
        /// <typeparam name="T">Type which cache fields should be invalidated.</typeparam>
        internal static void GenerateInvalidateIL<T>(ILGenerator ilGenerator)
        {
            IEnumerable<FieldInfo> cacheFields =
                typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(fieldInfo => fieldInfo.FieldType.GetInterfaces().Contains(typeof(ICache)));
            var invalidateCacheMethod = typeof(ICache).GetMethod(nameof(ICache.InvalidateCache))!;
            foreach (FieldInfo cacheField in cacheFields)
            {
                // Check if cacheField type is struct or class
                bool fieldIsStruct = cacheField.FieldType.GetTypeInfo().IsValueType;

                // Check if field can be null
                bool fieldCanBeNull = !fieldIsStruct;

                // Check if cacheField is ICache<struct> or ICache<class> or just ICache
                Type? cacheInterface = cacheField.FieldType.GetInterfaces().Where(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(ICache<>)).FirstOrDefault();
                bool internalTypeIsStruct = false;

                if (cacheInterface != null)
                    internalTypeIsStruct = cacheInterface.GenericTypeArguments[0].GetTypeInfo().IsValueType;

                // Check if it is null
                Label skipLabel = ilGenerator.DefineLabel();

                if (fieldCanBeNull)
                {
                    ilGenerator.Emit(OpCodes.Ldarg_0);
                    ilGenerator.Emit(OpCodes.Ldfld, cacheField);
                    ilGenerator.Emit(OpCodes.Ldnull);
                    ilGenerator.Emit(OpCodes.Ceq);
                    ilGenerator.Emit(OpCodes.Brtrue_S, skipLabel);
                }

                // Call InvalidateCacheEntries only if it is ICache<class>
                if (!fieldIsStruct && cacheInterface != null && !internalTypeIsStruct)
                {
                    ilGenerator.Emit(OpCodes.Ldarg_0);
                    ilGenerator.Emit(OpCodes.Ldfld, cacheField);
                    ilGenerator.Emit(OpCodes.Call, typeof(CacheInvalidator).GetMethod(nameof(CacheInvalidator.InvalidateCacheEntries))!.MakeGenericMethod(cacheInterface.GenericTypeArguments));
                }

                // Clear cache
                if (fieldIsStruct)
                {
                    ilGenerator.Emit(OpCodes.Ldarg_0);
                    ilGenerator.Emit(OpCodes.Ldflda, cacheField);
                    var interfaceMapping = cacheField.FieldType.GetTypeInfo().GetRuntimeInterfaceMap(typeof(ICache));
                    for (int i = 0; i < interfaceMapping.InterfaceMethods.Length; i++)
                        if (interfaceMapping.InterfaceMethods[i] == invalidateCacheMethod)
                        {
                            ilGenerator.Emit(OpCodes.Call, interfaceMapping.TargetMethods[i]);
                            break;
                        }
                }
                else
                {
                    ilGenerator.Emit(OpCodes.Ldarg_0);
                    ilGenerator.Emit(OpCodes.Ldfld, cacheField);
                    ilGenerator.Emit(OpCodes.Callvirt, invalidateCacheMethod);
                }

                if (fieldCanBeNull)
                    ilGenerator.MarkLabel(skipLabel);
            }
            ilGenerator.Emit(OpCodes.Ret);
        }
    }

    /// <summary>
    /// Helper class that invalidates cache of object of a given type.
    /// </summary>
    /// <typeparam name="T">Type that should be invalidated.</typeparam>
    internal static class CacheInvalidator<T>
        where T : class
    {
        /// <summary>
        /// Delegate that invalidates object. It is created as dynamic method to insure struct invalidation.
        /// </summary>
        private static Action<T> invalidateDelegate;

        static CacheInvalidator()
        {
            var dynamicMethod = new DynamicMethod
            (
                nameof(Invalidate),
                typeof(void),
                new[] { typeof(T) },
                typeof(T).GetTypeInfo().Module
            );

            var ilGenerator = dynamicMethod.GetILGenerator();

            CacheInvalidator.GenerateInvalidateIL<T>(ilGenerator);
            invalidateDelegate = (Action<T>)dynamicMethod.CreateDelegate(typeof(Action<T>));
        }

        /// <summary>
        /// Helper method that is implementation of <see cref="CacheInvalidator.InvalidateCaches{T}(T)"/>.
        /// </summary>
        /// <param name="root">Root object that should be invalidated.</param>
        public static void Invalidate(T? root)
        {
            if (root == null)
                return;
            invalidateDelegate(root);
        }
    }

    /// <summary>
    /// Helper class that invalidates cache of struct of a given type.
    /// </summary>
    /// <typeparam name="T">Type that should be invalidated.</typeparam>
    internal static class StructCacheInvalidator<T>
        where T : struct
    {
        private delegate void InvalidateDelegateType(ref T root);

        /// <summary>
        /// Delegate that invalidates object. It is created as dynamic method to insure struct invalidation.
        /// </summary>
        private static InvalidateDelegateType invalidateDelegate;

        static StructCacheInvalidator()
        {
            var dynamicMethod = new DynamicMethod
            (
                nameof(Invalidate),
                typeof(void),
                new[] { typeof(InvalidateDelegateType).GetMethod("Invoke")!.GetParameters()[0].ParameterType },
                typeof(T).GetTypeInfo().Module
            );

            var ilGenerator = dynamicMethod.GetILGenerator();

            CacheInvalidator.GenerateInvalidateIL<T>(ilGenerator);
            invalidateDelegate = (InvalidateDelegateType)dynamicMethod.CreateDelegate(typeof(InvalidateDelegateType));
        }

        /// <summary>
        /// Helper method that is implementation of <see cref="CacheInvalidator.InvalidateCaches{T}(ref T)"/>.
        /// </summary>
        /// <param name="root">Root struct that should be invalidated.</param>
        public static void Invalidate(ref T root)
        {
            invalidateDelegate(ref root);
        }
    }
}
