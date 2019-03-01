using System;
using System.IO;
using Xunit;

namespace SharpUtilities.Tests
{
    public class MemoryLoadedFileTests
    {
        [Fact]
        public unsafe void SimpleByteArray()
        {
            using (TempFile temp = new TempFile())
            {
                byte[] bytes = new byte[] { 127, 128, 129, 200 };

                File.WriteAllBytes(temp.Path, bytes);
                using (MemoryLoadedFile file = new MemoryLoadedFile(temp.Path))
                {
                    Assert.Equal(bytes.Length, file.Length);
                    Assert.NotEqual(IntPtr.Zero, new IntPtr(file.BasePointer));
                    for (int i = 0; i < bytes.Length; i++)
                        Assert.Equal(bytes[i], file.BasePointer[i]);
                }
            }
        }

        [Fact]
        public void Sad()
        {
            Assert.ThrowsAny<Exception>(() => new MemoryLoadedFile(Guid.NewGuid().ToString()));
        }
    }
}
