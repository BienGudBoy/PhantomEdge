using UnityEngine;

public class CombatShop : Shop
{
    [Header("Combat Shop Settings")]
    [SerializeField] private int damageIncrease = 5;
    [SerializeField] private int maxUpgrades = 3; // Maximum number of times this can be purchased
    
    private static int upgradeCount = 0; // Track upgrades across all CombatShops
    
    
    protected override bool TryPurchase()
    {
        if (player == null) return false;
        
        // Check if max upgrades reached
        if (upgradeCount >= maxUpgrades)
        {
            // This will be handled by the parent Shop class's ShowFloatingText
            return false;
        }
        
        PlayerCombat combat = player.GetComponent<PlayerCombat>();
        if (combat == null)
        {
            Debug.LogWarning("CombatShop: Player has no PlayerCombat!");
            return false;
        }
        
        // Increase damage
        combat.IncreaseDamage(damageIncrease);
        upgradeCount++;
        
        // Update prompt for next purchase
        UpdatePrompt();
        
        // Refresh the prompt if player is still in range
        if (canInteract)
        {
            ShowPrompt();
        }
        
        return true;
    }
    
    protected override void ShowFloatingText(string message, bool isSuccess)
    {
        // Override to show custom message for combat shop
        if (!isSuccess && message == "Purchase failed!")
        {
            message = $"Max upgrades reached!\n({upgradeCount}/{maxUpgrades})";
        }
        else if (isSuccess)
        {
            // Show damage increase info
            if (player != null)
            {
                PlayerCombat combat = player.GetComponent<PlayerCombat>();
                if (combat != null)
                {
                    message = $"Damage +{damageIncrease}!";
                }
            }
        }
        base.ShowFloatingText(message, isSuccess);
    }
    
    private void UpdatePrompt()
    {
        if (upgradeCount >= maxUpgrades)
        {
            interactPrompt = $"{shopName}\nMax upgrades reached!";
        }
        else
        {
            interactPrompt = $"{shopName}\n{shopDescription}\nCost: {cost} Score\nUpgrades: {upgradeCount}/{maxUpgrades}";
        }
    }
    
    protected override void Start()
    {
        shopName = "Combat Shop";
        shopDescription = $"Increase attack damage by {damageIncrease}";
        base.Start();
        
        // Update description if max upgrades reached
        UpdatePrompt();
    }
    
    // Reset upgrade count (useful for new game/level)
    public static void ResetUpgrades()
    {
        upgradeCount = 0;
    }
}

