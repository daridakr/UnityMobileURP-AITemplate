#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Linq;
using Core.Secrets;
using System.Collections.Generic;
using System;

namespace Editors.Project
{
    [InitializeOnLoad]
    public static class SecretsFileAutoGenerator
    {
        private static readonly string SecretsFilePath;
        private static readonly string StreamingAssetsPath;

        private const string LOG_PREFIX = "[Secrets Generator] ";

        static SecretsFileAutoGenerator()
        {
            StreamingAssetsPath = Application.streamingAssetsPath;
            SecretsFilePath = Path.Combine(StreamingAssetsPath, SecretsParams.SECRETS_FILE_NAME);

            EditorApplication.delayCall += CheckAndGenerateSecretsFile; // You don't need to unsubscribe here.
        }

        private static void CheckAndGenerateSecretsFile()
        {
            bool fileModified = false;

            try
            {
                EnsureStreamingAssetsDirectory();

                JObject secretsJson;
                bool fileExisted = File.Exists(SecretsFilePath);

                if (!fileExisted || IsFileEmpty(SecretsFilePath))
                {
                    if (fileExisted)
                        Debug.LogWarning(LOG_PREFIX + $"Secrets file exists but is empty. Overwriting with placeholder...");

                    secretsJson = CreateDefaultSecretsJson();
                    WriteSecretsFile(secretsJson);
                    fileModified = true;
                    Debug.Log(LOG_PREFIX + $"Placeholder secrets file created/overwritten at {SecretsFilePath}. Please edit it.");
                }
                else
                {
                    if (TryReadSecretsJson(out secretsJson))
                    {
                        if (AddMissingKeys(secretsJson))
                        {
                            WriteSecretsFile(secretsJson);
                            fileModified = true;
                            Debug.Log(LOG_PREFIX + $"Secrets file updated with missing default keys.");
                        }
                        else { Debug.Log(LOG_PREFIX + "Secrets file is up-to-date."); }
                    }
                }

                if (fileModified) AssetDatabase.Refresh();
            }
            catch (Exception ex)
            {
                Debug.LogError(LOG_PREFIX + $"An unexpected error occurred: {ex.Message}");
                Debug.LogException(ex);
            }
        }

        private static void EnsureStreamingAssetsDirectory()
        {
            if (!Directory.Exists(StreamingAssetsPath))
            {
                Directory.CreateDirectory(StreamingAssetsPath);
                Debug.Log(LOG_PREFIX + $"Created StreamingAssets directory at: {StreamingAssetsPath}");
                AssetDatabase.Refresh(); // Need to refresh for Unity to see the folder.
            }
        }

        private static bool IsFileEmpty(string path)
        {
            try
            {
                return string.IsNullOrWhiteSpace(File.ReadAllText(path));
            }
            catch (Exception ex)
            {
                Debug.LogWarning(LOG_PREFIX + $"Could not read secrets file at '{path}' to check if empty: {ex.Message}");
                return false;
            }
        }

        private static JObject CreateDefaultSecretsJson() => JObject.FromObject(new SecretsData());

        private static void WriteSecretsFile(JObject secretsJson)
        {
            try
            {
                string updatedJsonContent = JsonConvert.SerializeObject(secretsJson, Formatting.Indented);
                File.WriteAllText(SecretsFilePath, updatedJsonContent);
            }
            catch (Exception ex)
            {
                Debug.LogError($"{LOG_PREFIX}Failed to write updated secrets file at '{SecretsFilePath}': {ex.Message}");
                Debug.LogException(ex);
            }
        }

        private static bool TryReadSecretsJson(out JObject secretsJson)
        {
            secretsJson = null;

            try
            {
                string currentJsonContent = File.ReadAllText(SecretsFilePath);
                secretsJson = JObject.Parse(currentJsonContent);
                return true;
            }
            catch (JsonReaderException jsonEx)
            {
                Debug.LogError($"{LOG_PREFIX}Failed to parse secrets file at '{SecretsFilePath}'. Please check JSON validity. Error: {jsonEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Debug.LogError($"{LOG_PREFIX}Failed to read secrets file at '{SecretsFilePath}': {ex.Message}");
                Debug.LogException(ex);
                return false;
            }
        }

        private static bool AddMissingKeys(JObject existingJson)
        {
            bool keysAdded = false;

            IEnumerable<PropertyInfo> expectedProperties = typeof(SecretsData)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(prop => prop.PropertyType == typeof(string) && prop.CanRead && prop.CanWrite);

            foreach (PropertyInfo propInfo in expectedProperties)
            {
                // Assume the property name is the same as the JSON key
                string expectedJsonKey = propInfo.Name;
                // You can add logic for mapping C# names to JSON key names if they are different,
                // for example via [JsonProperty] attributes on properties in SecretsData.

                if (!existingJson.ContainsKey(expectedJsonKey))
                {
                    existingJson.Add(expectedJsonKey, SecretsParams.DEFAULT_API_KEY_PLACEHOLDER);
                    keysAdded = true;
                    Debug.Log(LOG_PREFIX + $"Added missing key '{expectedJsonKey}' to secrets file.");
                }
            }

            return keysAdded;
        }
    }
}
#endif