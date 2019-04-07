namespace SharpUtilities
{
    /// <summary>
    /// Represents substream <see cref="IBinaryReader"/>.
    /// </summary>
    /// <typeparam name="TStream">Type of the substream.</typeparam>
    public class BinarySubstreamReader<TStream> : IBinaryReader
        where TStream : IBinaryReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BinarySubstreamReader{TStream}"/> class.
        /// </summary>
        /// <param name="parentStream">Parent stream binary reader.</param>
        /// <param name="parentOffset">Position in the parent stream that is the beginning of this stream.</param>
        /// <param name="length">Length of this stream.</param>
        public BinarySubstreamReader(TStream parentStream, long parentOffset, long length)
        {
            ParentOffset = parentOffset;
            ParentStream = (TStream)parentStream.Duplicate();
            Length = length;
        }

        /// <summary>
        /// Gets the position in the parent stream that is the beginning of this stream.
        /// </summary>
        public long ParentOffset { get; private set; }

        /// <summary>
        /// Gets the parent stream binary reader.
        /// </summary>
        public TStream ParentStream { get; private set; }

        /// <summary>
        /// Gets or sets the position in the stream.
        /// </summary>
        public long Position { get => ParentStream.Position - ParentOffset; set => ParentStream.Position = value + ParentOffset; }

        /// <summary>
        /// Gets the length of the stream in bytes.
        /// </summary>
        public long Length { get; private set; }

        /// <summary>
        /// Gets the remaining number of bytes in the stream.
        /// </summary>
        public long BytesRemaining => Length - Position;

        /// <summary>
        /// Creates duplicate of this stream.
        /// </summary>
        public IBinaryReader Duplicate()
        {
            return new BinarySubstreamReader<TStream>(ParentStream, ParentOffset, Length);
        }

        /// <summary>
        /// Reads <c>byte</c> from the stream.
        /// </summary>
        public byte ReadByte()
        {
            return ParentStream.ReadByte();
        }

        /// <summary>
        /// Reads <c>short</c> from the stream.
        /// </summary>
        public short ReadShort()
        {
            return ParentStream.ReadShort();
        }

        /// <summary>
        /// Reads <c>ushort</c> from the stream.
        /// </summary>
        public ushort ReadUshort()
        {
            return ParentStream.ReadUshort();
        }

        /// <summary>
        /// Reads <c>int</c> from the stream.
        /// </summary>
        public int ReadInt()
        {
            return ParentStream.ReadInt();
        }

        /// <summary>
        /// Reads <c>uint</c> from the stream.
        /// </summary>
        public uint ReadUint()
        {
            return ParentStream.ReadUint();
        }

        /// <summary>
        /// Reads <c>long</c> from the stream.
        /// </summary>
        public long ReadLong()
        {
            return ParentStream.ReadLong();
        }

        /// <summary>
        /// Reads <c>ulong</c> from the stream.
        /// </summary>
        public ulong ReadUlong()
        {
            return ParentStream.ReadUlong();
        }

        /// <summary>
        /// Reads C-style string (null terminated) from the stream.
        /// </summary>
        public StringReference ReadCString()
        {
            return ParentStream.ReadCString();
        }

        /// <summary>
        /// Reads C-style wide (2 bytes) string (null terminated) from the stream.
        /// </summary>
        public StringReference ReadCStringWide()
        {
            return ParentStream.ReadCStringWide();
        }

        /// <summary>
        /// Reads bytes buffer from the stream.
        /// </summary>
        /// <param name="bytes">Buffer pointer where bytes should be stored.</param>
        /// <param name="count">Number of bytes to from the stream</param>
        public unsafe void ReadBytes(byte* bytes, uint count)
        {
            ParentStream.ReadBytes(bytes, count);
        }

        /// <summary>
        /// Moves position by the specified bytes.
        /// </summary>
        /// <param name="bytes">Number of bytes to move the stream.</param>
        public void Move(uint bytes)
        {
            ParentStream.Move(bytes);
        }

        /// <summary>
        /// Reads memory buffer from the stream.
        /// </summary>
        /// <param name="length">Number of bytes to read from the stream.</param>
        /// <returns>Memory buffer read from the stream.</returns>
        public MemoryBuffer ReadBuffer(uint length)
        {
            return ParentStream.ReadBuffer(length);
        }
    }
}
