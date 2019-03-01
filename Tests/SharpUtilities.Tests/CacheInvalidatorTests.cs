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

        class CacheTestClass
        {
            public CacheInvalidator CacheInvalidator;
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
            Assert.Equal(0, dictionaryCache.Count);
            Assert.False(simpleCache.Cached);
        }
    }
}
