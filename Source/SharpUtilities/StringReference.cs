namespace SharpUtilities
{
    /// <summary>
    /// Represents string that has been read from binary reader.
    /// </summary>
    public struct StringReference
    {
        /// <summary>
        /// List of encodings <see cref="StringReference"/> supports for reading.
        /// </summary>
        public enum Encoding
        {
            /// <summary>
            /// Uses <see cref="System.Text.Encoding.UTF8"/> for parsing string.
            /// </summary>
            UTF8,

            /// <summary>
            /// Uses <see cref="System.Text.Encoding.Unicode"/> for parsing string.
            /// </summary>
            Unicode,
        }

        /// <summary>
        /// Encoding used for reading string.
        /// </summary>
        private Encoding encoding;

        /// <summary>
        /// Cache for read string.
        /// </summary>
        private string text;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryBuffer"/> class.
        /// </summary>
        /// <param name="buffer">Buffer that holds the string.</param>
        /// <param name="encoding">Encoding that will be used for reading the string.</param>
        public StringReference(MemoryBuffer buffer, Encoding encoding)
            : this()
        {
            Buffer = buffer;
            this.encoding = encoding;
            text = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryBuffer"/> class.
        /// </summary>
        /// <param name="text">Cache for read string.</param>
        public StringReference(string text)
            : this()
        {
            this.text = text;
        }

        /// <summary>
        /// Gets the memory buffer that contains the string.
        /// </summary>
        public MemoryBuffer Buffer { get; private set; }

        /// <summary>
        /// Gets the string from the memory buffer.
        /// </summary>
        public string String
        {
            get
            {
                if (text == null)
                    text = ReadString();
                return text;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String;
        }

        /// <summary>
        /// Reads the string from the memory buffer.
        /// </summary>
        /// <returns>Converted string.</returns>
        private unsafe string ReadString()
        {
#if !NET45
            if (Buffer.BytePointer != null)
                return ReadString(Buffer.BytePointer, Buffer.Length, encoding);
#endif
            return ReadString(Buffer.Bytes, encoding);
        }

#if !NET45
        /// <summary>
        /// Converts byte buffer into string using the specified encoding.
        /// </summary>
        /// <param name="buffer">Start of the buffer.</param>
        /// <param name="length">Length of the buffer.</param>
        /// <param name="encoding">Encoding that will be used for conversion.</param>
        /// <returns>Converted string.</returns>
        private static unsafe string ReadString(byte* buffer, int length, Encoding encoding)
        {
            switch (encoding)
            {
                default:
                case Encoding.UTF8:
                    return System.Text.Encoding.UTF8.GetString(buffer, length);
                case Encoding.Unicode:
                    return System.Text.Encoding.Unicode.GetString(buffer, length);
            }
        }
#endif

        /// <summary>
        /// Converts bytes into string using the specified encoding.
        /// </summary>
        /// <param name="bytes">Bytes that hold the string.</param>
        /// <param name="encoding">Encoding that will be used for conversion.</param>
        /// <returns>Converted string.</returns>
        private static string ReadString(byte[] bytes, Encoding encoding)
        {
            if (bytes == null)
                return null;
            switch (encoding)
            {
                default:
                case Encoding.UTF8:
                    return System.Text.Encoding.UTF8.GetString(bytes);
                case Encoding.Unicode:
                    return System.Text.Encoding.Unicode.GetString(bytes);
            }
        }
    }
}
