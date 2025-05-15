using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public sealed class SceneLoaderWindow : EditorWindow
{
    [SerializeField] private List<string> _scenePaths = new List<string>();

    private SerializedObject _serializedObject;
    private SerializedProperty _serializedScenePaths;
    private Vector2 _scrollPosition;

    private const string _windowName = "Scene Loader";
    private const string _title = "Available Scenes";

    [MenuItem("Window/Scene Loader")]
    public static void ShowWindow() => GetWindow<SceneLoaderWindow>(_windowName);

    private void OnEnable()
    {
        _serializedObject = new SerializedObject(this);
        _serializedScenePaths = _serializedObject.FindProperty(nameof(_scenePaths));
    }

    private void OnGUI()
    {
        _serializedObject.Update();

        GUILayout.Label(_title, EditorStyles.boldLabel);

        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

        for (int i = 0; i < _serializedScenePaths.arraySize; i++)
        {
            SerializedProperty scenePathProperty = _serializedScenePaths.GetArrayElementAtIndex(i);

            if (GUILayout.Button(Path.GetFileNameWithoutExtension(scenePathProperty.stringValue)))
                OpenScene(scenePathProperty.stringValue);
        }

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Add Scene Folder"))
        {
            string folderPath = EditorUtility.OpenFolderPanel("Select Scene Folder", "Assets", "");

            if (string.IsNullOrEmpty(folderPath))
                return;

            if (folderPath.StartsWith(Application.dataPath))
                folderPath = "Assets" + folderPath.Substring(Application.dataPath.Length);

            ClearScenePaths();
            AddScenesFromFolder(folderPath);
        }

        _serializedObject.ApplyModifiedProperties();
    }

    public void AddScenesFromFolder(string folderPath)
    {
        string[] scenes = Directory.GetFiles(folderPath, "*.unity", SearchOption.AllDirectories);

        foreach (string scene in scenes)
            if (!_scenePaths.Contains(scene))
                _scenePaths.Add(scene);
    }

    public void ClearScenePaths() => _scenePaths.Clear();

    private void OpenScene(string scenePath)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            EditorSceneManager.OpenScene(scenePath);
    }
}
