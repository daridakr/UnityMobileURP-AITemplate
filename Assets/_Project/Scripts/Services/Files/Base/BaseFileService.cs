using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Services.Files
{
    public abstract class BaseFileService :
        IFileService
    {
        private readonly SemaphoreSlim _semaphore;

        private string _pathInternal;
        private CurrentStreamFileData _initializedData;
        private bool _isInitialized;
        private bool _isDisposed;

        public string FilePath => _pathInternal;

        private const string LOG_PREFIX = "[BaseFileService] ";

        public BaseFileService() => _semaphore = new SemaphoreSlim(1, 1);

        public async Task InitializeAsync(string filePath, CurrentStreamFileData data)
        {
            if (_isInitialized) { Debug.LogWarning($"{LOG_PREFIX}Already initialized for path: {_pathInternal}"); return; }

            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath), "File path cannot be null or empty.");
            if (data.Access != FileAccess.Write && data.Access != FileAccess.ReadWrite)
                throw new ArgumentException("FileAccess must include Write.", nameof(data.Access));

            _pathInternal = filePath;
            _initializedData = data;

            await _semaphore.WaitAsync();
            try
            {
                if (_isInitialized) return;

                string directoryPath = Path.GetDirectoryName(_pathInternal);
                if (!string.IsNullOrEmpty(directoryPath)) Directory.CreateDirectory(directoryPath);
                
                await InitializeStreamInternalAsync(_initializedData);

                _isInitialized = true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"{LOG_PREFIX}FAILED to initialize for path '{_pathInternal}'. Error: {ex.Message}");
                Debug.LogException(ex);

                await DisposeInternalAsync();
                throw;
            }
            finally { _semaphore.Release(); }
        }

        protected virtual Task InitializeStreamInternalAsync(CurrentStreamFileData data) => Task.CompletedTask;
        protected virtual Task DisposeInternalAsync() => Task.CompletedTask;

        public async Task WriteLineAsync(string line)
        {
            if (!_isInitialized || _isDisposed)
                { Debug.LogError($"{LOG_PREFIX}Cannot write line. Not initialized or disposed. Path: {_pathInternal}"); return; }

            if (line == null) return;

            await _semaphore.WaitAsync();
            try
            {
                if (!_isInitialized || _isDisposed) return;

                await WriteInternalAsync(line, _initializedData);
            }
            catch (Exception ex) { Debug.LogError($"{LOG_PREFIX}Failed to write line to '{_pathInternal}'. Error: {ex.Message}"); Debug.LogException(ex); }
            finally { _semaphore.Release(); }
        }

        public async Task FlushAsync()
        {
            if (!_isInitialized || _isDisposed) return;
            
            await _semaphore.WaitAsync();
            try
            {
                if (!_isInitialized || _isDisposed) return;

                await FlushInternalAsync();
            }
            catch (Exception ex) { Debug.LogError($"{LOG_PREFIX}Failed to flush '{_pathInternal}'. Error: {ex.Message}"); }
            finally { _semaphore.Release(); }
        }

        protected abstract Task WriteInternalAsync(string line, CurrentStreamFileData currentData);
        protected abstract Task FlushInternalAsync();

        public void Dispose()
        {
            OnDisposed();
            GC.SuppressFinalize(this);
        }

        // Focus on release of resources without the risk of blocking the flow.
        protected virtual async void OnDisposed()
        {
            try
            {
                await CloseStreamInternalAsync();
            }
            catch (Exception ex) { Debug.LogError($"{LOG_PREFIX}Error during stream close: {ex.Message}"); }
            finally
            {
                _isInitialized = false;
                _semaphore?.Dispose();
            }

            _isDisposed = true;

            GC.SuppressFinalize(this);
        }

        protected virtual Task CloseStreamInternalAsync() => Task.CompletedTask;
    }
}