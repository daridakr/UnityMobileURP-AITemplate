using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Services.Files
{
    // Implements IFileService by opening/closing the file for each write.
    public sealed class TransientStreamFileService : BaseFileService,
        ITransientFileService
    {
        private const string LOG_PREFIX = "[TransientFileService] ";

        protected override async Task WriteInternalAsync(string line, CurrentStreamFileData currentData)
        {
            try
            {
                await using FileStream fileStream = new(
                    FilePath,
                    currentData.OpenMode,
                    currentData.Access,
                    currentData.Share,
                    currentData.BufferSize,
                    useAsync: true);

                await using var writer = new StreamWriter(fileStream, currentData.Encoding);
                await writer.WriteLineAsync(line);
            }
            catch(Exception ex)
            {
                Debug.LogError($"{LOG_PREFIX}Error during transient write to {FilePath}. Error: {ex.Message}");
                Debug.LogException(ex);
                throw;
            }
        }

        protected override Task FlushInternalAsync() => Task.CompletedTask;
    }
}