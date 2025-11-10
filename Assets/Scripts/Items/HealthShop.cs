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
            ShowMessage("You're already at full health!");
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
}

