using UnityEngine;

public class HealthShop : Shop
{
    [Header("Health Shop Settings")]
    [SerializeField] private int healAmount = 100; // 0 = heal to full
    
    protected override void Start()
    {
		shopName = "Health Shop";
		shopDescription = "Restore your health";
        base.Start();
    }
    
    protected override bool TryPurchase()
    {
        if (player == null) return false;
        
        HealthSystem health = player.GetComponent<HealthSystem>();
        if (health == null)
        {
            Debug.LogWarning("HealthShop: Player has no HealthSystem!");
            return false;
        }
        
        // Check if player needs healing
        if (health.CurrentHealth >= health.MaxHealth)
        {
            // This will be handled by the parent Shop class's ShowFloatingText
            return false;
        }
        
        // Heal player
        if (healAmount <= 0)
        {
            // Heal to full
            health.Heal(health.MaxHealth - health.CurrentHealth);
        }
        else
        {
            health.Heal(healAmount);
        }
        
        return true;
    }
    
    protected override void ShowFloatingText(string message, bool isSuccess)
    {
        // Override to show custom message for health shop
        if (!isSuccess && message == "Purchase failed!")
        {
            message = "Already at full health!";
        }
        base.ShowFloatingText(message, isSuccess);
    }
}

