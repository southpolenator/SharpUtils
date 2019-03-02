namespace SharpUtilities
{
    /// <summary>
    /// Class that implements <see cref="IBinaryReader"/> for <see cref="MemoryLoadedFile"/> as stream.
    /// </summary>
    public unsafe class MemoryLoadedFileReader : MemoryBinaryReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryLoadedFileReader"/> class.
        /// </summary>
        /// <param name="file">The <see cref="MemoryLoadedFile"/> as stream.</param>
        public MemoryLoadedFileReader(MemoryLoadedFile file)
            : base(file.BasePointer, file.Length)
        {
            File = file;
        }

        /// <summary>
        /// Gets the <see cref="MemoryLoadedFile"/>.
        /// </summary>
        public MemoryLoadedFile File { get; private set; }

        /// <summary>
        /// Creates duplicate of this stream.
        /// </summary>
        public override IBinaryReader Duplicate()
        {
            return new MemoryLoadedFileReader(File)
            {
                Position = Position,
            };
        }
    }
}
