using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SceneSetupAutomation : EditorWindow
{
    [MenuItem("Tools/Setup All Scenes")]
    public static void SetupAllScenes()
    {
        // Setup Scene2
        SetupGameplayScene("Assets/Scenes/Scene2.unity");
        
        // Setup Hub
        SetupGameplayScene("Assets/Scenes/Hub.unity");
        
        // Setup Mainmenu
        SetupMainMenuScene("Assets/Scenes/Mainmenu.unity");
        
        Debug.Log("All scenes setup complete!");
    }
    
    private static void SetupGameplayScene(string scenePath)
    {
        EditorSceneManager.OpenScene(scenePath);
        
        // Find or create Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }
        
        // Create UIManager
        GameObject uiManager = new GameObject("UIManager");
        uiManager.transform.SetParent(canvas.transform, false);
        uiManager.AddComponent<UIManager>();
        
        // Create HealthBar (Slider)
        GameObject healthBarGO = new GameObject("HealthBar");
        healthBarGO.transform.SetParent(canvas.transform, false);
        Slider healthBar = healthBarGO.AddComponent<Slider>();
        RectTransform healthBarRect = healthBarGO.GetComponent<RectTransform>();
        healthBarRect.anchorMin = new Vector2(0, 1);
        healthBarRect.anchorMax = new Vector2(0, 1);
        healthBarRect.pivot = new Vector2(0, 1);
        healthBarRect.anchoredPosition = new Vector2(20, -20);
        healthBarRect.sizeDelta = new Vector2(200, 20);
        
        // Create HealthText
        GameObject healthTextGO = new GameObject("HealthText");
        healthTextGO.transform.SetParent(canvas.transform, false);
        TextMeshProUGUI healthText = healthTextGO.AddComponent<TextMeshProUGUI>();
        healthText.text = "Health: 100/100";
        healthText.fontSize = 18;
        RectTransform healthTextRect = healthTextGO.GetComponent<RectTransform>();
        healthTextRect.anchorMin = new Vector2(0, 1);
        healthTextRect.anchorMax = new Vector2(0, 1);
        healthTextRect.pivot = new Vector2(0, 1);
        healthTextRect.anchoredPosition = new Vector2(20, -50);
        healthTextRect.sizeDelta = new Vector2(200, 30);
        
        // Create ScoreText
        GameObject scoreTextGO = new GameObject("ScoreText");
        scoreTextGO.transform.SetParent(canvas.transform, false);
        TextMeshProUGUI scoreText = scoreTextGO.AddComponent<TextMeshProUGUI>();
        scoreText.text = "Score: 0";
        scoreText.fontSize = 24;
        scoreText.alignment = TextAlignmentOptions.TopRight;
        RectTransform scoreTextRect = scoreTextGO.GetComponent<RectTransform>();
        scoreTextRect.anchorMin = new Vector2(1, 1);
        scoreTextRect.anchorMax = new Vector2(1, 1);
        scoreTextRect.pivot = new Vector2(1, 1);
        scoreTextRect.anchoredPosition = new Vector2(-20, -20);
        scoreTextRect.sizeDelta = new Vector2(200, 50);
        
        // Create GameOverPanel
        GameObject gameOverPanel = new GameObject("GameOverPanel");
        gameOverPanel.transform.SetParent(canvas.transform, false);
        Image gameOverImage = gameOverPanel.AddComponent<Image>();
        gameOverImage.color = new Color(0, 0, 0, 0.8f);
        RectTransform gameOverRect = gameOverPanel.GetComponent<RectTransform>();
        gameOverRect.anchorMin = Vector2.zero;
        gameOverRect.anchorMax = Vector2.one;
        gameOverRect.sizeDelta = Vector2.zero;
        gameOverPanel.SetActive(false);
        
        // Create VictoryPanel
        GameObject victoryPanel = new GameObject("VictoryPanel");
        victoryPanel.transform.SetParent(canvas.transform, false);
        Image victoryImage = victoryPanel.AddComponent<Image>();
        victoryImage.color = new Color(0, 0.5f, 0, 0.8f);
        RectTransform victoryRect = victoryPanel.GetComponent<RectTransform>();
        victoryRect.anchorMin = Vector2.zero;
        victoryRect.anchorMax = Vector2.one;
        victoryRect.sizeDelta = Vector2.zero;
        victoryPanel.SetActive(false);
        
        // Create PauseMenu
        GameObject pauseMenu = new GameObject("PauseMenu");
        pauseMenu.transform.SetParent(canvas.transform, false);
        pauseMenu.AddComponent<PauseMenu>();
        
        GameObject pausePanel = new GameObject("PauseMenuPanel");
        pausePanel.transform.SetParent(pauseMenu.transform, false);
        Image pauseImage = pausePanel.AddComponent<Image>();
        pauseImage.color = new Color(0, 0, 0, 0.9f);
        RectTransform pauseRect = pausePanel.GetComponent<RectTransform>();
        pauseRect.anchorMin = Vector2.zero;
        pauseRect.anchorMax = Vector2.one;
        pauseRect.sizeDelta = Vector2.zero;
        pausePanel.SetActive(false);
        
        // Create GameManager
        GameObject gameManager = new GameObject("GameManager");
        gameManager.AddComponent<GameManager>();
        
        // Create AudioManager
        GameObject audioManager = new GameObject("AudioManager");
        audioManager.AddComponent<AudioManager>();
        audioManager.AddComponent<AudioSource>();
        
        // Create SpawnManager
        GameObject spawnManager = new GameObject("SpawnManager");
        spawnManager.AddComponent<SpawnManager>();
        
        // Create SpawnPoints parent
        GameObject spawnPoints = new GameObject("SpawnPoints");
        for (int i = 1; i <= 4; i++)
        {
            GameObject spawnPoint = new GameObject($"SpawnPoint{i}");
            spawnPoint.transform.SetParent(spawnPoints.transform);
            spawnPoint.transform.position = new Vector3(i * 20 - 10, -2, 0);
        }
        
        // Create CheckpointSystem
        GameObject checkpointSystem = new GameObject("CheckpointSystem");
        checkpointSystem.AddComponent<CheckpointSystem>();
        
        // Create sample Checkpoint
        GameObject checkpoint1 = new GameObject("Checkpoint1");
        checkpoint1.AddComponent<Checkpoint>();
        BoxCollider2D checkpointCollider = checkpoint1.AddComponent<BoxCollider2D>();
        checkpointCollider.isTrigger = true;
        checkpointCollider.size = new Vector2(2, 3);
        checkpoint1.transform.position = new Vector3(0, -2, 0);
        
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        Debug.Log($"Setup complete for {scenePath}");
    }
    
    private static void SetupMainMenuScene(string scenePath)
    {
        EditorSceneManager.OpenScene(scenePath);
        
        // Check if already has Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasGO.AddComponent<GraphicRaycaster>();
        }
        
        // Create MainMenu GameObject
        GameObject mainMenu = new GameObject("MainMenu");
        mainMenu.transform.SetParent(canvas.transform, false);
        mainMenu.AddComponent<MainMenu>();
        
        // Create GameManager
        if (FindObjectOfType<GameManager>() == null)
        {
            GameObject gameManager = new GameObject("GameManager");
            gameManager.AddComponent<GameManager>();
        }
        
        // Create AudioManager
        if (FindObjectOfType<AudioManager>() == null)
        {
            GameObject audioManager = new GameObject("AudioManager");
            audioManager.AddComponent<AudioManager>();
            audioManager.AddComponent<AudioSource>();
        }
        
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        Debug.Log($"Setup complete for {scenePath}");
    }
}
