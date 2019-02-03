using Xunit;

namespace SharpUtils.Tests
{
    public class SimpleCacheTests
    {
        [Fact]
        public void SingleEvaluation()
        {
            int count = 0;
            SimpleCache<int> cache = SimpleCache.Create(() =>
            {
                count++;
                return 42;
            });
            Assert.Equal(0, count);
            Assert.False(cache.Cached);
            Assert.Equal(42, cache.Value);
            Assert.Equal(1, count);
            Assert.True(cache.Cached);
            Assert.Equal(42, cache.Value);
            Assert.Equal(1, count);
            Assert.Single(cache, 42);
        }

        [Fact]
        public void EvaluationAfterInvalidate()
        {
            int count = 0;
            SimpleCache<int> cache = SimpleCache.Create(() =>
            {
                count++;
                return 42;
            });
            Assert.Equal(0, count);
            Assert.Equal(42, cache.Value);
            Assert.Equal(1, count);
            Assert.Equal(42, cache.Value);
            Assert.Equal(1, count);
            cache.InvalidateCache();
            Assert.Equal(1, count);
            Assert.Equal(42, cache.Value);
            Assert.Equal(2, count);
        }

        [Fact]
        public void SettingValue()
        {
            int count = 0;
            SimpleCache<int> cache = SimpleCache.Create(() =>
            {
                count++;
                return 42;
            });
            Assert.Equal(0, count);
            Assert.Equal(42, cache.Value);
            Assert.Equal(1, count);
            cache.Value = 12345;
            Assert.Equal(12345, cache.Value);
            Assert.Equal(1, count);
            cache.InvalidateCache();
            Assert.Equal(1, count);
            Assert.Equal(42, cache.Value);
            Assert.Equal(2, count);
        }

        [Fact]
        public void Assignment()
        {
            int count = 0;
            SimpleCache<int> cache = SimpleCache.Create(() =>
            {
                count++;
                return 42;
            });
            var cache2 = cache;
            Assert.Equal(0, count);
            Assert.False(cache.Cached);
            Assert.Equal(42, cache.Value);
            Assert.Equal(1, count);
            Assert.True(cache.Cached);
            Assert.True(cache2.Cached);
            Assert.Equal(42, cache2.Value);
            Assert.Equal(1, count);
            var cache3 = cache;
            Assert.True(cache3.Cached);
            Assert.Equal(42, cache3.Value);
            Assert.Equal(1, count);
        }
    }

    public class SimpleCacheStructTests
    {
        [Fact]
        public void SingleEvaluation()
        {
            int count = 0;
            SimpleCacheStruct<int> cache = SimpleCache.CreateStruct(() =>
            {
                count++;
                return 42;
            });
            Assert.Equal(0, count);
            Assert.False(cache.Cached);
            Assert.Equal(42, cache.Value);
            Assert.Equal(1, count);
            Assert.True(cache.Cached);
            Assert.Equal(42, cache.Value);
            Assert.Equal(1, count);
            Assert.Single(cache, 42);
        }

        [Fact]
        public void EvaluationAfterInvalidate()
        {
            int count = 0;
            SimpleCacheStruct<int> cache = SimpleCache.CreateStruct(() =>
            {
                count++;
                return 42;
            });
            Assert.Equal(0, count);
            Assert.Equal(42, cache.Value);
            Assert.Equal(1, count);
            Assert.Equal(42, cache.Value);
            Assert.Equal(1, count);
            cache.InvalidateCache();
            Assert.Equal(1, count);
            Assert.Equal(42, cache.Value);
            Assert.Equal(2, count);
        }

        [Fact]
        public void SettingValue()
        {
            int count = 0;
            SimpleCacheStruct<int> cache = SimpleCache.CreateStruct(() =>
            {
                count++;
                return 42;
            });
            Assert.Equal(0, count);
            Assert.Equal(42, cache.Value);
            Assert.Equal(1, count);
            cache.Value = 12345;
            Assert.Equal(12345, cache.Value);
            Assert.Equal(1, count);
            cache.InvalidateCache();
            Assert.Equal(1, count);
            Assert.Equal(42, cache.Value);
            Assert.Equal(2, count);
        }

        [Fact]
        public void Assignment()
        {
            int count = 0;
            SimpleCacheStruct<int> cache = SimpleCache.CreateStruct(() =>
            {
                count++;
                return 42;
            });
            var cache2 = cache;
            Assert.Equal(0, count);
            Assert.False(cache.Cached);
            Assert.Equal(42, cache.Value);
            Assert.Equal(1, count);
            Assert.True(cache.Cached);
            Assert.False(cache2.Cached);
            Assert.Equal(42, cache2.Value);
            Assert.Equal(2, count);
            Assert.True(cache2.Cached);
            var cache3 = cache;
            Assert.True(cache3.Cached);
            Assert.Equal(42, cache3.Value);
            Assert.Equal(2, count);
        }
    }
}
