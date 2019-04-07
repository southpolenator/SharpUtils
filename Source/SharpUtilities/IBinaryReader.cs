namespace SharpUtilities
{
    /// <summary>
    /// Interface that defines simple binary reader
    /// </summary>
    public interface IBinaryReader
    {
        /// <summary>
        /// Gets or sets the position in the stream.
        /// </summary>
        long Position { get; set; }

        /// <summary>
        /// Gets the length of the stream in bytes.
        /// </summary>
        long Length { get; }

        /// <summary>
        /// Gets the remaining number of bytes in the stream.
        /// </summary>
        long BytesRemaining { get; }

        /// <summary>
        /// Reads <c>byte</c> from the stream.
        /// </summary>
        byte ReadByte();

        /// <summary>
        /// Reads <c>short</c> from the stream.
        /// </summary>
        short ReadShort();

        /// <summary>
        /// Reads <c>ushort</c> from the stream.
        /// </summary>
        ushort ReadUshort();

        /// <summary>
        /// Reads <c>int</c> from the stream.
        /// </summary>
        int ReadInt();

        /// <summary>
        /// Reads <c>uint</c> from the stream.
        /// </summary>
        uint ReadUint();

        /// <summary>
        /// Reads <c>long</c> from the stream.
        /// </summary>
        long ReadLong();

        /// <summary>
        /// Reads <c>ulong</c> from the stream.
        /// </summary>
        ulong ReadUlong();

        /// <summary>
        /// Reads C-style string (null terminated) from the stream.
        /// </summary>
        StringReference ReadCString();

        /// <summary>
        /// Reads C-style wide (2 bytes) string (null terminated) from the stream.
        /// </summary>
        StringReference ReadCStringWide();

        /// <summary>
        /// Moves position by the specified bytes.
        /// </summary>
        /// <param name="bytes">Number of bytes to move the stream.</param>
        void Move(uint bytes);

        /// <summary>
        /// Reads bytes buffer from the stream.
        /// </summary>
        /// <param name="bytes">Buffer pointer where bytes should be stored.</param>
        /// <param name="count">Number of bytes to copy from the stream.</param>
        unsafe void ReadBytes(byte* bytes, uint count);

        /// <summary>
        /// Reads memory buffer from the stream.
        /// </summary>
        /// <param name="length">Number of bytes to read from the stream.</param>
        /// <returns>Memory buffer read from the stream.</returns>
        MemoryBuffer ReadBuffer(uint length);

        /// <summary>
        /// Creates duplicate of this stream.
        /// </summary>
        IBinaryReader Duplicate();
    }
}
