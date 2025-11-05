using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("Health Bar Settings")]
    [SerializeField] private Image healthBarFill;
    [SerializeField] private bool findPlayerAutomatically = true;
    [SerializeField] private GameObject playerObject;
    
    [Header("Animation Settings")]
    [SerializeField] private float lerpSpeed = 5f;
    [SerializeField] private bool smoothTransition = true;
    
    private HealthSystem playerHealth;
    private float targetFillAmount;
    private float currentFillAmount;
    
    private void Start()
    {
        if (findPlayerAutomatically)
        {
            FindAndSubscribeToPlayer();
        }
        else if (playerObject != null)
        {
            SubscribeToPlayer(playerObject);
        }
    }
    
    private void Update()
    {
        // Smoothly lerp the fill amount if smooth transition is enabled
        if (smoothTransition && healthBarFill != null)
        {
            currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, Time.deltaTime * lerpSpeed);
            healthBarFill.fillAmount = currentFillAmount;
        }
    }
    
    private void FindAndSubscribeToPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            SubscribeToPlayer(player);
        }
        else
        {
            Debug.LogWarning("HealthBarUI: Player not found! Make sure the player has the 'Player' tag.");
        }
    }
    
    private void SubscribeToPlayer(GameObject player)
    {
        playerHealth = player.GetComponent<HealthSystem>();
        if (playerHealth != null)
        {
            // Subscribe to health change events
            playerHealth.OnHealthChanged += UpdateHealthBar;
            
            // Initialize the health bar with current health
            UpdateHealthBar(playerHealth.CurrentHealth, playerHealth.MaxHealth);
            
            Debug.Log("HealthBarUI: Successfully subscribed to player health system.");
        }
        else
        {
            Debug.LogError("HealthBarUI: Player does not have a HealthSystem component!");
        }
    }
    
    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (healthBarFill == null)
        {
            Debug.LogWarning("HealthBarUI: Health bar fill image is not assigned!");
            return;
        }
        
        // Calculate the fill amount (0 to 1)
        targetFillAmount = maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;
        
        // If smooth transition is disabled, update immediately
        if (!smoothTransition)
        {
            currentFillAmount = targetFillAmount;
            healthBarFill.fillAmount = targetFillAmount;
        }
        
        // Clamp to ensure it's between 0 and 1
        targetFillAmount = Mathf.Clamp01(targetFillAmount);
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealthBar;
        }
    }
    
    // Public method to manually set the player if needed
    public void SetPlayer(GameObject player)
    {
        if (player != null)
        {
            SubscribeToPlayer(player);
        }
    }
}
