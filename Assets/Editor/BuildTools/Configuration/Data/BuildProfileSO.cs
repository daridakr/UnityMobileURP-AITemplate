using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace Editors.Build.Configuration
{
    [CreateAssetMenu(fileName = "New" + nameof(BuildProfileSO), menuName = "Build Tools/Build Profile", order = 0)]
    public class BuildProfileSO : ScriptableObject
    {        
        [Header("General Settings")]
        [Tooltip("Affects default settings and output naming.")]
        [SerializeField] private BuildType _buildType = BuildType.Debug;

        [Tooltip("Scripting Define Symbols for this build type.")]
        [SerializeField] private ScriptDefineSymbols _defineSymbols = ScriptDefineSymbols.NONE;

        [Tooltip("Enables profiling, debug console, etc.")]
        [SerializeField] private bool _isDevelopmentBuild = true;

        [Tooltip("Additional build options for the Unity BuildPipeline.")]
        [SerializeField] private BuildOptions _buildOptions = BuildOptions.Development | BuildOptions.AllowDebugging;

        [Header("Versioning")]
        [Tooltip("How to increment the PlayerSettings.bundleVersion for this build.")]
        [SerializeField] private VersionIncrementType _versionIncrementMode = VersionIncrementType.Patch;

        [Header("Code Stripping")]
        [Tooltip("Managed code stripping level to reduce build size.")]
        [SerializeField] private ManagedStrippingLevel _strippingLevel = ManagedStrippingLevel.Low;

        [Header("Logging Stack Traces")]
        [SerializeField] private StackTraceSettings _stackTraces = StackTraceSettings.DebugDefault;

        [Header("Android Specific Settings")]
        [Tooltip("These settings are applied only if the current build target is Android.")]
        [SerializeField] private AndroidBuildSettings _androidSettings = AndroidBuildSettings.DebugDefault;

        [Header("iOS Specific Settings")]
        [Tooltip("These settings are applied only if the current build target is iOS.")]
        [SerializeField] private IOSBuildSettings _iosSettings = IOSBuildSettings.Default;

        private static readonly string LOG_PREFIX = $"[{nameof(BuildProfileSO)}] ";
        
        public BuildType ProfileBuildType => _buildType;
        public string BuildNamePrefix => PlayerSettings.productName;
        public bool IsDevelopmentBuild => _isDevelopmentBuild;
        public BuildOptions CurrentBuildOptions => _buildOptions;
        public VersionIncrementType VersionIncrementMode => _versionIncrementMode;
        public ManagedStrippingLevel StrippingLevel => _strippingLevel;
        public StackTraceSettings StackTraces => _stackTraces;
        public AndroidBuildSettings AndroidSettings => _androidSettings;
        public IOSBuildSettings IosSettings => _iosSettings;

        public string EffectiveDefineSymbols
        {
            get
            {
                List<string> symbols = new();

                if (_defineSymbols != ScriptDefineSymbols.NONE)
                {
                    foreach (ScriptDefineSymbols flag in System.Enum.GetValues(typeof(ScriptDefineSymbols)))
                    {
                        if (flag == ScriptDefineSymbols.NONE) continue;

                        if ((_defineSymbols & flag) == flag) symbols.Add(flag.ToString());
                    }
                }

                return string.Join(";", symbols.Distinct());
            }
        }

        private void OnValidate()
        {
            if (Application.isPlaying) return;

            if (_buildType == BuildType.Release && string.IsNullOrEmpty(_iosSettings.DeveloperTeamID))
                Debug.LogWarning($"{LOG_PREFIX}Profile '{name}': iOS Release build should have a Developer Team ID specified.", this);

            if (_isDevelopmentBuild)
            {
                if ((_buildOptions & BuildOptions.Development) == 0)
                {
                    Debug.LogWarning($"{LOG_PREFIX}Profile '{name}': IsDevelopmentBuild is true, but BuildOptions.Development is not set. Consider enabling it.", this);
                    // _buildOptions |= BuildOptions.Development;
                }
                if ((_buildOptions & BuildOptions.AllowDebugging) == 0 && ProfileBuildType == BuildType.Debug)
                {
                    Debug.LogWarning($"{LOG_PREFIX}Profile '{name}': Debug build type, but BuildOptions.AllowDebugging is not set. Consider enabling it.", this);
                }
                if (ProfileBuildType != BuildType.Release && (_strippingLevel == ManagedStrippingLevel.High || _strippingLevel == ManagedStrippingLevel.Medium))
                {
                    Debug.LogWarning($"{LOG_PREFIX}Profile '{name}': Development/QA build with Medium/High stripping. This might slow down iteration or hide debug info. Consider Minimal or Low.", this);
                }
            }
            else
            {
                if ((_buildOptions & BuildOptions.Development) != 0)
                {
                    Debug.LogWarning($"{LOG_PREFIX}Profile '{name}': IsDevelopmentBuild is false, but BuildOptions.Development is set. This is unusual for a release profile.", this);
                }
                if ((_buildOptions & BuildOptions.AllowDebugging) != 0)
                {
                    Debug.LogWarning($"{LOG_PREFIX}Profile '{name}': IsDevelopmentBuild is false, but BuildOptions.AllowDebugging is set. Ensure this is intended for this release profile.", this);
                }
                if (_strippingLevel == ManagedStrippingLevel.Disabled || _strippingLevel == ManagedStrippingLevel.Minimal)
                {
                    Debug.LogWarning($"{LOG_PREFIX}Profile '{name}': Release build with Disabled/Minimal stripping. Consider Medium or High for smaller build size.", this);
                }
            }
        }

        // Presets.

        public void ApplyPreset_FastDebug()
        {
            Undo.RecordObject(this, nameof(ApplyPreset_FastDebug)); // Ctrl+Z
            _buildType = BuildType.Debug;
            _isDevelopmentBuild = true;
            _buildOptions = BuildOptions.Development | BuildOptions.AllowDebugging | BuildOptions.ConnectWithProfiler | BuildOptions.DetailedBuildReport;
            _versionIncrementMode = VersionIncrementType.None;
            _strippingLevel = ManagedStrippingLevel.Minimal;
            _defineSymbols = ScriptDefineSymbols.DEVELOPMENT_BUILD | ScriptDefineSymbols.ENABLE_PROFILER | ScriptDefineSymbols.ENABLE_QA_LOGGING | ScriptDefineSymbols.TEST;
            _stackTraces = StackTraceSettings.DebugDefault;
            _androidSettings = AndroidBuildSettings.DebugDefault;
            _iosSettings = IOSBuildSettings.Default;
            EditorUtility.SetDirty(this);
            Debug.Log($"{LOG_PREFIX}Applied 'Fast Debug' preset to '{name}'.");
        }

        public void ApplyPreset_DefaultDebug()
        {
            Undo.RecordObject(this, nameof(ApplyPreset_DefaultDebug));
            _buildType = BuildType.Debug;
            _isDevelopmentBuild = true;
            _buildOptions = BuildOptions.Development | BuildOptions.AllowDebugging | BuildOptions.ConnectWithProfiler;
            _versionIncrementMode = VersionIncrementType.Patch;
            _strippingLevel = ManagedStrippingLevel.Low;
            _defineSymbols = ScriptDefineSymbols.DEVELOPMENT_BUILD | ScriptDefineSymbols.ENABLE_QA_LOGGING | ScriptDefineSymbols.TEST;
            _stackTraces = StackTraceSettings.DebugDefault;
            _androidSettings = AndroidBuildSettings.DebugDefault;
            _iosSettings = IOSBuildSettings.Default;
            EditorUtility.SetDirty(this);
            Debug.Log($"{LOG_PREFIX}Applied 'Default Debug' preset to '{name}'.");
        }
        
        public void ApplyPreset_TestRelease()
        {
            Undo.RecordObject(this, nameof(ApplyPreset_TestRelease));
            _buildType = BuildType.Release;
            _isDevelopmentBuild = false;
            _buildOptions = BuildOptions.None;
            _versionIncrementMode = VersionIncrementType.Patch;
            _strippingLevel = ManagedStrippingLevel.High;
            _defineSymbols = ScriptDefineSymbols.NONE;
            _stackTraces = StackTraceSettings.ReleaseDefault;
            _androidSettings = AndroidBuildSettings.DebugDefault;
            _iosSettings = IOSBuildSettings.Default;
            EditorUtility.SetDirty(this);
            Debug.Log($"[{LOG_PREFIX}Applied 'Test Release' preset to '{name}'.");
        }

        public void ApplyPreset_PublishRelease()
        {
            Undo.RecordObject(this, nameof(ApplyPreset_PublishRelease));
            _buildType = BuildType.Release;
            _isDevelopmentBuild = false;
            _buildOptions = BuildOptions.None;
            _versionIncrementMode = VersionIncrementType.Minor;
            _strippingLevel = ManagedStrippingLevel.High;
            _defineSymbols = ScriptDefineSymbols.NONE;
            _stackTraces = StackTraceSettings.ReleaseDefault;
            _androidSettings = AndroidBuildSettings.ReleaseDefault;
            _iosSettings = IOSBuildSettings.Default;
            EditorUtility.SetDirty(this);
            Debug.Log($"[{LOG_PREFIX}Applied 'Publish Release' preset to '{name}'. Ensure iOS Developer Team ID is set if targeting iOS Release.");
        }
    }
}