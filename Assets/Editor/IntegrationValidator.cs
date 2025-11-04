using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class IntegrationValidator : EditorWindow
{
    [MenuItem("Tools/Validate Scene Setup")]
    public static void ValidateCurrentScene()
    {
        string sceneName = EditorSceneManager.GetActiveScene().name;
        Debug.Log($"=== Validating Scene: {sceneName} ===");
        
        int issuesFound = 0;
        int warningsFound = 0;
        
        // Check for GameManager
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("❌ GameManager not found in scene!");
            issuesFound++;
        }
        else
        {
            Debug.Log("✅ GameManager found");
        }
        
        // Check for AudioManager
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        if (audioManager == null)
        {
            Debug.LogError("❌ AudioManager not found in scene!");
            issuesFound++;
        }
        else
        {
            Debug.Log("✅ AudioManager found");
        }
        
        // Check for Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("❌ Canvas not found in scene!");
            issuesFound++;
        }
        else
        {
            Debug.Log("✅ Canvas found");
        }
        
        // Scene-specific checks
        if (sceneName == "Scene1" || sceneName == "Scene2" || sceneName == "Hub")
        {
            ValidateGameplayScene(ref issuesFound, ref warningsFound);
        }
        else if (sceneName == "Mainmenu")
        {
            ValidateMainMenuScene(ref issuesFound, ref warningsFound);
        }
        
        // Summary
        Debug.Log($"\n=== Validation Complete ===");
        Debug.Log($"Errors: {issuesFound}");
        Debug.Log($"Warnings: {warningsFound}");
        
        if (issuesFound == 0 && warningsFound == 0)
        {
            Debug.Log("🎉 Scene is fully set up and ready!");
        }
        else if (issuesFound == 0)
        {
            Debug.Log("✅ No critical issues. Review warnings for optimization.");
        }
        else
        {
            Debug.LogWarning($"⚠️ Found {issuesFound} critical issues that need to be fixed.");
        }
    }
    
    private static void ValidateGameplayScene(ref int issues, ref int warnings)
    {
        // Check UIManager
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("❌ UIManager not found!");
            issues++;
        }
        else
        {
            Debug.Log("✅ UIManager found");
            
            // Check UIManager references using reflection to avoid compile errors
            System.Type type = uiManager.GetType();
            
            CheckField(uiManager, "healthBar", "Slider", ref warnings);
            CheckField(uiManager, "healthText", "TextMeshProUGUI", ref warnings);
            CheckField(uiManager, "scoreText", "TextMeshProUGUI", ref warnings);
            CheckField(uiManager, "gameOverPanel", "GameObject", ref warnings);
            CheckField(uiManager, "victoryPanel", "GameObject", ref warnings);
        }
        
        // Check PauseMenu
        PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
        if (pauseMenu == null)
        {
            Debug.LogError("❌ PauseMenu not found!");
            issues++;
        }
        else
        {
            Debug.Log("✅ PauseMenu found");
        }
        
        // Check SpawnManager
        SpawnManager spawnManager = FindObjectOfType<SpawnManager>();
        if (spawnManager == null)
        {
            Debug.LogWarning("⚠️ SpawnManager not found (optional for Hub)");
            warnings++;
        }
        else
        {
            Debug.Log("✅ SpawnManager found");
        }
        
        // Check CheckpointSystem
        CheckpointSystem checkpointSystem = FindObjectOfType<CheckpointSystem>();
        if (checkpointSystem == null)
        {
            Debug.LogWarning("⚠️ CheckpointSystem not found");
            warnings++;
        }
        else
        {
            Debug.Log("✅ CheckpointSystem found");
        }
        
        // Check for Player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("⚠️ Player not found in scene (needs to be placed manually)");
            warnings++;
        }
        else
        {
            Debug.Log("✅ Player found");
        }
    }
    
    private static void ValidateMainMenuScene(ref int issues, ref int warnings)
    {
        // Check MainMenu
        MainMenu mainMenu = FindObjectOfType<MainMenu>();
        if (mainMenu == null)
        {
            Debug.LogError("❌ MainMenu not found!");
            issues++;
        }
        else
        {
            Debug.Log("✅ MainMenu found");
            
            CheckField(mainMenu, "startButton", "Button", ref warnings);
            CheckField(mainMenu, "settingsButton", "Button", ref warnings);
            CheckField(mainMenu, "creditsButton", "Button", ref warnings);
            CheckField(mainMenu, "quitButton", "Button", ref warnings);
        }
    }
    
    private static void CheckField(Object obj, string fieldName, string typeName, ref int warnings)
    {
        var field = obj.GetType().GetField(fieldName, 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);
        
        if (field != null)
        {
            var value = field.GetValue(obj);
            if (value == null || value.ToString() == "null")
            {
                Debug.LogWarning($"⚠️ {obj.GetType().Name}.{fieldName} ({typeName}) is not assigned");
                warnings++;
            }
        }
    }
}
