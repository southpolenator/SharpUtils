using System;
using System.IO;

namespace SharpUtilities.Tests
{
    internal class TempFile : IDisposable
    {
        public TempFile()
        {
            Path = System.IO.Path.GetTempFileName();
        }

        public string Path { get; private set; }

        public void Dispose()
        {
            File.Delete(Path);
        }
    }
}
