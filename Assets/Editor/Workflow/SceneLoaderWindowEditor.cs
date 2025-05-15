using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SceneLoaderWindow))]
public sealed class SceneLoaderWindowEditor : Editor
{
    SerializedProperty _scenePaths;

    private void OnEnable() =>
        _scenePaths = serializedObject.FindProperty(nameof(_scenePaths));

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_scenePaths, new GUIContent("Scene Paths"), true);

        serializedObject.ApplyModifiedProperties();

        SceneLoaderWindow window = (SceneLoaderWindow)target;

        if (GUILayout.Button("Add Scene Folder"))
        {
            string folderPath = EditorUtility.OpenFolderPanel("Select Scene Folder", "Assets", "");

            if (string.IsNullOrEmpty(folderPath))
                return;

            if (folderPath.StartsWith(Application.dataPath))
                folderPath = "Assets" + folderPath.Substring(Application.dataPath.Length);

            window.ClearScenePaths();
            window.AddScenesFromFolder(folderPath);
        }
    }
}
