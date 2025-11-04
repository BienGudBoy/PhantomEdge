using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Main Menu Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;
    
    [Header("Settings Panel")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Button settingsCloseButton;
    
    [Header("Credits Panel")]
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private Button creditsCloseButton;
    
    [Header("Scene Settings")]
    [SerializeField] private string firstLevelScene = "Scene1";
    
    private void Start()
    {
        // Set initial game state
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetState(GameManager.GameState.Menu);
        }
        
        // Setup button listeners
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButton);
        }
        
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueButton);
            // Disable continue button if no saved progress
            continueButton.interactable = HasSavedProgress();
        }
        
        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OnSettingsButton);
        }
        
        if (creditsButton != null)
        {
            creditsButton.onClick.AddListener(OnCreditsButton);
        }
        
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitButton);
        }
        
        if (settingsCloseButton != null)
        {
            settingsCloseButton.onClick.AddListener(OnSettingsClose);
        }
        
        if (creditsCloseButton != null)
        {
            creditsCloseButton.onClick.AddListener(OnCreditsClose);
        }
        
        // Setup volume slider
        if (volumeSlider != null)
        {
            volumeSlider.value = AudioListener.volume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
        
        // Hide panels at start
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
    }
    
    public void OnStartButton()
    {
        PlayClickSound();
        
        // Start new game
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadScene(firstLevelScene);
            GameManager.Instance.StartGame();
        }
        else
        {
            SceneManager.LoadScene(firstLevelScene);
        }
    }
    
    public void OnContinueButton()
    {
        PlayClickSound();
        
        // Load saved game (for now, just load first level)
        // TODO: Implement save/load system
        OnStartButton();
    }
    
    public void OnSettingsButton()
    {
        PlayClickSound();
        
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }
    
    public void OnSettingsClose()
    {
        PlayClickSound();
        
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }
    
    public void OnCreditsButton()
    {
        PlayClickSound();
        
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(true);
        }
    }
    
    public void OnCreditsClose()
    {
        PlayClickSound();
        
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(false);
        }
    }
    
    public void OnQuitButton()
    {
        PlayClickSound();
        
        // Quit the game
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    private void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        // TODO: Save volume preference
    }
    
    private bool HasSavedProgress()
    {
        // TODO: Implement save system check
        // For now, return false
        return false;
    }
    
    private void PlayClickSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayClickSound();
        }
    }
    
    public void LoadScene(string sceneName)
    {
        PlayClickSound();
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadScene(sceneName);
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }
    
    private void OnDestroy()
    {
        // Remove button listeners
        if (startButton != null) startButton.onClick.RemoveListener(OnStartButton);
        if (continueButton != null) continueButton.onClick.RemoveListener(OnContinueButton);
        if (settingsButton != null) settingsButton.onClick.RemoveListener(OnSettingsButton);
        if (creditsButton != null) creditsButton.onClick.RemoveListener(OnCreditsButton);
        if (quitButton != null) quitButton.onClick.RemoveListener(OnQuitButton);
        if (settingsCloseButton != null) settingsCloseButton.onClick.RemoveListener(OnSettingsClose);
        if (creditsCloseButton != null) creditsCloseButton.onClick.RemoveListener(OnCreditsClose);
        if (volumeSlider != null) volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
    }
}