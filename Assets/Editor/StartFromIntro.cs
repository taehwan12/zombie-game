using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class StartFromIntro
{
    static StartFromIntro()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        
        // Ensure Intro scene is the start scene
        SetIntroAsStartScene();
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            // If the user starts play mode, save the current scene and load the intro scene
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }

        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            if (SceneHierarchyHooks.IsScene0(SceneHierarchyHooks.GetActiveSceneName()))
            {
                // Already in scene 0
                return;
            }
            
            // This is actually better done with SceneBootstrapper pattern but for now 
            // we'll just check if we should load scene 0.
            // However, a simpler way is to use EditorSceneManager.playModeStartScene.
        }
    }

    [MenuItem("Tools/Set Intro as Start Scene")]
    public static void SetIntroAsStartScene()
    {
        var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/Intro.unity");
        if (scene != null)
        {
            EditorSceneManager.playModeStartScene = scene;
            Debug.Log("Intro scene set as play mode start scene.");
        }
    }
}

internal static class SceneHierarchyHooks {
    public static bool IsScene0(string name) => name == "Intro";
    public static string GetActiveSceneName() => UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
}