using UnityEngine;
using UnityEditor;

public class EnsureAudioListener : EditorWindow
{
    [MenuItem("Tools/Ensure AudioListener on Camera")]
    public static void EnsureAudioListenerOnCamera()
    {
        // Find the main camera
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            // Try to find any camera
            mainCamera = FindFirstObjectByType<Camera>();
        }
        
        if (mainCamera == null)
        {
            Debug.LogError("No camera found in the scene! Please add a camera first.");
            return;
        }
        
        // Check if AudioListener exists
        AudioListener audioListener = mainCamera.GetComponent<AudioListener>();
        if (audioListener == null)
        {
            // Add AudioListener component
            audioListener = mainCamera.gameObject.AddComponent<AudioListener>();
            Debug.Log($"AudioListener added to {mainCamera.gameObject.name}");
            
            // Mark scene as dirty
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        }
        else
        {
            Debug.Log($"AudioListener already exists on {mainCamera.gameObject.name}");
        }
    }
}








