using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("Pause Menu Panel")]
    [SerializeField] private GameObject pauseMenuPanel;
    
    [Header("Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;
    
    // Public method for Editor setup
    public void SetupButtons(GameObject panel, Button resume, Button restart, Button mainMenu, Button quit)
    {
        pauseMenuPanel = panel;
        resumeButton = resume;
        restartButton = restart;
        mainMenuButton = mainMenu;
        quitButton = quit;
    }
    
    private void Start()
    {
        // Subscribe to GameManager state changes
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged += HandleStateChanged;
        }
        
        // Setup button listeners
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(OnResumeButton);
        }
        
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartButton);
        }
        
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(OnMainMenuButton);
        }
        
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitButton);
        }
        
        // Hide pause menu at start
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
    }
    
    // Removed Update() - GameManager handles Escape key to avoid conflicts
    
    private void HandleStateChanged(GameManager.GameState newState)
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(newState == GameManager.GameState.Paused);
        }
    }
    
    private void ShowPauseMenu()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PauseGame();
        }
        
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }
    }
    
    public void OnResumeButton()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayClickSound();
        }
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResumeGame();
        }
        
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
    }
    
    public void OnRestartButton()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayClickSound();
        }
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }
    
    public void OnMainMenuButton()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayClickSound();
        }
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToMenu();
        }
    }
    
    public void OnQuitButton()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayClickSound();
        }
        
        // Quit the game
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged -= HandleStateChanged;
        }
        
        // Remove button listeners
        if (resumeButton != null) resumeButton.onClick.RemoveListener(OnResumeButton);
        if (restartButton != null) restartButton.onClick.RemoveListener(OnRestartButton);
        if (mainMenuButton != null) mainMenuButton.onClick.RemoveListener(OnMainMenuButton);
        if (quitButton != null) quitButton.onClick.RemoveListener(OnQuitButton);
    }
}