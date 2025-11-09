using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [Header("HUD Elements")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI scoreText;
    
    [Header("Game Over Panel")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    
    [Header("Victory Panel")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private TextMeshProUGUI victoryScoreText;
    
    private HealthSystem playerHealth;
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        // Subscribe to GameManager events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged += UpdateScore;
            GameManager.Instance.OnStateChanged += HandleStateChanged;
        }
        
        // Find player and subscribe to health events
        FindAndSubscribeToPlayer();
        
        // Initialize UI
        UpdateScore(0);
        
        // Hide panels at start
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
    }
    
    private void FindAndSubscribeToPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<HealthSystem>();
            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged += UpdateHealth;
                // Note: Death is handled by GameManager -> YouDiedScreen, not here
                
                // Initialize health display
                UpdateHealth(playerHealth.CurrentHealth, playerHealth.MaxHealth);
            }
        }
    }
    
    private void UpdateHealth(int currentHealth, int maxHealth)
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
        
        if (healthText != null)
        {
            healthText.text = $"{currentHealth} / {maxHealth}";
        }
    }
    
    private void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }
    
    private void HandleStateChanged(GameManager.GameState newState)
    {
        switch (newState)
        {
            case GameManager.GameState.GameOver:
                // GameOver is now handled by YouDiedScreen, not the old gameOverPanel
                // Keep this for backwards compatibility if needed
                break;
            case GameManager.GameState.Victory:
                ShowVictory();
                break;
            case GameManager.GameState.Playing:
                HideAllPanels();
                break;
        }
    }
    
    private void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            if (finalScoreText != null && GameManager.Instance != null)
            {
                finalScoreText.text = $"Final Score: {GameManager.Instance.Score}";
            }
        }
    }
    
    private void ShowVictory()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
            
            if (victoryScoreText != null && GameManager.Instance != null)
            {
                victoryScoreText.text = $"Final Score: {GameManager.Instance.Score}";
            }
        }
    }
    
    private void HideAllPanels()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
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
    
    public void OnMenuButton()
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
    
    public void OnNextLevelButton()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayClickSound();
        }
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.NextLevel();
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged -= UpdateScore;
            GameManager.Instance.OnStateChanged -= HandleStateChanged;
        }
        
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealth;
        }
    }
}