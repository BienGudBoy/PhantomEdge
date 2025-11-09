using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    // Game State]
    private GameState currentState = GameState.Menu;
    
    // Score]
    private int score = 0;
    
    // Events
    public event Action<int> OnScoreChanged;
    public event Action<GameState> OnStateChanged;
    
    private HealthSystem playerHealth;
    private bool isPaused = false;
    
    public enum GameState
    {
        Menu,
        Playing,
        Paused,
        GameOver,
        Victory
    }
    
    public GameState CurrentState => currentState;
    public int Score => score;
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // Subscribe to scene loaded event
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Only set state if we're not already paused (don't override pause state)
        if (currentState == GameState.Paused)
        {
            return;
        }
        
        // Set state based on loaded scene
        if (scene.name == "Mainmenu")
        {
            SetState(GameState.Menu);
        }
        else
        {
            // We're in a gameplay scene
            SetState(GameState.Playing);
            
            // Find player health
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerHealth = player.GetComponent<HealthSystem>();
                if (playerHealth != null)
                {
                    playerHealth.OnDeath -= HandlePlayerDeath;
                    playerHealth.OnDeath += HandlePlayerDeath;
                }
            }
        }
    }
    
    private void Start()
    {
        // Check if we're in a gameplay scene or menu scene
        string currentScene = SceneManager.GetActiveScene().name;
        
        if (currentScene == "Mainmenu")
        {
            SetState(GameState.Menu);
        }
        else
        {
            // We're in a gameplay scene (Scene1, Scene2, Hub, etc.)
            SetState(GameState.Playing);
            
            // Find player health
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerHealth = player.GetComponent<HealthSystem>();
                if (playerHealth != null)
                {
                    playerHealth.OnDeath -= HandlePlayerDeath;
                    playerHealth.OnDeath += HandlePlayerDeath;
                }
            }
        }
    }
    
    private void Update()
    {
        // Handle pause with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Playing)
            {
                PauseGame();
            }
            else if (currentState == GameState.Paused)
            {
                ResumeGame();
            }
        }
    }
    
    public void SetState(GameState newState)
    {
        currentState = newState;
        OnStateChanged?.Invoke(currentState);
    }
    
    public void StartGame()
    {
        SetState(GameState.Playing);
        score = 0;
        OnScoreChanged?.Invoke(score);
        
        // Find player health
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<HealthSystem>();
            if (playerHealth != null)
            {
                playerHealth.OnDeath -= HandlePlayerDeath;
                playerHealth.OnDeath += HandlePlayerDeath;
            }
        }
    }
    
    public void PauseGame()
    {
        if (currentState == GameState.Paused)
        {
            return; // Already paused
        }
        
        isPaused = true;
        SetState(GameState.Paused);
        Time.timeScale = 0f;
        Debug.Log("Game Paused");
    }
    
    public void ResumeGame()
    {
        if (currentState != GameState.Paused)
        {
            return; // Not paused
        }
        
        isPaused = false;
        SetState(GameState.Playing);
        Time.timeScale = 1f;
        Debug.Log("Game Resumed");
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        SetState(GameState.Playing);
        score = 0;
        OnScoreChanged?.Invoke(score);
    }
    
    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Mainmenu");
        SetState(GameState.Menu);
        score = 0;
        OnScoreChanged?.Invoke(score);
    }
    
    public void AddScore(int points)
    {
        score += points;
        OnScoreChanged?.Invoke(score);
        
        // Play score sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayScoreSound();
        }
    }
    
    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
        
        if (sceneName == "Mainmenu")
        {
            SetState(GameState.Menu);
        }
        else
        {
            SetState(GameState.Playing);
        }
    }
    
    public void NextLevel()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentIndex < SceneManager.sceneCountInBuildSettings - 1)
        {
            SceneManager.LoadScene(currentIndex + 1);
            SetState(GameState.Playing);
        }
        else
        {
            // No more levels, go to victory
            SetState(GameState.Victory);
        }
    }
    
    private void HandlePlayerDeath()
    {
        SetState(GameState.GameOver);
        
        // Show game over screen after delay
        Invoke(nameof(ShowGameOver), 2f);
    }
    
    private void ShowGameOver()
    {
        // TODO: Show game over UI
        Debug.Log("Game Over! Final Score: " + score);
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from scene loaded event
        SceneManager.sceneLoaded -= OnSceneLoaded;
        
        if (playerHealth != null)
        {
            playerHealth.OnDeath -= HandlePlayerDeath;
        }
    }
}




