using System.IO;
using System.Text;

namespace Services.Files
{
    public readonly struct CurrentStreamFileData
    {
        private readonly FileMode _openMode;
        private readonly FileAccess _access;
        private readonly FileShare _share;
        private readonly Encoding _encoding;
        private readonly int _bufferSize;

        public FileMode OpenMode => _openMode;
        public FileAccess Access => _access;
        public FileShare Share => _share;
        public Encoding Encoding => _encoding;
        public int BufferSize => _bufferSize;

        private const int DEFAULT_BUFFER_SIZE = 4096;

        public CurrentStreamFileData(
            FileMode openMode,
            FileAccess access,
            FileShare share,
            Encoding encoding,
            int bufferSize)
        {
            _openMode = openMode;
            _access = access;
            _share = share;
            _encoding = encoding ?? Encoding.UTF8;
            _bufferSize = bufferSize;
        }

        public static CurrentStreamFileData Default =>
            new(FileMode.Append, FileAccess.Write, FileShare.Read, null, DEFAULT_BUFFER_SIZE);
    }
}