using System.Text;

namespace SharpUtilities
{
    /// <summary>
    /// Class that implements <see cref="IBinaryReader"/> for memory (pointer and length).
    /// </summary>
    public unsafe class MemoryBinaryReader : IBinaryReader
    {
        /// <summary>
        /// Pointer to the start of memory buffer.
        /// </summary>
        private readonly byte* basePointer;

        /// <summary>
        /// Current position in the memory buffer.
        /// </summary>
        private byte* pointer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryBinaryReader"/> class.
        /// </summary>
        public MemoryBinaryReader(byte* memory, long length)
        {
            pointer = basePointer = memory;
            Length = length;
        }

        /// <summary>
        /// Gets the length of the stream in bytes.
        /// </summary>
        public long Length { get; private set; }

        /// <summary>
        /// Gets or sets the position in the stream.
        /// </summary>
        public long Position
        {
            get => pointer - basePointer;
            set => pointer = basePointer + value;
        }

        /// <summary>
        /// Gets the remaining number of bytes in the stream.
        /// </summary>
        public long BytesRemaining => Length - Position;

        /// <summary>
        /// Creates duplicate of this stream.
        /// </summary>
        public virtual IBinaryReader Duplicate()
        {
            return new MemoryBinaryReader(basePointer, Length)
            {
                Position = Position,
            };
        }

        /// <summary>
        /// Reads <c>byte</c> from the stream.
        /// </summary>
        public byte ReadByte()
        {
            byte value = *pointer;

            pointer++;
            return value;
        }

        /// <summary>
        /// Reads <c>short</c> from the stream.
        /// </summary>
        public short ReadShort()
        {
            short value = *((short*)pointer);

            pointer += 2;
            return value;
        }

        /// <summary>
        /// Reads <c>ushort</c> from the stream.
        /// </summary>
        public ushort ReadUshort()
        {
            ushort value = *((ushort*)pointer);

            pointer += 2;
            return value;
        }

        /// <summary>
        /// Reads <c>int</c> from the stream.
        /// </summary>
        public int ReadInt()
        {
            int value = *((int*)pointer);

            pointer += 4;
            return value;
        }

        /// <summary>
        /// Reads <c>uint</c> from the stream.
        /// </summary>
        public uint ReadUint()
        {
            uint value = *((uint*)pointer);

            pointer += 4;
            return value;
        }

        /// <summary>
        /// Reads <c>long</c> from the stream.
        /// </summary>
        public long ReadLong()
        {
            long value = *((long*)pointer);

            pointer += 8;
            return value;
        }

        /// <summary>
        /// Reads <c>ulong</c> from the stream.
        /// </summary>
        public ulong ReadUlong()
        {
            ulong value = *((ulong*)pointer);

            pointer += 8;
            return value;
        }

        /// <summary>
        /// Reads C-style string (null terminated) from the stream.
        /// </summary>
        public string ReadCString()
        {
            byte* start = pointer;
            byte* end = start;

            while (*end != 0)
                end++;
#if NET45
            byte[] bytes = this.ReadByteArray((int)(end - start));

            pointer++;
            return Encoding.UTF8.GetString(bytes);
#else
            pointer = end + 1;
            return Encoding.UTF8.GetString(start, (int)(end - start));
#endif
        }

        /// <summary>
        /// Reads C-style wide (2 bytes) string (null terminated) from the stream.
        /// </summary>
        public string ReadCStringWide()
        {
            ushort* start = (ushort*)pointer;
            ushort* end = start;

            while (*end != 0)
                end++;
#if NET45
            byte[] bytes = this.ReadByteArray((int)((byte*)end - (byte*)start));

            pointer += 2;
            return Encoding.Unicode.GetString(bytes);
#else
            pointer = (byte*)(end + 1);
            return Encoding.Unicode.GetString((byte*)start, (int)((byte*)end - (byte*)start));
#endif
        }

        /// <summary>
        /// Reads bytes buffer from the stream.
        /// </summary>
        /// <param name="bytes">Buffer pointer where bytes should be stored.</param>
        /// <param name="count">Number of bytes to from the stream</param>
        public void ReadBytes(byte* bytes, uint count)
        {
            MemoryBuffer.MemCpy(bytes, pointer, count);
            pointer += count;
        }

        /// <summary>
        /// Moves position by the specified bytes.
        /// </summary>
        /// <param name="bytes">Number of bytes to move the stream.</param>
        public void Move(uint bytes)
        {
            pointer += bytes;
        }
    }
}
