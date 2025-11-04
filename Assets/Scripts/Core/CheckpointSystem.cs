using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    public static CheckpointSystem Instance;
    
    [Header("Checkpoint Settings")]
    [SerializeField] private Vector3 currentCheckpoint;
    [SerializeField] private bool hasCheckpoint = false;
    
    [Header("Respawn Settings")]
    [SerializeField] private float respawnDelay = 2f;
    [SerializeField] private int respawnHealthAmount = 100;
    
    private GameObject player;
    private HealthSystem playerHealth;
    
    public Vector3 CurrentCheckpoint => currentCheckpoint;
    public bool HasCheckpoint => hasCheckpoint;
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        FindPlayer();
    }
    
    private void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<HealthSystem>();
            
            // Subscribe to player death
            if (playerHealth != null)
            {
                playerHealth.OnDeath -= OnPlayerDeath;
                playerHealth.OnDeath += OnPlayerDeath;
            }
            
            // Set initial checkpoint if none exists
            if (!hasCheckpoint)
            {
                SetCheckpoint(player.transform.position);
            }
        }
    }
    
    public void SetCheckpoint(Vector3 position)
    {
        currentCheckpoint = position;
        hasCheckpoint = true;
        
        Debug.Log($"Checkpoint set at {position}");
        
        // Play checkpoint sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayScoreSound(); // Using score sound as checkpoint sound
        }
    }
    
    private void OnPlayerDeath()
    {
        if (hasCheckpoint)
        {
            Invoke(nameof(RespawnPlayer), respawnDelay);
        }
    }
    
    private void RespawnPlayer()
    {
        if (player == null)
        {
            FindPlayer();
        }
        
        if (player != null && playerHealth != null)
        {
            // Move player to checkpoint
            player.transform.position = currentCheckpoint;
            
            // Reset player velocity
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
            
            // Revive player
            playerHealth.Revive(respawnHealthAmount);
            
            Debug.Log($"Player respawned at checkpoint: {currentCheckpoint}");
        }
    }
    
    public void ClearCheckpoint()
    {
        hasCheckpoint = false;
        currentCheckpoint = Vector3.zero;
    }
    
    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDeath -= OnPlayerDeath;
        }
    }
}

public class Checkpoint : MonoBehaviour
{
    [Header("Visual Feedback")]
    [SerializeField] private GameObject activeIndicator;
    [SerializeField] private GameObject inactiveIndicator;
    
    private bool isActivated = false;
    
    private void Start()
    {
        UpdateVisuals();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActivated && other.CompareTag("Player"))
        {
            ActivateCheckpoint();
        }
    }
    
    private void ActivateCheckpoint()
    {
        isActivated = true;
        
        if (CheckpointSystem.Instance != null)
        {
            CheckpointSystem.Instance.SetCheckpoint(transform.position);
        }
        
        UpdateVisuals();
    }
    
    private void UpdateVisuals()
    {
        if (activeIndicator != null)
        {
            activeIndicator.SetActive(isActivated);
        }
        
        if (inactiveIndicator != null)
        {
            inactiveIndicator.SetActive(!isActivated);
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = isActivated ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}