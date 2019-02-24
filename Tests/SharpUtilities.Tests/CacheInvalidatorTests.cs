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
    }
}
