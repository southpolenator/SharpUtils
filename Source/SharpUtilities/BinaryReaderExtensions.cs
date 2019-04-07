using System;
using System.Text;

namespace SharpUtilities
{
    /// <summary>
    /// Extension methods for <see cref="IBinaryReader"/>.
    /// </summary>
    public unsafe static class BinaryReaderExtensions
    {
        /// <summary>
        /// Reads <c>Guid</c> from the stream.
        /// </summary>
        public static Guid ReadGuid(this IBinaryReader reader)
        {
            return new Guid(
                reader.ReadInt(),
                reader.ReadShort(),
                reader.ReadShort(),
                reader.ReadByte(),
                reader.ReadByte(),
                reader.ReadByte(),
                reader.ReadByte(),
                reader.ReadByte(),
                reader.ReadByte(),
                reader.ReadByte(),
                reader.ReadByte());
        }

        /// <summary>
        /// Reads all bytes from the stream.
        /// </summary>
        /// <param name="reader">Binary reader.</param>
        public static byte[] ReadAllBytes(this IBinaryReader reader)
        {
            reader.Position = 0;
            return ReadByteArray(reader, (int)reader.Length);
        }

        /// <summary>
        /// Reads <c>byte[]</c> from the stream.
        /// </summary>
        /// <param name="reader">Binary reader.</param>
        /// <param name="length">Number of elements to be read.</param>
        public static byte[] ReadByteArray(this IBinaryReader reader, int length)
        {
            byte[] result = new byte[length];

            fixed (byte* b = result)
            {
                reader.ReadBytes(b, (uint)length);
            }
            return result;
        }

        /// <summary>
        /// Reads <c>ushort[]</c> from the stream.
        /// </summary>
        /// <param name="reader">Binary reader.</param>
        /// <param name="length">Number of elements to be read.</param>
        public static ushort[] ReadUshortArray(this IBinaryReader reader, int length)
        {
            ushort[] result = new ushort[length];

            fixed (ushort* b = result)
            {
                reader.ReadBytes((byte*)b, (uint)length * 2); // 2 = sizeof(ushort)
            }
            return result;
        }

        /// <summary>
        /// Reads <c>uint[]</c> from the stream.
        /// </summary>
        /// <param name="reader">Binary reader.</param>
        /// <param name="length">Number of elements to be read.</param>
        public static uint[] ReadUintArray(this IBinaryReader reader, int length)
        {
            uint[] result = new uint[length];

            fixed (uint* b = result)
            {
                reader.ReadBytes((byte*)b, (uint)length * 4); // 4 = sizeof(uint)
            }
            return result;
        }

        /// <summary>
        /// Creates substream of the specified length from the current position and moves position by the substream length.
        /// </summary>
        /// <typeparam name="TStream">Type of the parent stream.</typeparam>
        /// <param name="reader">Parent binary stream.</param>
        /// <param name="length">Substream length in bytes.</param>
        public static IBinaryReader ReadSubstream<TStream>(this TStream reader, long length = -1)
            where TStream : IBinaryReader
        {
            if (length < 0)
                length = reader.Length - reader.Position;
            IBinaryReader result = new BinarySubstreamReader<TStream>(reader, reader.Position, length);

            reader.Position += length;
            return result;
        }

        /// <summary>
        /// Aligns binary reader to the specified number of alignment bytes.
        /// </summary>
        /// <param name="reader">Stream binary reader.</param>
        /// <param name="alignment">Number of bytes to be aligned</param>
        public static void Align(this IBinaryReader reader, long alignment)
        {
            long unaligned = reader.Position % alignment;

            if (unaligned != 0)
                reader.Move((uint)(alignment - unaligned));
        }

        /// <summary>
        /// Reads 32-bit floating point number from the stream.
        /// </summary>
        /// <param name="reader">Stream binary reader.</param>
        public static unsafe float ReadFloat(this IBinaryReader reader)
        {
            uint data = reader.ReadUint();

            return *((float*)&data);
        }

        /// <summary>
        /// Reads 64-bit floating point number from the stream.
        /// </summary>
        /// <param name="reader">Stream binary reader.</param>
        public static unsafe double ReadDouble(this IBinaryReader reader)
        {
            ulong data = reader.ReadUlong();

            return *((double*)&data);
        }

        /// <summary>
        /// Reads length prefixed UTF8 string from the stream.
        /// </summary>
        /// <param name="reader">Stream binary reader.</param>
        public static StringReference ReadBString(this IBinaryReader reader)
        {
            ushort length = reader.ReadUshort();
            MemoryBuffer buffer = reader.ReadBuffer(length);

            return new StringReference(buffer, StringReference.Encoding.UTF8);
        }

        /// <summary>
        /// Reads decimal number from the stream.
        /// </summary>
        /// <param name="reader">Stream binary reader.</param>
        public static decimal ReadDecimal(this IBinaryReader reader)
        {
            int bits0 = reader.ReadInt();
            int bits1 = reader.ReadInt();
            int bits2 = reader.ReadInt();
            int bits3 = reader.ReadInt();

            return new decimal(bits2, bits3, bits1, bits0 < 0, (byte)((bits0 & 0x00FF0000) >> 16));
        }
    }
}
