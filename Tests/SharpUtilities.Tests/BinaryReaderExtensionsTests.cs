using System;
using System.IO;
using Xunit;

namespace SharpUtilities.Tests
{
    public class BinaryReaderExtensionsTests
    {
        [Fact]
        public void ReadGuid()
        {
            using (TempFile temp = new TempFile())
            {
                Guid guid = Guid.NewGuid();
                byte[] bytes = guid.ToByteArray();

                File.WriteAllBytes(temp.Path, bytes);
                using (MemoryLoadedFile file = new MemoryLoadedFile(temp.Path))
                {
                    MemoryLoadedFileReader reader = new MemoryLoadedFileReader(file);

                    Assert.Equal(guid, reader.ReadGuid());
                    Assert.Equal(0, reader.BytesRemaining);
                }
            }
        }

        [Fact]
        public void ReadAllBytes()
        {
            using (TempFile temp = new TempFile())
            {
                byte[] bytes = Guid.NewGuid().ToByteArray();

                File.WriteAllBytes(temp.Path, bytes);
                using (MemoryLoadedFile file = new MemoryLoadedFile(temp.Path))
                {
                    MemoryLoadedFileReader reader = new MemoryLoadedFileReader(file);

                    Assert.Equal(bytes, reader.ReadAllBytes());
                    Assert.Equal(0, reader.BytesRemaining);
                }
            }
        }

        [Fact]
        public void ReadUshortArray()
        {
            using (TempFile temp = new TempFile())
            {
                byte[] bytes = new byte[] { 127, 0, 255, 255, 0, 1 };
                ushort[] validation = new ushort[] { 127, ushort.MaxValue, 256 };

                File.WriteAllBytes(temp.Path, bytes);
                using (MemoryLoadedFile file = new MemoryLoadedFile(temp.Path))
                {
                    MemoryLoadedFileReader reader = new MemoryLoadedFileReader(file);

                    Assert.Equal(validation, reader.ReadUshortArray(validation.Length));
                    Assert.Equal(0, reader.BytesRemaining);
                }
            }
        }

        [Fact]
        public void ReadUintArray()
        {
            using (TempFile temp = new TempFile())
            {
                byte[] bytes = new byte[] { 127, 0, 0, 0, 255, 255, 255, 255, 0, 0, 0, 1 };
                uint[] validation = new uint[] { 127, uint.MaxValue, 256 * 256 * 256U };

                File.WriteAllBytes(temp.Path, bytes);
                using (MemoryLoadedFile file = new MemoryLoadedFile(temp.Path))
                {
                    MemoryLoadedFileReader reader = new MemoryLoadedFileReader(file);

                    Assert.Equal(validation, reader.ReadUintArray(validation.Length));
                    Assert.Equal(0, reader.BytesRemaining);
                }
            }
        }

        [Fact]
        public void ReadSubstream()
        {
            using (TempFile temp = new TempFile())
            {
                Guid guid = Guid.NewGuid();
                byte[] bytes = guid.ToByteArray();

                File.WriteAllBytes(temp.Path, bytes);
                using (MemoryLoadedFile file = new MemoryLoadedFile(temp.Path))
                {
                    MemoryLoadedFileReader reader = new MemoryLoadedFileReader(file);
                    IBinaryReader reader2 = reader.ReadSubstream();

                    Assert.Equal(0, reader2.Position);
                    Assert.Equal(reader.Length, reader2.Length);
                    Assert.Equal(0, reader.BytesRemaining);
                    reader2.Position = 1;
                    Assert.Equal(1, reader2.Position);
                    Assert.Equal(0, reader.BytesRemaining);
                    Assert.NotEqual(reader.Position, reader2.Position);
                }
            }
        }

        [Fact]
        public void ReadSubstreamWithLength()
        {
            using (TempFile temp = new TempFile())
            {
                Guid guid = Guid.NewGuid();
                byte[] bytes = guid.ToByteArray();

                File.WriteAllBytes(temp.Path, bytes);
                using (MemoryLoadedFile file = new MemoryLoadedFile(temp.Path))
                {
                    MemoryLoadedFileReader reader = new MemoryLoadedFileReader(file);
                    IBinaryReader reader2 = reader.ReadSubstream(reader.Length);

                    Assert.Equal(0, reader2.Position);
                    Assert.Equal(reader.Length, reader2.Length);
                    Assert.Equal(0, reader.BytesRemaining);
                    reader2.Position = 1;
                    Assert.Equal(1, reader2.Position);
                    Assert.Equal(0, reader.BytesRemaining);
                    Assert.NotEqual(reader.Position, reader2.Position);
                }
            }
        }

        [Fact]
        public void Align()
        {
            using (TempFile temp = new TempFile())
            {
                Guid guid = Guid.NewGuid();
                byte[] bytes = guid.ToByteArray();

                File.WriteAllBytes(temp.Path, bytes);
                using (MemoryLoadedFile file = new MemoryLoadedFile(temp.Path))
                {
                    MemoryLoadedFileReader reader = new MemoryLoadedFileReader(file);

                    Assert.Equal(0, reader.Position);
                    reader.Align(2);
                    Assert.Equal(0, reader.Position);
                    reader.Align(4);
                    Assert.Equal(0, reader.Position);
                    reader.Align(8);
                    Assert.Equal(0, reader.Position);
                    reader.Position = 1;
                    reader.Align(2);
                    Assert.Equal(2, reader.Position);
                    reader.Align(4);
                    Assert.Equal(4, reader.Position);
                    reader.Align(8);
                    Assert.Equal(8, reader.Position);
                }
            }
        }

        [Fact]
        public void ReadFloat()
        {
            using (TempFile temp = new TempFile())
            {
                float value = 42.24f;
                byte[] bytes = BitConverter.GetBytes(value);

                File.WriteAllBytes(temp.Path, bytes);
                using (MemoryLoadedFile file = new MemoryLoadedFile(temp.Path))
                {
                    MemoryLoadedFileReader reader = new MemoryLoadedFileReader(file);

                    Assert.Equal(value, reader.ReadFloat());
                    Assert.Equal(0, reader.BytesRemaining);
                }
            }
        }

        [Fact]
        public void ReadDouble()
        {
            using (TempFile temp = new TempFile())
            {
                double value = 42.24;
                byte[] bytes = BitConverter.GetBytes(value);

                File.WriteAllBytes(temp.Path, bytes);
                using (MemoryLoadedFile file = new MemoryLoadedFile(temp.Path))
                {
                    MemoryLoadedFileReader reader = new MemoryLoadedFileReader(file);

                    Assert.Equal(value, reader.ReadDouble());
                    Assert.Equal(0, reader.BytesRemaining);
                }
            }
        }

        [Fact]
        public void ReadBString()
        {
            using (TempFile temp = new TempFile())
            {
                byte[] bytes = new byte[] { 4, 0, (byte)'T', (byte)'e', (byte)'s', (byte)'t' };

                File.WriteAllBytes(temp.Path, bytes);
                using (MemoryLoadedFile file = new MemoryLoadedFile(temp.Path))
                {
                    MemoryLoadedFileReader reader = new MemoryLoadedFileReader(file);

                    Assert.Equal("Test", reader.ReadBString());
                    Assert.Equal(0, reader.BytesRemaining);
                }
            }
        }

        [Fact]
        public unsafe void ReadDecimal()
        {
            using (TempFile temp = new TempFile())
            {
                decimal[] values = new decimal[] { new decimal(42.24) };
                byte[] bytes = new byte[16];

                fixed (decimal* d = values)
                fixed (byte* b = bytes)
                {
                    MemoryBuffer.MemCpy(b, d, 16);
                }

                File.WriteAllBytes(temp.Path, bytes);
                using (MemoryLoadedFile file = new MemoryLoadedFile(temp.Path))
                {
                    MemoryLoadedFileReader reader = new MemoryLoadedFileReader(file);

                    Assert.Equal(values[0], reader.ReadDecimal());
                    Assert.Equal(0, reader.BytesRemaining);
                }
            }
        }
    }
}
