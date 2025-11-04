using UnityEngine;
using UnityEditor;

public class BuildSettingsSetup : EditorWindow
{
    [MenuItem("Tools/Configure Build Settings")]
    public static void ConfigureBuildSettings()
    {
        // Define scenes in correct order
        string[] scenePaths = new string[]
        {
            "Assets/Scenes/Mainmenu.unity",    // Index 0
            "Assets/Scenes/Scene1.unity",      // Index 1
            "Assets/Scenes/Scene2.unity",      // Index 2
            "Assets/Scenes/Hub.unity"          // Index 3
        };
        
        // Create EditorBuildSettingsScene array
        EditorBuildSettingsScene[] buildScenes = new EditorBuildSettingsScene[scenePaths.Length];
        
        for (int i = 0; i < scenePaths.Length; i++)
        {
            buildScenes[i] = new EditorBuildSettingsScene(scenePaths[i], true);
            Debug.Log($"Added to Build Settings [{i}]: {scenePaths[i]}");
        }
        
        // Apply to Build Settings
        EditorBuildSettings.scenes = buildScenes;
        
        Debug.Log("Build Settings configured successfully!");
        Debug.Log("Scene order:");
        Debug.Log("  [0] Mainmenu");
        Debug.Log("  [1] Scene1");
        Debug.Log("  [2] Scene2");
        Debug.Log("  [3] Hub");
    }
}
