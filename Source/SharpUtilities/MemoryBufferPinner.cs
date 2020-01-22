using System.Runtime.InteropServices;

namespace SharpUtilities
{
    /// <summary>
    /// Simple disposable wrapper that allows unsafe access to <see cref="MemoryBuffer"/> on stack.
    /// </summary>
    public unsafe ref struct MemoryBufferPinner
    {
        /// <summary>
        /// GC handle to pinned array of memory buffer if it was created from managed byte array.
        /// </summary>
        private GCHandle? pinnedArrayHandle;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryBufferPinner"/> class.
        /// </summary>
        /// <param name="buffer">Memory buffer that will be accessed in unsafe code.</param>
        public MemoryBufferPinner(MemoryBuffer buffer)
        {
            Buffer = buffer;
            BytePointer = buffer.BytePointer;
            if (BytePointer == null)
            {
                pinnedArrayHandle = GCHandle.Alloc(buffer.Bytes, GCHandleType.Pinned);
                BytePointer = (byte*)pinnedArrayHandle.Value.AddrOfPinnedObject().ToPointer();
            }
            else
                pinnedArrayHandle = null;
        }

        /// <summary>
        /// Gets the byte pointer.
        /// </summary>
        public byte* BytePointer { get; private set; }

        /// <summary>
        /// Gets the length of memory buffer.
        /// </summary>
        public int Length => Buffer.Length;

        /// <summary>
        /// Gets the memory buffer that provided byte pointer.
        /// </summary>
        /// <value></value>
        public MemoryBuffer Buffer { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            pinnedArrayHandle?.Free();
        }
    }
}
