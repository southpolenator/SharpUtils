using System;
using Xunit;

namespace SharpUtilities.Tests
{
    public class MemoryBufferTests
    {
        [Fact]
        public unsafe void UnsafeInitialization()
        {
            byte[] originalBytes = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            fixed (byte* pointer = originalBytes)
            {
                MemoryBuffer buffer = new MemoryBuffer(pointer, originalBytes.Length);

                Assert.Equal(new IntPtr(pointer), new IntPtr(buffer.BytePointer));
                Assert.Equal(originalBytes.Length, buffer.BytePointerLength);
                Assert.Equal(originalBytes, buffer.Bytes);
            }
        }

        [Fact]
        public unsafe void SafeInitialization()
        {
            byte[] originalBytes = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            MemoryBuffer buffer = new MemoryBuffer(originalBytes);

            Assert.Equal(IntPtr.Zero, new IntPtr(buffer.BytePointer));
            Assert.Equal(-1, buffer.BytePointerLength);
            Assert.Equal(originalBytes, buffer.Bytes);
        }

        [Fact]
        public unsafe void SadPath()
        {
            Assert.Throws<ArgumentNullException>(() => new MemoryBuffer(null));
            Assert.Throws<ArgumentNullException>(() => new MemoryBuffer(null, 0));
        }
    }
}
