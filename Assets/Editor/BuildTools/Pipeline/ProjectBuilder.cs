using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System.IO;
using System.Linq;
using Editors.Build.Configuration;
using System.Text;
using Editors.Build.Versioning;

namespace Editors.Build.Automation
{
    public static class ProjectBuilder
    {
        public const string OUTPUT_DIRECTORY = "Builds";
        private const string AAB_EXTENSION = ".aab";
        private const string APK_EXTENSION = ".apk";
        
        private static readonly string LOG_PREFIX = $"[{nameof(ProjectBuilder)}] ";

        public static void ExecuteBuildProcess(BuildProfileSO profile, BuildTarget buildTarget)
        {
            if (profile == null)
            {
                Debug.LogError($"{LOG_PREFIX}Build Profile is null. Build cancelled.");
                return;
            }

            if (!EnsureValidBuildTarget(buildTarget)) return;

            Debug.Log($"{LOG_PREFIX}=== Starting Build Process ===");
            LogProfileDetails(profile, buildTarget);

            BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            string originalDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            bool originalDevelopmentFlag = EditorUserBuildSettings.development;
            ManagedStrippingLevel originalStrippingLevel = PlayerSettings.GetManagedStrippingLevel(targetGroup);
            // Save original StackTrace settings if needed
            // Save original Android/iOS specific settings if needed

            try
            {
                ApplyProfileSettings(profile, buildTarget);

                GenerateBuildPathAndName(profile, buildTarget, out string buildDirectory, out string fileName);
                if (!Directory.Exists(buildDirectory)) Directory.CreateDirectory(buildDirectory);
                string locationPathName = Path.Combine(buildDirectory, fileName);

                BuildPlayerOptions buildPlayerOptions = new()
                {
                    scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(s => s.path).ToArray(),
                    locationPathName = locationPathName,
                    target = buildTarget,
                    options = profile.CurrentBuildOptions
                };

                if (buildPlayerOptions.scenes == null || !buildPlayerOptions.scenes.Any())
                {
                    Debug.LogError($"{LOG_PREFIX}No active scenes found in build settings. Aborting build.");
                    EditorUtility.DisplayDialog("Build Error", "No active scenes found in build settings. Please add and enable scenes in File > Build Settings.", "OK");
                    return;
                }

                Debug.Log($"{LOG_PREFIX}Starting BuildPipeline.BuildPlayer for {buildTarget} at {locationPathName} with options: {profile.CurrentBuildOptions}.");
                BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
                LogBuildReport(report, locationPathName, profile);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"{LOG_PREFIX}Build process EXCEPTION: {ex.Message}");
                Debug.LogException(ex);
            }
            finally
            {
                // Restore original settings after build if needed
                Debug.Log($"{LOG_PREFIX}Reverting temporary project settings...");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, originalDefines);
                EditorUserBuildSettings.development = originalDevelopmentFlag;
                PlayerSettings.SetManagedStrippingLevel(targetGroup, originalStrippingLevel);
                // Restore other saved settings if needed
                AssetDatabase.SaveAssets(); // It's important to save the restored settings
                Debug.Log($"{LOG_PREFIX}Project settings reverted.");
                Debug.Log($"{LOG_PREFIX}=== Build Process Finished ===");
            }
        }

        private static bool EnsureValidBuildTarget(BuildTarget target)
        {
            if (target == BuildTarget.Android || target == BuildTarget.iOS) return true;

            string errorMessage = $"Target build platform '{target}' is not supported by this builder (currently only Android or iOS).";
            Debug.LogError($"{LOG_PREFIX}{errorMessage}");

            EditorUtility.DisplayDialog("Unsupported Build Target", $"{errorMessage}\nPlease select a supported platform.", "OK");
            return false;
        }

        private static void LogProfileDetails(BuildProfileSO profile, BuildTarget targetPlatform)
        {
            StringBuilder logBuilder = new();

            logBuilder.AppendLine($"{LOG_PREFIX}Preparing build with profile: '{profile.name}' for target '{targetPlatform}'");
            logBuilder.AppendLine($"  Profile Type: {profile.ProfileBuildType}");
            logBuilder.AppendLine($"  Output Name Prefix: {profile.BuildNamePrefix}");
            logBuilder.AppendLine($"  Is Development Build: {profile.IsDevelopmentBuild}");
            logBuilder.AppendLine($"  Effective Define Symbols: '{profile.EffectiveDefineSymbols}'");
            logBuilder.AppendLine($"  Build Options: {profile.CurrentBuildOptions}");
            logBuilder.AppendLine($"  Stripping Level: {profile.StrippingLevel}");

            Debug.Log(logBuilder.ToString());
        }

        private static void ApplyProfileSettings(BuildProfileSO profile, BuildTarget targetPlatform)
        {
            BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(targetPlatform);

            Debug.Log($"{LOG_PREFIX}Applying settings from '{profile.name}' for target '{targetPlatform}' (Group: '{targetGroup}')...");

            IncrementBuildVersions(targetPlatform, profile);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, profile.EffectiveDefineSymbols);
            Debug.Log($"{LOG_PREFIX}  Set Define Symbols: '{profile.EffectiveDefineSymbols}'");

            EditorUserBuildSettings.development = profile.IsDevelopmentBuild;
            Debug.Log($"{LOG_PREFIX}  Set Global Development Build Flag: {profile.IsDevelopmentBuild}");

            PlayerSettings.SetManagedStrippingLevel(targetGroup, profile.StrippingLevel);
            Debug.Log($"{LOG_PREFIX}  Set Managed Stripping Level: {profile.StrippingLevel}");

            PlayerSettings.SetStackTraceLogType(LogType.Log, profile.StackTraces.Log);
            PlayerSettings.SetStackTraceLogType(LogType.Warning, profile.StackTraces.Warning);
            PlayerSettings.SetStackTraceLogType(LogType.Error, profile.StackTraces.Error);
            PlayerSettings.SetStackTraceLogType(LogType.Assert, profile.StackTraces.Assert);
            PlayerSettings.SetStackTraceLogType(LogType.Exception, profile.StackTraces.Exception);
            Debug.Log($"{LOG_PREFIX}  Set Stack Traces: Log={profile.StackTraces.Log}, Warn={profile.StackTraces.Warning}, Error={profile.StackTraces.Error}, Assert={profile.StackTraces.Assert}, Ex={profile.StackTraces.Exception}");

            if (targetPlatform == BuildTarget.Android)
            {
                #if UNITY_ANDROID
                PlayerSettings.Android.useCustomKeystore = profile.AndroidSettings.UseCustomKeystore;
                Debug.Log($"{LOG_PREFIX} Set Android Use Custom Keystore: {profile.AndroidSettings.UseCustomKeystore}");

                if (profile.AndroidSettings.UseCustomKeystore)
                {
                    if (string.IsNullOrEmpty(PlayerSettings.Android.keystoreName) || string.IsNullOrEmpty(PlayerSettings.Android.keyaliasName))
                        Debug.LogWarning($"{LOG_PREFIX} Android UseCustomKeystore is true, but Keystore Name or Alias Name is not set in Player Settings! Build might fail or use debug key.");
                }

                EditorUserBuildSettings.buildAppBundle = profile.AndroidSettings.BuildAppBundle;
                Debug.Log($"{LOG_PREFIX} Set Android Build App Bundle (AAB): {profile.AndroidSettings.BuildAppBundle}");

                #else
                Debug.LogWarning($"{LOG_PREFIX} Android build target selected, but UNITY_ANDROID symbol is not defined. Android specific settings might not apply correctly if Android Build Support module is not installed.");
                #endif
            }
            else if (targetPlatform == BuildTarget.iOS)
            {
                #if UNITY_IOS
                if (!string.IsNullOrEmpty(profile.IosSettings.DeveloperTeamID))
                {
                    PlayerSettings.iOS.appleDeveloperTeamID = profile.IosSettings.DeveloperTeamID;
                    Debug.Log($"{LOG_PREFIX}  Set iOS Developer Team ID: {profile.IosSettings.DeveloperTeamID}");
                }
                else Debug.LogWarning($"{LOG_PREFIX}  iOS Developer Team ID is not specified in the build profile. Manual configuration in Xcode might be required.");
                PlayerSettings.iOS.appleEnableAutomaticSigning = string.IsNullOrEmpty(profile.IosSettings.DeveloperTeamID);

                #else
                Debug.LogWarning($"{LOG_PREFIX}  iOS build target selected, but UNITY_IOS symbol is not defined. iOS specific settings might not apply correctly if iOS Build Support module is not installed.");
                #endif
            }
            
            Debug.Log($"{LOG_PREFIX}Project settings applied from '{profile.name}'.");
        }

        private static void IncrementBuildVersions(BuildTarget targetPlatform, BuildProfileSO profile)
        {
            VersionIncrementer.IncrementApplicationVersion(profile.VersionIncrementMode);
            VersionIncrementer.IncrementPlatformBuildCode(targetPlatform);
        }

        private static void GenerateBuildPathAndName(BuildProfileSO profile, BuildTarget targetPlatform, out string buildDirectory, out string fileName)
        {
            string appVersion = PlayerSettings.bundleVersion;
            string buildNumber = "0";
            string fileExtension = "";

            if (targetPlatform == BuildTarget.Android)
            {
                buildNumber = PlayerSettings.Android.bundleVersionCode.ToString();
                fileExtension = profile.AndroidSettings.BuildAppBundle ? AAB_EXTENSION : APK_EXTENSION;
            }
            else if (targetPlatform == BuildTarget.iOS)
            {
                buildNumber = PlayerSettings.iOS.buildNumber;
            }

            fileName = $"{profile.BuildNamePrefix}_{targetPlatform}_{profile.ProfileBuildType}_v{appVersion}_b{buildNumber}{fileExtension}";
            
            buildDirectory = Path.Combine(OUTPUT_DIRECTORY, targetPlatform.ToString(), profile.ProfileBuildType.ToString());

            fileName = string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));

            Debug.Log($"{LOG_PREFIX}Generated build name: {fileName}");
            Debug.Log($"{LOG_PREFIX}Target build directory: {Path.GetFullPath(buildDirectory)}");
        }

        private static void LogBuildReport(BuildReport report, string outputPath, BuildProfileSO profile)
        {
            BuildSummary summary = report.summary;
            string resultString = $"Build '{profile.name}' for {summary.platform} ({profile.ProfileBuildType})";

            if (summary.result == BuildResult.Succeeded)
            {
                float sizeMB = (float)summary.totalSize / (1024 * 1024);
                Debug.Log($"{LOG_PREFIX}BUILD SUCCEEDED: {resultString}. Time: {summary.totalTime.TotalSeconds:F2}s. Size: {sizeMB:F2} MB. Path: {Path.GetFullPath(outputPath)}");
                EditorUtility.RevealInFinder(Path.GetFullPath(Path.GetDirectoryName(outputPath)));
            }
            else
            {
                string errorType = summary.result == BuildResult.Failed ? "FAILED" : summary.result.ToString().ToUpperInvariant();
                Debug.LogError($"{LOG_PREFIX}BUILD {errorType}: {resultString}. Time: {summary.totalTime.TotalSeconds:F2}s. Errors: {summary.totalErrors}. Warnings: {summary.totalWarnings}.");

                if (!string.IsNullOrEmpty(outputPath) && summary.result != BuildResult.Succeeded)
                    Debug.LogWarning($"{LOG_PREFIX}Output was intended for: {Path.GetFullPath(outputPath)}");

                if (summary.totalErrors > 0 && report.steps.Length > 0)
                {
                    StringBuilder stepsLog = new();
                    stepsLog.AppendLine($"{LOG_PREFIX}Detailed Build Steps & Messages:");

                    foreach (var step in report.steps)
                    {
                        if (step.messages.Any(m => m.type == LogType.Error))
                        {
                            stepsLog.AppendLine($"  Step: {step.name}, Duration: {step.duration.TotalSeconds:F2}s");

                            foreach (var msg in step.messages)
                                stepsLog.AppendLine($"    [{msg.type}] {msg.content.Replace("\n", "\n      ")}");
                        }
                    }
                    if (stepsLog.Length > LOG_PREFIX.Length + "Detailed Build Steps & Messages:".Length + 5)
                        Debug.LogError(stepsLog.ToString());
                }
            }
        }
    }
}