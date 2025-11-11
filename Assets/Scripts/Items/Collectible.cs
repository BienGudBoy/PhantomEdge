using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("Collectible Settings")]
    [SerializeField] private CollectibleType type = CollectibleType.Coin;
    [SerializeField] private int value = 10;
    [SerializeField] private float spinSpeed = 180f;
    
    private bool isCollected = false;
    
		private bool IsPlayerCollider(Collider2D col)
		{
			if (col == null) return false;
			// Accept either the collider's object or any parent as the player
			if (col.CompareTag("Player")) return true;
			var pc = col.GetComponentInParent<PlayerController>();
			if (pc != null) return true;
			var hs = col.GetComponentInParent<HealthSystem>();
			return hs != null && col.transform.root.CompareTag("Player");
		}
		
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
    
		// Trigger-based pickup (works when coin collider is trigger)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected) return;
        
			if (IsPlayerCollider(other))
        {
				Collect(other.transform.root.gameObject);
        }
    }
		
		// Collision-based pickup (works when coin collider is NOT a trigger and uses physics bounce)
		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (isCollected) return;
			
			if (collision.collider != null && IsPlayerCollider(collision.collider))
			{
				Collect(collision.collider.transform.root.gameObject);
			}
		}
    
    private void Collect(GameObject player)
    {
        isCollected = true;
        
        switch (type)
        {
            case CollectibleType.Coin:
				// Add coins
				{
					int add = value;
					if (add <= 0) add = 1;
					
					GameManager gm = GameManager.Instance != null ? GameManager.Instance : FindFirstObjectByType<GameManager>();
					if (gm != null)
					{
						gm.AddCoins(add);
					}
					else
					{
						Debug.LogWarning("Collectible: GameManager not found, cannot add coins.");
					}
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
        
        // Play sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayScoreSound();
        }
        
        // Destroy collectible
        Destroy(gameObject);
    }
}

