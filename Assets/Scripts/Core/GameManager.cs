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
		
		// Currency]
		private int coins = 0;
    
    // Events
    public event Action<int> OnScoreChanged;
		public event Action<int> OnCoinsChanged;
    public event Action<GameState> OnStateChanged;
    
    private HealthSystem playerHealth;
    private bool isPaused = false;
    
    private int storedPlayerHealth = -1;
    private int storedPlayerMaxHealth = -1;
    private bool pendingPlayerRestore = false;
    private Coroutine scoreRefreshCoroutine;
    private Coroutine healthRefreshCoroutine;
    
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
		public int Coins => coins;
    
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
            InitializePlayerReferences();
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
            InitializePlayerReferences();
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
        pendingPlayerRestore = false;
        storedPlayerHealth = -1;
        storedPlayerMaxHealth = -1;
        
        InitializePlayerReferences();
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
			coins = 0;
			OnCoinsChanged?.Invoke(coins);
    }
    
    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Mainmenu");
        SetState(GameState.Menu);
        score = 0;
        OnScoreChanged?.Invoke(score);
			coins = 0;
			OnCoinsChanged?.Invoke(coins);
    }
    
    public void AddScore(int points)
    {
        score += points;
        OnScoreChanged?.Invoke(score);
        
        if (scoreRefreshCoroutine == null)
        {
            scoreRefreshCoroutine = StartCoroutine(NotifyScoreAfterFrame());
        }

        if (playerHealth != null)
        {
            if (healthRefreshCoroutine == null)
            {
                healthRefreshCoroutine = StartCoroutine(NotifyHealthAfterFrame());
            }
        }
    }
    
    public bool SpendScore(int points)
    {
        if (score >= points)
        {
            score -= points;
            OnScoreChanged?.Invoke(score);
            return true;
        }
        return false;
    }
		
		// Coins API
		public void AddCoins(int amount)
		{
			if (amount <= 0) return;
			coins += amount;
			OnCoinsChanged?.Invoke(coins);
		}
		
		public bool SpendCoins(int amount)
		{
			if (amount <= 0) return true;
			if (coins >= amount)
			{
				coins -= amount;
				OnCoinsChanged?.Invoke(coins);
				return true;
			}
			return false;
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
        Scene activeScene = SceneManager.GetActiveScene();
        string activeSceneName = activeScene.name;

        if (activeSceneName == "Scene2")
        {
            SceneManager.LoadScene("Scene1");
            SetState(GameState.Playing);
            return;
        }

        int currentIndex = activeScene.buildIndex;
        if (currentIndex < SceneManager.sceneCountInBuildSettings - 1)
        {
            SceneManager.LoadScene(currentIndex + 1);
            SetState(GameState.Playing);
        }
        else
        {
            // No more levels, loop back to the first gameplay scene
            SceneManager.LoadScene("Scene1");
            SetState(GameState.Playing);
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
    
    public void HandleBossDeath()
    {
        Debug.Log("GameManager: HandleBossDeath() called");
        SetState(GameState.Victory);

        VictoryScreen victoryScreen = FindFirstObjectByType<VictoryScreen>();
        if (victoryScreen != null)
        {
            Debug.Log("GameManager: Found VictoryScreen, calling ShowVictoryScreen()");
            victoryScreen.OnVictorySequenceComplete -= OnVictoryScreenSequenceComplete;
            victoryScreen.OnVictorySequenceComplete += OnVictoryScreenSequenceComplete;
            victoryScreen.ShowVictoryScreen();
        }
        else
        {
            Debug.LogWarning("GameManager: VictoryScreen not found in scene! Please run Tools > Setup VictoryScreen");
        }
    }

    private void OnVictoryScreenSequenceComplete()
    {
        VictoryScreen victoryScreen = FindFirstObjectByType<VictoryScreen>();
        if (victoryScreen != null)
        {
            victoryScreen.OnVictorySequenceComplete -= OnVictoryScreenSequenceComplete;
        }

        NextLevel();
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
        
        yield return new WaitForSeconds(deathAnimationLength);
        
        // Lock the animator in the death state to prevent any transitions
        // This ensures the death animation doesn't loop or get interrupted
        if (playerAnimator != null)
        {
            // Ensure IsDeath stays true
            playerAnimator.SetBool("IsDeath", true);
            
            // Disable the animator to freeze it in the current state
            // This prevents any state transitions that might cause the death animation to loop
            playerAnimator.enabled = false;
            Debug.Log("GameManager: Animator locked in death state");
        }
        
        Debug.Log("GameManager: finished waiting for death animation - player state is now locked");
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

    private void InitializePlayerReferences()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<HealthSystem>();
            if (playerHealth != null)
            {
                playerHealth.OnDeath -= HandlePlayerDeath;
                playerHealth.OnDeath += HandlePlayerDeath;

                if (pendingPlayerRestore && storedPlayerHealth > 0 && storedPlayerMaxHealth > 0)
                {
                    playerHealth.SetHealthState(storedPlayerHealth, storedPlayerMaxHealth);
                    pendingPlayerRestore = false;
                    storedPlayerHealth = -1;
                    storedPlayerMaxHealth = -1;
                }
            }
        }

        OnScoreChanged?.Invoke(score);
    }

    private IEnumerator NotifyScoreAfterFrame()
    {
        yield return null;
        OnScoreChanged?.Invoke(score);
        scoreRefreshCoroutine = null;
    }

    private IEnumerator NotifyHealthAfterFrame()
    {
        yield return null;
        if (playerHealth != null)
        {
            playerHealth.RefreshHealthEvent();
        }
        healthRefreshCoroutine = null;
    }
}




