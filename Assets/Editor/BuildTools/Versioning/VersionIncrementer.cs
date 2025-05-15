using System;
using System.Collections.Generic;
using Editors.Build.Configuration;
using UnityEditor;
using UnityEngine;

namespace Editors.Build.Versioning
{
    public static class VersionIncrementer
    {
        private static readonly string LOG_PREFIX = $"[{nameof(VersionIncrementer)}] ";

        private static readonly IReadOnlyDictionary<VersionIncrementType, Func<VersionParts, VersionParts>>
            _appVersionIncrementHandlers = new Dictionary<VersionIncrementType, Func<VersionParts, VersionParts>>()
            {
                [VersionIncrementType.None] = version => version,
                [VersionIncrementType.Patch] = version => version.IncrementPatch(),
                [VersionIncrementType.Minor] = version => version.IncrementMinor(),
                [VersionIncrementType.Major] = version => version.IncrementMajor()
            };

        private static readonly IReadOnlyDictionary<BuildTarget, Action>
            _platformBuildCodeIncrementActions = new Dictionary<BuildTarget, Action>
            {
                [BuildTarget.Android] = () => IncrementAndroidBuildCode(),
                [BuildTarget.iOS] = () => IncrementIosIncrementBuildCode(),
            };

        public static void IncrementApplicationVersion(VersionIncrementType incrementMode)
        {
            if (incrementMode == VersionIncrementType.None)
            {
                Debug.Log($"{LOG_PREFIX}Application version increment skipped (Mode: None). Current: {PlayerSettings.bundleVersion}");
                return;
            }

            if (Version.TryParse(PlayerSettings.bundleVersion, out Version currentSystemVersion))
            {
                VersionParts currentParts = new(currentSystemVersion);

                if (_appVersionIncrementHandlers.TryGetValue(incrementMode, out var handler))
                {
                    VersionParts newParts = handler(currentParts);
                    string newVersionString = newParts.ToString();

                    if (PlayerSettings.bundleVersion != newVersionString)
                    {
                        PlayerSettings.bundleVersion = newVersionString;
                        Debug.Log($"{LOG_PREFIX}Incremented Application Version to: {newVersionString} (Mode: {incrementMode})");
                    }
                    else Debug.Log($"{LOG_PREFIX}Application Version remains {newVersionString} (Mode: {incrementMode}, no change after handler).");
                }
                else Debug.LogWarning($"{LOG_PREFIX}Unknown VersionIncrementType: {incrementMode}. Application version not changed.");
            }
            else Debug.LogWarning($"{LOG_PREFIX}Could not parse current bundleVersion: '{PlayerSettings.bundleVersion}'. Version not incremented. Consider setting an initial valid version (e.g., 0.1.0).");
        }

        public static void IncrementPlatformBuildCode(BuildTarget targetPlatform)
        {
            if (_platformBuildCodeIncrementActions.TryGetValue(targetPlatform, out Action incrementAction))
                incrementAction();
            else
                Debug.LogWarning($"{LOG_PREFIX}No build code increment logic defined for platform: {targetPlatform}. Build code not changed.");
        }

        private static void IncrementAndroidBuildCode()
        {
            PlayerSettings.Android.bundleVersionCode++;
            Debug.Log($"{LOG_PREFIX}Incremented Android Bundle Version Code to: {PlayerSettings.Android.bundleVersionCode}");
        }

        private static void IncrementIosIncrementBuildCode()
        {
            if (int.TryParse(PlayerSettings.iOS.buildNumber, out int currentIOSBuildNumber))
                PlayerSettings.iOS.buildNumber = (currentIOSBuildNumber + 1).ToString();
            else
            {
                PlayerSettings.iOS.buildNumber = "1";
                Debug.LogWarning($"{LOG_PREFIX}iOS build number was not a valid integer. Resetting to 1.");
            }

            Debug.Log($"{LOG_PREFIX}Incremented iOS Build Number to: {PlayerSettings.iOS.buildNumber}");
        }
    }
}