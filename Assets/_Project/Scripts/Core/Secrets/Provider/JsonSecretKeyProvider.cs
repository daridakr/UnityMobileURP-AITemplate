using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Core.Secrets
{
    public sealed class JsonSecretKeyProvider :
        ISecretKeyProvider
    {
        private readonly string _rawSecretsFilePath;
        private readonly object _initLock;

        private JObject _secretsJson;
        private Task<bool> _loadingTask;

        private const string LOG_PREFIX = "[Secrets] ";
        private const string EDITOR_DESKTOP_PATH_KEY = "file:///";

        public JsonSecretKeyProvider()
        {
            _rawSecretsFilePath = Path.Combine(Application.streamingAssetsPath, SecretsParams.SECRETS_FILE_NAME);
            _initLock = new object();

            _secretsJson = null;
            _loadingTask = null;
        }

        public async Task<(bool success, string secretValue)> TryGetSecretAsync(string keyName)
        {
            if (string.IsNullOrWhiteSpace(keyName))
            {
                Debug.Log(LOG_PREFIX + "Requested key name cannot be null or whitespace.");
                return (false, null);
            }

            bool loadedSuccessfully = await EnsureSecretsLoadedAsync();

            if (!loadedSuccessfully || _secretsJson == null)
                return (false, null);

            return ExtractSecretFromJObject(keyName);
        }

        private Task<bool> EnsureSecretsLoadedAsync()
        {
            lock (_initLock)
            {
                if (_loadingTask == null)
                {
                    Debug.Log($"{LOG_PREFIX}Initiating asynchronous loading of secrets file...");
                    _loadingTask = LoadSecretsFileAsync();
                }
            }
            return _loadingTask;
        }

        private async Task<bool> LoadSecretsFileAsync()
        {
            if (_secretsJson != null) return true;
            _secretsJson = null;

            bool isSuccessful = false;
            string requestUrl;

            if (Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.LinuxPlayer)
                requestUrl = $"{EDITOR_DESKTOP_PATH_KEY}{_rawSecretsFilePath}";
            else requestUrl = _rawSecretsFilePath;

            Debug.Log($"{LOG_PREFIX}Attempting to load secrets from: {requestUrl}");

            using (UnityWebRequest loadRequest = UnityWebRequest.Get(requestUrl))
            {
                UnityWebRequestAsyncOperation loadingAsyncOperation = loadRequest.SendWebRequest();

                while (!loadingAsyncOperation.isDone) await Task.Yield();

                #if UNITY_2020_1_OR_NEWER
                if (loadRequest.result == UnityWebRequest.Result.Success)
                #else
                if (!loadRequest.isNetworkError && !loadRequest.isHttpError)
                #endif
                {
                    string jsonContent = loadRequest.downloadHandler.text;

                    if (!string.IsNullOrWhiteSpace(jsonContent))
                    {
                        try
                        {
                            _secretsJson = JObject.Parse(jsonContent);
                            Debug.Log($"{LOG_PREFIX}Secrets file loaded and parsed successfully.");
                            isSuccessful = true;
                        }
                        catch (Exception ex) { Debug.LogError($"{LOG_PREFIX}Failed to parse JSON. Error: {ex.Message}"); Debug.LogException(ex); }
                    }
                    else { Debug.LogWarning($"{LOG_PREFIX}Secrets file loaded but is empty."); }
                }
                else { Debug.LogError($"{LOG_PREFIX}Failed to load secrets file. Error: {loadRequest.error}"); }
            }

            return isSuccessful;
        }

        private (bool success, string secretValue) ExtractSecretFromJObject(string keyName)
        {
            if (_secretsJson == null)
            {
                Debug.LogError($"{LOG_PREFIX}ExtractSecret called but secrets are not loaded!");
                return (false, null);
            }

            JToken token = _secretsJson.GetValue(keyName, StringComparison.OrdinalIgnoreCase);

            if (token != null && token.Type == JTokenType.String)
            {
                string value = token.ToString();

                if (!string.IsNullOrEmpty(value) && value != SecretsParams.DEFAULT_API_KEY_PLACEHOLDER)
                    return (true, value);
                else { Debug.LogWarning($"{LOG_PREFIX}Secret '{keyName}' found, but value is empty or placeholder."); }
            }
            else { Debug.LogWarning($"{LOG_PREFIX}Secret key '{keyName}' not found within loaded secrets."); }

            return (false, null);
        }
    }
}