using System;
using System.Threading.Tasks;

namespace Services.Files
{
    public interface IFileService :
        IDisposable
    {
        public string FilePath { get; }

        public Task InitializeAsync(string filePath, CurrentStreamFileData data);
        public Task WriteLineAsync(string line);
        public Task FlushAsync();
    }
}