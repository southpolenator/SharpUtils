using Xunit;

namespace SharpUtilities.Tests
{
    public class DictionaryCacheTests
    {
        [Fact]
        public void SimpleTest()
        {
            int[] values = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            int evaluationCount = 0;
            DictionaryCache<int, string> cache = new DictionaryCache<int, string>(key =>
            {
                evaluationCount++;
                return key.ToString();
            });

            Assert.Equal(0, cache.Count);
            foreach (int value in values)
                Assert.Equal(value.ToString(), cache[value]);
            Assert.Equal(values.Length, cache.Count);
            Assert.Equal(values.Length, evaluationCount);
            foreach (int value in values)
                Assert.Equal(value.ToString(), cache[value]);
            Assert.Equal(values.Length, evaluationCount);
        }

        [Fact]
        public void Clear()
        {
            int[] values = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            int evaluationCount = 0;
            DictionaryCache<int, string> cache = new DictionaryCache<int, string>(key =>
            {
                evaluationCount++;
                return key.ToString();
            });

            foreach (int value in values)
                Assert.Equal(value.ToString(), cache[value]);
            Assert.Equal(values.Length, cache.Count);
            Assert.Equal(values.Length, evaluationCount);
            cache.Clear();
            Assert.Equal(0, cache.Count);
            foreach (int value in values)
                Assert.Equal(value.ToString(), cache[value]);
            Assert.Equal(values.Length, cache.Count);
            Assert.Equal(2 * values.Length, evaluationCount);
        }

        [Fact]
        public void TryGetExistingValue()
        {
            int[] values = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            int evaluationCount = 0;
            DictionaryCache<int, string> cache = new DictionaryCache<int, string>(key =>
            {
                evaluationCount++;
                return key.ToString();
            });

            foreach (int value in values)
                Assert.Equal(value.ToString(), cache[value]);
            Assert.Equal(values.Length, cache.Count);
            Assert.Equal(values.Length, evaluationCount);
            Assert.True(cache.TryGetExistingValue(values[0], out string testValue));
            cache.Clear();
            Assert.False(cache.TryGetExistingValue(values[0], out testValue));
        }

        [Fact]
        public void TryGetValue()
        {
            int[] values = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            int evaluationCount = 0;
            DictionaryCache<int, string> cache = new DictionaryCache<int, string>(key =>
            {
                evaluationCount++;
                return key.ToString();
            });

            foreach (int value in values)
            {
                Assert.True(cache.TryGetValue(value, out string testValue));
                Assert.Equal(value.ToString(), testValue);
            }
            Assert.Equal(values.Length, cache.Count);
            Assert.Equal(values.Length, evaluationCount);
        }

        [Fact]
        public void RemoveEntry()
        {
            int[] values = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            int evaluationCount = 0;
            DictionaryCache<int, string> cache = new DictionaryCache<int, string>(key =>
            {
                evaluationCount++;
                return key.ToString();
            });

            foreach (int value in values)
                Assert.Equal(value.ToString(), cache[value]);
            Assert.Equal(values.Length, cache.Count);
            Assert.Equal(values.Length, evaluationCount);
            Assert.True(cache.TryGetExistingValue(values[0], out string testValue));
            Assert.True(cache.RemoveEntry(values[0], out testValue));
            Assert.False(cache.TryGetExistingValue(values[0], out testValue));
            Assert.False(cache.RemoveEntry(values[0], out testValue));
        }

        [Fact]
        public void Disposable()
        {
            int disposedCount = 0;
            using (DictionaryCache<int, DisposableAction> cache = new DictionaryCache<int, DisposableAction>(key => new DisposableAction(() => disposedCount++)))
            {
                var a0 = cache[0];

                cache.Clear();
                Assert.Equal(1, disposedCount);

                var a1 = cache[0];
            }
            Assert.Equal(2, disposedCount);
        }
    }
}
