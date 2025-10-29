using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("Collectible Settings")]
    [SerializeField] private CollectibleType type = CollectibleType.Coin;
    [SerializeField] private int value = 10;
    [SerializeField] private float spinSpeed = 180f;
    
    private bool isCollected = false;
    
    public enum CollectibleType
    {
        Coin,
        HealthPotion,
        PowerUp
    }
    
    private void Update()
    {
        // Simple spinning animation
        transform.Rotate(0, 0, spinSpeed * Time.deltaTime);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected) return;
        
        if (other.CompareTag("Player"))
        {
            Collect(other.gameObject);
        }
    }
    
    private void Collect(GameObject player)
    {
        isCollected = true;
        
        switch (type)
        {
            case CollectibleType.Coin:
                // Add score
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddScore(value);
                }
                break;
                
            case CollectibleType.HealthPotion:
                // Heal player
                HealthSystem health = player.GetComponent<HealthSystem>();
                if (health != null)
                {
                    health.Heal(value);
                }
                break;
                
            case CollectibleType.PowerUp:
                // TODO: Apply power-up effect
                Debug.Log("Power-up collected!");
                break;
        }
        
        // Play kosound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayScoreSound();
        }
        
        // Destroy collectible
        Destroy(gameObject);
    }
}

