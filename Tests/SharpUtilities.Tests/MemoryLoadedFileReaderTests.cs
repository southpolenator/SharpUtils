using System;
using System.IO;
using Xunit;

namespace SharpUtilities.Tests
{
    public class MemoryLoadedFileReaderTests
    {
        [Fact]
        public void ReadByte()
        {
            using (TempFile temp = new TempFile())
            {
                byte[] bytes = new byte[] { 127, 128, 129, 200 };

                File.WriteAllBytes(temp.Path, bytes);
                using (MemoryLoadedFile file = new MemoryLoadedFile(temp.Path))
                {
                    MemoryLoadedFileReader reader = new MemoryLoadedFileReader(file);

                    for (int i = 0; i < bytes.Length; i++)
                        Assert.Equal(bytes[i], reader.ReadByte());
                    Assert.Equal(0, reader.BytesRemaining);
                }
            }
        }

        [Fact]
        public void ReadShort()
        {
            using (TempFile temp = new TempFile())
            {
                byte[] bytes = new byte[] { 127, 0, 255, 255, 0, 1 };

                File.WriteAllBytes(temp.Path, bytes);
                using (MemoryLoadedFile file = new MemoryLoadedFile(temp.Path))
                {
                    MemoryLoadedFileReader reader = new MemoryLoadedFileReader(file);

                    Assert.Equal(127, reader.ReadShort());
                    Assert.Equal(-1, reader.ReadShort());
                    Assert.Equal(256, reader.ReadShort());
                    Assert.Equal(0, reader.BytesRemaining);
                }
            }
        }

        [Fact]
        public void ReadUshort()
        {
            using (TempFile temp = new TempFile())
            {
                byte[] bytes = new byte[] { 127, 0, 255, 255, 0, 1 };

                File.WriteAllBytes(temp.Path, bytes);
                using (MemoryLoadedFile file = new MemoryLoadedFile(temp.Path))
                {
                    MemoryLoadedFileReader reader = new MemoryLoadedFileReader(file);

                    Assert.Equal(127, reader.ReadUshort());
                    Assert.Equal(ushort.MaxValue, reader.ReadUshort());
                    Assert.Equal(256, reader.ReadUshort());
                    Assert.Equal(0, reader.BytesRemaining);
                }
            }
        }

        [Fact]
        public void ReadInt()
        {
            using (TempFile temp = new TempFile())
            {
                byte[] bytes = new byte[] { 127, 0, 0, 0, 255, 255, 255, 255, 0, 0, 0, 1 };

                File.WriteAllBytes(temp.Path, bytes);
                using (MemoryLoadedFile file = new MemoryLoadedFile(temp.Path))
                {
                    MemoryLoadedFileReader reader = new MemoryLoadedFileReader(file);

                    Assert.Equal(127, reader.ReadInt());
                    Assert.Equal(-1, reader.ReadInt());
                    Assert.Equal(256 * 256 * 256, reader.ReadInt());
                    Assert.Equal(0, reader.BytesRemaining);
                }
            }
        }

        [Fact]
        public void ReadUint()
        {
            using (TempFile temp = new TempFile())
            {
                byte[] bytes = new byte[] { 127, 0, 0, 0, 255, 255, 255, 255, 0, 0, 0, 1 };

                File.WriteAllBytes(temp.Path, bytes);
                using (MemoryLoadedFile file = new MemoryLoadedFile(temp.Path))
                {
                    MemoryLoadedFileReader reader = new MemoryLoadedFileReader(file);

                    Assert.Equal(127U, reader.ReadUint());
                    Assert.Equal(uint.MaxValue, reader.ReadUint());
                    Assert.Equal(256 * 256 * 256U, reader.ReadUint());
                    Assert.Equal(0, reader.BytesRemaining);
                }
            }
        }

        [Fact]
        public void ReadLong()
        {
            using (TempFile temp = new TempFile())
            {
                byte[] bytes = new byte[] { 127, 0, 0, 0, 0, 0, 0, 0, 255, 255, 255, 255, 255, 255, 255, 255, 0, 0, 0, 1, 0, 0, 0, 0 };

                File.WriteAllBytes(temp.Path, bytes);
                using (MemoryLoadedFile file = new MemoryLoadedFile(temp.Path))
                {
                    MemoryLoadedFileReader reader = new MemoryLoadedFileReader(file);

                    Assert.Equal(127, reader.ReadLong());
                    Assert.Equal(-1, reader.ReadLong());
                    Assert.Equal(256 * 256 * 256, reader.ReadLong());
                    Assert.Equal(0, reader.BytesRemaining);
                }
            }
        }

        [Fact]
        public void ReadUlong()
        {
            using (TempFile temp = new TempFile())
            {
                byte[] bytes = new byte[] { 127, 0, 0, 0, 0, 0, 0, 0, 255, 255, 255, 255, 255, 255, 255, 255, 0, 0, 0, 1, 0, 0, 0, 0 };

                File.WriteAllBytes(temp.Path, bytes);
                using (MemoryLoadedFile file = new MemoryLoadedFile(temp.Path))
                {
                    MemoryLoadedFileReader reader = new MemoryLoadedFileReader(file);

                    Assert.Equal(127U, reader.ReadUlong());
                    Assert.Equal(ulong.MaxValue, reader.ReadUlong());
                    Assert.Equal(256 * 256 * 256U, reader.ReadUlong());
                    Assert.Equal(0, reader.BytesRemaining);
                }
            }
        }

        [Fact]
        public void ReadCString()
        {
            using (TempFile temp = new TempFile())
            {
                byte[] bytes = new byte[] { (byte)'T', (byte)'e', (byte)'s', (byte)'t', 0 };

                File.WriteAllBytes(temp.Path, bytes);
                using (MemoryLoadedFile file = new MemoryLoadedFile(temp.Path))
                {
                    MemoryLoadedFileReader reader = new MemoryLoadedFileReader(file);

                    Assert.Equal("Test", reader.ReadCString());
                    Assert.Equal(0, reader.BytesRemaining);
                }
            }
        }

        [Fact]
        public void ReadCStringWide()
        {
            using (TempFile temp = new TempFile())
            {
                byte[] bytes = new byte[] { (byte)'T', 0, (byte)'e', 0, (byte)'s', 0, (byte)'t', 0, 0, 0 };

                File.WriteAllBytes(temp.Path, bytes);
                using (MemoryLoadedFile file = new MemoryLoadedFile(temp.Path))
                {
                    MemoryLoadedFileReader reader = new MemoryLoadedFileReader(file);

                    Assert.Equal("Test", reader.ReadCStringWide());
                    Assert.Equal(0, reader.BytesRemaining);
                }
            }
        }

        [Fact]
        public unsafe void ReadBytes()
        {
            using (TempFile temp = new TempFile())
            {
                byte[] bytes = Guid.NewGuid().ToByteArray();

                File.WriteAllBytes(temp.Path, bytes);
                using (MemoryLoadedFile file = new MemoryLoadedFile(temp.Path))
                {
                    MemoryLoadedFileReader reader = new MemoryLoadedFileReader(file);

                    byte[] readBytes = new byte[reader.BytesRemaining];

                    fixed (byte* b = readBytes)
                    {
                        reader.ReadBytes(b, (uint)readBytes.Length);
                    }
                    Assert.Equal(bytes, readBytes);
                    Assert.Equal(0, reader.BytesRemaining);
                }
            }
        }

        [Fact]
        public unsafe void Move()
        {
            using (TempFile temp = new TempFile())
            {
                byte[] bytes = Guid.NewGuid().ToByteArray();

                File.WriteAllBytes(temp.Path, bytes);
                using (MemoryLoadedFile file = new MemoryLoadedFile(temp.Path))
                {
                    MemoryLoadedFileReader reader = new MemoryLoadedFileReader(file);

                    Assert.Equal(bytes.Length, reader.Length);
                    reader.Move((uint)bytes.Length / 2);
                    Assert.Equal(bytes.Length / 2, reader.BytesRemaining);
                    reader.Position += bytes.Length / 2;
                    Assert.Equal(0, reader.BytesRemaining);
                }
            }
        }

        [Fact]
        public unsafe void Duplicate()
        {
            using (TempFile temp = new TempFile())
            {
                byte[] bytes = Guid.NewGuid().ToByteArray();

                File.WriteAllBytes(temp.Path, bytes);
                using (MemoryLoadedFile file = new MemoryLoadedFile(temp.Path))
                {
                    MemoryLoadedFileReader reader = new MemoryLoadedFileReader(file);
                    reader.Position = 5;
                    IBinaryReader reader2 = reader.Duplicate();

                    Assert.Equal(reader.Position, reader2.Position);
                    reader.Position++;
                    Assert.NotEqual(reader.Position, reader2.Position);
                    reader2.Position++;
                    Assert.Equal(reader.Position, reader2.Position);
                    reader2.Position++;
                    Assert.NotEqual(reader.Position, reader2.Position);
                }
            }
        }
    }
}
