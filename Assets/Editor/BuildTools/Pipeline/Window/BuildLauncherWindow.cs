using System.Collections.Generic;
using System.Linq;
using Editors.Build.Configuration;
using UnityEditor;
using UnityEngine;

namespace Editors.Build.Automation
{
    public class BuildLauncherWindow : EditorWindow
    {
        private List<BuildProfileSO> _availableProfiles = new();
        private string[] _profileNames;
        private int _selectedProfileIndex = 0;
        private BuildTarget _selectedBuildTarget;
        private Vector2 _scrollPosition;

        private const string MENU_ITEM_NAME = "Build Project/Open Build Launcher...";
        private const string WINDOW_NAME = nameof(BuildLauncherWindow);

        [MenuItem(MENU_ITEM_NAME, priority = 0)]
        public static void ShowWindow() => GetWindow<BuildLauncherWindow>(WINDOW_NAME);

        private void OnEnable()
        {
            LoadAvailableProfiles();

            _selectedBuildTarget = EditorUserBuildSettings.activeBuildTarget;
        }

        private void LoadAvailableProfiles()
        {
            _availableProfiles.Clear();

            string[] guids = AssetDatabase.FindAssets($"t:{typeof(BuildProfileSO).Name}");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                BuildProfileSO profile = AssetDatabase.LoadAssetAtPath<BuildProfileSO>(path);

                if (profile != null) _availableProfiles.Add(profile);
            }

            _availableProfiles = _availableProfiles.OrderBy(p => p.name).ToList();
            _profileNames = _availableProfiles.Select(p => p.name).ToArray();
            _selectedProfileIndex = Mathf.Clamp(_selectedProfileIndex, 0, Mathf.Max(0, _profileNames.Length - 1));
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Build Automation Launcher", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (_availableProfiles.Count == 0)
            {
                EditorGUILayout.HelpBox("No Build Profiles (BuildProfileSO) found in the project. Please create some first via Assets/Create menu.", MessageType.Warning);
                if (GUILayout.Button("Refresh Profiles")) LoadAvailableProfiles();

                return;
            }

            EditorGUILayout.BeginHorizontal();

            _selectedProfileIndex = EditorGUILayout.Popup("Select Build Profile", _selectedProfileIndex, _profileNames);

            if (GUILayout.Button("Refresh", GUILayout.Width(70)))
            {
                LoadAvailableProfiles();
                return;
            }

            EditorGUILayout.EndHorizontal();
            
            _selectedBuildTarget = (BuildTarget)EditorGUILayout.EnumPopup("Target Platform", _selectedBuildTarget);

            EditorGUILayout.Space();

            BuildProfileSO currentProfile = _availableProfiles[_selectedProfileIndex];

            if (currentProfile != null)
            {
                EditorGUILayout.LabelField("Selected Profile Details:", EditorStyles.centeredGreyMiniLabel);

                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(150));
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                DisplayProfileInfo(currentProfile);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndScrollView();

                EditorGUILayout.Space();

                GUI.backgroundColor = Color.green;

                if (GUILayout.Button($"Build '{currentProfile.name}' for {_selectedBuildTarget}", GUILayout.Height(40)))
                {
                    if (EditorUtility.DisplayDialog("Confirm Build",
                        $"Are you sure you want to build profile '{currentProfile.name}' for target '{_selectedBuildTarget}'?\n\n" +
                        $"This will modify your Project Settings according to the profile.", "Yes, Build It", "Cancel"))
                    {
                        if (_selectedBuildTarget != EditorUserBuildSettings.activeBuildTarget)
                        {
                           if (EditorUtility.DisplayDialog("Switch Active Platform?",
                               $"The selected target platform ('{_selectedBuildTarget}') is different from the current active platform ('{EditorUserBuildSettings.activeBuildTarget}').\n\n" +
                               "Do you want to switch the active platform before building? (Recommended)", "Switch and Build", "Build Anyway (Not Recommended)"))
                            {
                                BuildTargetGroup group = BuildPipeline.GetBuildTargetGroup(_selectedBuildTarget);

                                if (EditorUserBuildSettings.SwitchActiveBuildTarget(group, _selectedBuildTarget))
                                    ProjectBuilder.ExecuteBuildProcess(currentProfile, _selectedBuildTarget);
                                else
                                    Debug.LogError($"{WINDOW_NAME} Failed to switch active build target to {_selectedBuildTarget}. Build cancelled.");
                            }
                            else
                            {
                                Debug.LogWarning($"{WINDOW_NAME} Building for '{_selectedBuildTarget}' while active target is '{EditorUserBuildSettings.activeBuildTarget}'. This might lead to unexpected results or longer build times.");
                                ProjectBuilder.ExecuteBuildProcess(currentProfile, _selectedBuildTarget);
                            }
                        }
                        else ProjectBuilder.ExecuteBuildProcess(currentProfile, _selectedBuildTarget);
                    }
                }

                GUI.backgroundColor = Color.white;
            }
            else EditorGUILayout.HelpBox("Selected profile is null. Please refresh.", MessageType.Error);
        }

        private void DisplayProfileInfo(BuildProfileSO profile)
        {
            EditorGUILayout.LabelField("Profile Name:", profile.name);
            EditorGUILayout.LabelField("Build Type:", profile.ProfileBuildType.ToString());
            EditorGUILayout.LabelField("Output Name Prefix:", profile.BuildNamePrefix);
            EditorGUILayout.LabelField("Dev Build:", profile.IsDevelopmentBuild.ToString());
            EditorGUILayout.LabelField("Define Symbols:", profile.EffectiveDefineSymbols);
        }
    }
}