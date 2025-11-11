using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingSceneManager : MonoBehaviour
{
    private const float LOAD_DELAY = 1.5f;
    
    private void Start()
    {
        // Start the loading coroutine
        StartCoroutine(LoadTargetSceneCoroutine());
    }
    
    private IEnumerator LoadTargetSceneCoroutine()
    {
        // Wait a few frames to ensure the scene is fully loaded and rendered
        // This ensures the campfire and ninja are visible before the delay starts
        yield return null; // Wait one frame for initialization
        yield return new WaitForEndOfFrame(); // Wait for rendering to complete
        yield return null; // Wait one more frame to ensure everything is rendered
        
        // Get the target scene name from GameManager
        string targetScene = GameManager.Instance != null 
            ? GameManager.Instance.GetTargetScene() 
            : null;
        
        if (string.IsNullOrEmpty(targetScene))
        {
            Debug.LogWarning("LoadingSceneManager: No target scene set, defaulting to Mainmenu");
            targetScene = "Mainmenu";
        }
        
        // Wait for the static delay
        yield return new WaitForSeconds(LOAD_DELAY);
        
        // Load the target scene
        SceneManager.LoadScene(targetScene);
    }
}