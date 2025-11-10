using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

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
        Debug.Log("GameManager: HandlePlayerDeath() called");
        SetState(GameState.GameOver);
        
        // Wait for death animation to play before pausing
        StartCoroutine(WaitForDeathAnimationThenPause());
        
        // Show "YOU DIED" screen
        YouDiedScreen deathScreen = FindFirstObjectByType<YouDiedScreen>();
        if (deathScreen != null)
        {
            Debug.Log("GameManager: Found YouDiedScreen, calling ShowDeathScreen()");
            deathScreen.ShowDeathScreen();
        }
        else
        {
            Debug.LogWarning("GameManager: YouDiedScreen not found in scene! Please run Tools > Setup YouDiedScreen");
        }
    }
    
    private IEnumerator WaitForDeathAnimationThenPause()
    {
        // Get the death animation length from the player's animator
        float deathAnimationLength = 1.5f; // Default fallback duration
        Animator playerAnimator = null;
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerAnimator = player.GetComponent<Animator>();
            if (playerAnimator != null && playerAnimator.runtimeAnimatorController != null)
            {
                // Try to find the Death animation clip length
                RuntimeAnimatorController ac = playerAnimator.runtimeAnimatorController;
                foreach (AnimationClip clip in ac.animationClips)
                {
                    if (clip.name == "Death")
                    {
                        deathAnimationLength = clip.length;
                        Debug.Log($"GameManager: Found Death animation, length: {deathAnimationLength}");
                        break;
                    }
                }
            }
        }
        
        // Wait for 90% of the death animation to play (pause slightly before it completes)
        // This prevents the weird transition state
        float waitTime = deathAnimationLength * 0.9f;
        yield return new WaitForSeconds(waitTime);
        
        // Pause the animator first to freeze it in the Death state
        if (playerAnimator != null)
        {
            playerAnimator.enabled = false;
            Debug.Log("GameManager: Animator paused");
        }
        
        // Small delay to ensure animator is frozen
        yield return new WaitForSeconds(0.05f);
        
        // Now pause the game (keep state as GameOver, not Paused)
        Time.timeScale = 0f;
        isPaused = true;
        Debug.Log("GameManager: Game paused after death animation");
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




