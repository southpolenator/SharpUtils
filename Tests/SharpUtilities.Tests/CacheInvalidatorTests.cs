using System;
using Xunit;

namespace SharpUtilities.Tests
{
    public class CacheInvalidatorTests
    {
        [Fact]
        public void ComplexTest()
        {
            DictionaryCache<int, string> dictionaryCache;
            SimpleCache<int> simpleCache;

            using (CacheInvalidator cacheInvalidator = new CacheInvalidator())
            {
                dictionaryCache = cacheInvalidator.CreateDictionaryCache<int, string>(key => key.ToString());
                simpleCache = cacheInvalidator.CreateSimpleCache(() => 42);
                dictionaryCache.TryGetValue(simpleCache.Value, out string testValue);
            }
            Assert.Equal(0, dictionaryCache.Count);
            Assert.False(simpleCache.Cached);
        }

        [Fact]
        public void Enumerate()
        {
            DictionaryCache<int, string> dictionaryCache;
            SimpleCache<int> simpleCache;

            using (CacheInvalidator cacheInvalidator = new CacheInvalidator())
            {
                dictionaryCache = cacheInvalidator.CreateDictionaryCache<int, string>(key => key.ToString());
                simpleCache = cacheInvalidator.CreateSimpleCache(() => 42);
                dictionaryCache.TryGetValue(simpleCache.Value, out string testValue);
                Assert.NotEmpty(cacheInvalidator);
            }
        }

        internal class CacheTestClass
        {
            public CacheInvalidator? CacheInvalidator;
        }

        [Fact]
        public void InvalidateCaches()
        {
            DictionaryCache<int, string> dictionaryCache;
            SimpleCache<int> simpleCache;
            CacheTestClass test = new CacheTestClass();
            test.CacheInvalidator = new CacheInvalidator();
            dictionaryCache = test.CacheInvalidator.CreateDictionaryCache<int, string>(key => key.ToString());
            simpleCache = test.CacheInvalidator.CreateSimpleCache(() => 42);
            dictionaryCache.TryGetValue(simpleCache.Value, out string testValue);
            CacheInvalidator.InvalidateCaches(test);
            CacheInvalidator.InvalidateCaches<CacheTestClass>(null);
            CacheInvalidator.InvalidateCaches(new CacheTestClass());
            Assert.Equal(0, dictionaryCache.Count);
            Assert.False(simpleCache.Cached);
        }

        internal class DifferentCaches
        {
            internal SimpleCache<int> simpleCache;
            internal SimpleCacheStruct<int> simpleCacheStruct;
            internal SimpleCacheWithContext<int, DifferentCaches> simpleCacheWithContext;
            private static Func<DifferentCaches, int> simpleCacheWithContextStaticDelegate = (t) => 42;

            public DifferentCaches()
            {
                simpleCache = SimpleCache.Create(() => 42);
                simpleCacheStruct = SimpleCache.CreateStruct(() => 42);
                simpleCacheWithContext = SimpleCache.CreateWithContext(this, simpleCacheWithContextStaticDelegate);
            }

            public int SimpleCacheValue => simpleCache.Value;
            public int SimpleCacheStructValue => simpleCacheStruct.Value;
            public int SimpleCacheWithContextValue => simpleCacheWithContext.Value;
        }

        [Fact]
        public void InvalidateObject()
        {
            DifferentCaches differentCaches = new DifferentCaches();
            Assert.False(differentCaches.simpleCache.Cached);
            Assert.False(differentCaches.simpleCacheStruct.Cached);
            Assert.False(differentCaches.simpleCacheWithContext.Cached);
            Assert.Equal(42, differentCaches.SimpleCacheValue);
            Assert.Equal(42, differentCaches.SimpleCacheStructValue);
            Assert.Equal(42, differentCaches.SimpleCacheWithContextValue);
            Assert.True(differentCaches.simpleCache.Cached);
            Assert.True(differentCaches.simpleCacheStruct.Cached);
            Assert.True(differentCaches.simpleCacheWithContext.Cached);
            CacheInvalidator.InvalidateCaches(differentCaches);
            Assert.False(differentCaches.simpleCache.Cached);
            Assert.False(differentCaches.simpleCacheStruct.Cached);
            Assert.False(differentCaches.simpleCacheWithContext.Cached);
        }

        internal struct DifferentCachesStruct
        {
            internal SimpleCache<int> simpleCache;
            internal SimpleCacheStruct<int> simpleCacheStruct;

            public DifferentCachesStruct(bool unused)
            {
                simpleCache = SimpleCache.Create(() => 42);
                simpleCacheStruct = SimpleCache.CreateStruct(() => 42);
            }

            public int SimpleCacheValue => simpleCache.Value;
            public int SimpleCacheStructValue => simpleCacheStruct.Value;
        }

        [Fact]
        public void InvalidateStruct()
        {
            DifferentCachesStruct differentCaches = new DifferentCachesStruct(false);
            Assert.False(differentCaches.simpleCache.Cached);
            Assert.False(differentCaches.simpleCacheStruct.Cached);
            Assert.Equal(42, differentCaches.SimpleCacheValue);
            Assert.Equal(42, differentCaches.SimpleCacheStructValue);
            Assert.True(differentCaches.simpleCache.Cached);
            Assert.True(differentCaches.simpleCacheStruct.Cached);
            CacheInvalidator.InvalidateCaches(ref differentCaches);
            Assert.False(differentCaches.simpleCache.Cached);
            Assert.False(differentCaches.simpleCacheStruct.Cached);
        }
    }
}
