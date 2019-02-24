using Xunit;

namespace SharpUtilities.Tests
{
    public class ArrayCacheTests
    {
        private readonly string[] testArray = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };

        [Fact]
        public void SimpleTest()
        {
            int populateCount = 0;
            ArrayCache<string> arrayCache = new ArrayCache<string>(testArray.Length, index =>
            {
                populateCount++;
                return testArray[index];
            });

            Assert.Equal(testArray.Length, arrayCache.Count);
            for (int i = 0; i < arrayCache.Count; i++)
                Assert.Equal(testArray[i], arrayCache[i]);
            Assert.Equal(testArray.Length, populateCount);
            for (int i = 0; i < arrayCache.Count; i++)
                Assert.Equal(testArray[i], arrayCache[i]);
            Assert.Equal(testArray.Length, populateCount);
        }

        [Fact]
        public void Clear()
        {
            int populateCount = 0;
            ArrayCache<string> arrayCache = new ArrayCache<string>(testArray.Length, index =>
            {
                populateCount++;
                return testArray[index];
            });

            Assert.Equal(testArray.Length, arrayCache.Count);
            for (int i = 0; i < arrayCache.Count; i++)
                Assert.Equal(testArray[i], arrayCache[i]);
            Assert.Equal(testArray.Length, populateCount);
            arrayCache.Clear();
            for (int i = 0; i < arrayCache.Count; i++)
                Assert.Equal(testArray[i], arrayCache[i]);
            Assert.Equal(testArray.Length * 2, populateCount);
            arrayCache.InvalidateCache();
            for (int i = 0; i < arrayCache.Count; i++)
                Assert.Equal(testArray[i], arrayCache[i]);
            Assert.Equal(testArray.Length * 3, populateCount);
        }

        [Fact]
        public void Disposable()
        {
            int disposedCount = 0;
            using (ArrayCache<DisposableAction> arrayCache = new ArrayCache<DisposableAction>(1, index => new DisposableAction(() => disposedCount++)))
            {
                var a0 = arrayCache[0];

                arrayCache.Clear();
                Assert.Equal(1, disposedCount);

                var a1 = arrayCache[0];
            }
            Assert.Equal(2, disposedCount);
        }
    }
}
