#if TEST || ENABLE_QA_LOGGING
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Services.Files;
using UnityEngine;
using Utilities.Constants;

namespace Test.Logging
{
    public sealed class FileQALogger :
        IQALogger
    {
        private readonly IPersistentFileService _fileService;
        private bool _isInitialized = false;
       
        private const string LOG_FILE_NAME = "live_farm_qa_log.txt";
        private const string TIMESTAMP_FORMAT = "yyyy-MM-dd HH:mm:ss.fff"; // ms
        private const string SEPARATOR = "--------------------";
        private const string LOG_PREFIX = "[FileQALogger] ";
        private const int INITIAL_LOG_ENTRY_CAPACITY = 512; // You can reduce to 256 or increase to 1024 if required

        public FileQALogger(IPersistentFileService fileService) =>
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));

        public async Task InitializeAsync()
        {
            if (_isInitialized) return;

            try
            {
                string logFilePath = Path.Combine(Application.persistentDataPath, LOG_FILE_NAME);
                await _fileService.InitializeAsync(logFilePath, CurrentStreamFileData.Default);

                await _fileService.WriteLineAsync($"--- Q/A Log Session Started: {DateTime.Now.ToString(TIMESTAMP_FORMAT)} ---");
                await _fileService.FlushAsync();
                
                _isInitialized = true;
                Debug.Log($"{LOG_PREFIX}Initialized. Logging to: {_fileService.FilePath}");
            }
            catch(Exception ex)
            {
                Debug.LogError($"{LOG_PREFIX}Failed to initialize. Error: {ex.Message}");
                Debug.LogException(ex);

                _isInitialized = false;

                throw;
            }
        }

        public async Task LogQaAsync(
            string prompt, string responseText, bool isSuccess,
            long? overallLatencyMs = null, string errorMessage = null)
        {
            #if !(TEST || ENABLE_QA_LOGGING)
                await Task.CompletedTask; return;
            #endif

            if (!_isInitialized) { Debug.LogError($"{LOG_PREFIX}Cannot log, logger was not initialized successfully."); return; }

            string timestamp = DateTime.Now.ToString(TIMESTAMP_FORMAT);
            string status = isSuccess ? "Success" : "Failure";
            string responseOrError = isSuccess ? responseText : errorMessage;
            string latencyInfo = overallLatencyMs.HasValue
                ? string.Format(UIMessages.Formatting.LatencyFormat, overallLatencyMs.Value)
                : UIMessages.Formatting.TimingNotAvailable;

            var logBuilder = new StringBuilder(INITIAL_LOG_ENTRY_CAPACITY);
            logBuilder.Append('[').Append(timestamp).Append("] Q: ").AppendLine(prompt?.Trim() ?? "NULL_PROMPT");
            logBuilder.Append('[').Append(timestamp).Append("] A: ").Append(responseOrError?.Trim() ?? "NULL_RESPONSE");
            logBuilder.Append(" (").Append(status).Append(" | ").Append(latencyInfo).AppendLine(")");
            logBuilder.AppendLine(SEPARATOR);

            string logEntry = logBuilder.ToString();

            try
            {
                await _fileService.WriteLineAsync(logEntry);
                await _fileService.FlushAsync();
            }
            catch (Exception ex)
            {
                Debug.LogError($"{LOG_PREFIX}Error occurred while writing Q/A via FileService. Error: {ex.Message}");
                Debug.LogException(ex);
            }
        }
        public void Dispose() => (_fileService as IDisposable)?.Dispose();
    }
}
#endif