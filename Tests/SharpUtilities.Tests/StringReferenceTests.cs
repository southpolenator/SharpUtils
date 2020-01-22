using System;
using Xunit;

namespace SharpUtilities.Tests
{
    public class StringReferenceTests
    {
        [Fact]
        public void Empty()
        {
            StringReference s = new StringReference();

            Assert.Equal(string.Empty, s.String);
            Assert.True(s.IsEmpty);
        }

        [Fact]
        public void StringInitialized()
        {
            StringReference s = new StringReference("Test");

            Assert.Equal("Test", s.ToString());
            Assert.False(s.IsEmpty);
        }

        [Fact]
        public void BytesUT8()
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes("Test");
            MemoryBuffer buffer = new MemoryBuffer(bytes);
            StringReference s = new StringReference(buffer, StringReference.Encoding.UTF8);

            Assert.False(s.IsEmpty);
            Assert.Equal("Test", s.String);
        }

        [Fact]
        public void BytesUnicode()
        {
            byte[] bytes = System.Text.Encoding.Unicode.GetBytes("Test");
            MemoryBuffer buffer = new MemoryBuffer(bytes);
            StringReference s = new StringReference(buffer, StringReference.Encoding.Unicode);

            Assert.False(s.IsEmpty);
            Assert.Equal("Test", s.String);
        }
    }
}
