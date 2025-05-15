using System;
using System.IO;
using System.Threading.Tasks;

namespace Services.Files
{
    // Implements IFileService keeping the StreamWriter open.
    public sealed class PersistentStreamFileService : BaseFileService,
        IPersistentFileService
    {
        private StreamWriter _writer;

        private const string WRITER_IS_NULL_EXCEPTION = "Writer is not initialized.";

        public PersistentStreamFileService()
            : base()
        {
            _writer = null;
        }

        protected override Task InitializeStreamInternalAsync(CurrentStreamFileData data)
        {
            FileStream fileStream = new(FilePath, data.OpenMode, data.Access, data.Share, data.BufferSize, useAsync: true);
            _writer = new StreamWriter(fileStream, data.Encoding);
            // _writer.AutoFlush = true; if needed

            return Task.CompletedTask;
        }

        protected override Task DisposeInternalAsync()
        {
            _writer?.Dispose();
            _writer = null;

            return Task.CompletedTask;
        }

        protected override async Task WriteInternalAsync(string line, CurrentStreamFileData data)
        {
            if (_writer == null) throw new InvalidOperationException(WRITER_IS_NULL_EXCEPTION);
            await _writer.WriteLineAsync(line);
        }

        protected override async Task FlushInternalAsync()
        {
            if (_writer == null) throw new InvalidOperationException(WRITER_IS_NULL_EXCEPTION);
            await _writer.FlushAsync();
        }

        protected override async Task CloseStreamInternalAsync()
        {
            _writer?.Close();
            _writer = null;

            await Task.CompletedTask;
        }
    }
}