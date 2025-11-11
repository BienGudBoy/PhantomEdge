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
        // Wait a frame to ensure the scene (including AudioListener) is fully initialized
        yield return null;
        
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