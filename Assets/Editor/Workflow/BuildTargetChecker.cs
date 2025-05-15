using UnityEditor;

[InitializeOnLoad]
public class BuildTargetChecker
{
    static BuildTargetChecker() => EditorApplication.update += RunCheck;

    public static void RunCheck()
    {
        EditorApplication.update -= RunCheck;

        CheckBuildTarget();
    }

    public static void CheckBuildTarget()
    {
        BuildTarget currentTarget = EditorUserBuildSettings.activeBuildTarget;
        BuildTargetGroup currentGroup = BuildPipeline.GetBuildTargetGroup(currentTarget);

        BuildTarget androidBuildTarget = BuildTarget.Android;
        BuildTarget iosBuildTarget = BuildTarget.iOS;

        if (currentTarget != androidBuildTarget && currentTarget != iosBuildTarget)
        {
            bool shouldSwitch = EditorUtility.DisplayDialog(
                "Wrong Build Target",
                $"Active build target: {currentTarget}.\n\n" +
                $"It is recommended to switch to Android or iOS to work on the project correctly.\n\n" +
                "Go to Build Settings to switch to Android/IOS?",
                "Yes, open Build Settings",
                "No, ignore"
            );

            if (shouldSwitch)
            {
                EditorWindow.GetWindow(typeof(BuildPlayerWindow));
                // НЕ ИСПОЛЬЗУЙТЕ EditorUserBuildSettings.SwitchActiveBuildTarget здесь!
                // Это вызовет немедленный реимпорт и может быть нежелательно.
                // Просто открываем окно настроек сборки.
            }
        }
    }
}