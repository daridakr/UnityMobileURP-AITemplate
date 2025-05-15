using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace Editors.Build.Configuration
{
    [CustomEditor(typeof(BuildProfileSO))]
    public class BuildProfileSOEditor : Editor
    {
        private List<PresetAction> _presets;
        private string[] _presetNames;
        private int _selectedPresetIndex = 0;

        private void OnEnable()
        {
            _presets = new List<PresetAction>
            {
                new("Fast Debug (Optimized for speed)", profile => profile.ApplyPreset_FastDebug()),
                new("Default Debug (Balanced)", profile => profile.ApplyPreset_DefaultDebug()),
                new("Test Release (For test release build)", profile => profile.ApplyPreset_TestRelease()),
                new("Publish Release (For publishing)", profile => profile.ApplyPreset_PublishRelease()),
            };

            _presetNames = new string[_presets.Count];

            for (int i = 0; i < _presets.Count; i++)
                _presetNames[i] = _presets[i].Name;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Apply Configuration Preset", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            _selectedPresetIndex = EditorGUILayout.Popup(_selectedPresetIndex, _presetNames);

            if (GUILayout.Button("Apply Preset", GUILayout.Width(100)))
            {
                BuildProfileSO profile = (BuildProfileSO)target;

                if (EditorUtility.DisplayDialog("Confirm Apply Preset",
                    $"Are you sure you want to apply the '{_presets[_selectedPresetIndex].Name}' preset to '{profile.name}'?\n" +
                    "This will overwrite current settings in this profile.", "Yes, Apply", "Cancel"))
                {
                    _presets[_selectedPresetIndex].ApplyAction(profile);
                }
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.HelpBox("Applying a preset will overwrite the fields in this Scriptable Object with predefined values. Use Ctrl+Z (Cmd+Z on Mac) to undo.", MessageType.Info);

            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Profile Specific Settings", EditorStyles.boldLabel);
            DrawDefaultInspector();

            serializedObject.ApplyModifiedProperties();
        }

        private readonly struct PresetAction
        {
            public readonly string Name { get; }
            public readonly Action<BuildProfileSO> ApplyAction { get; }

            public PresetAction(string name, Action<BuildProfileSO> applyAction)
            {
                Name = name;
                ApplyAction = applyAction;
            }
        }
    }
}