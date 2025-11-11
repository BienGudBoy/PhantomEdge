using UnityEngine;

public class CombatShop : Shop
{
    [Header("Combat Shop Settings")]
	[SerializeField] private CombatShopUI shopUI;
    
    protected override void Start()
    {
		shopName = "Combat Shop";
		shopDescription = "Press E to open upgrade shop";
        base.Start();
        
        // Find shop UI if not assigned
        if (shopUI == null)
        {
            shopUI = FindFirstObjectByType<CombatShopUI>();
            if (shopUI == null)
            {
                Debug.LogWarning("CombatShop: CombatShopUI not found! Please assign it in the inspector or ensure it exists in the scene.");
            }
        }
    }
    
    public override void Interact()
    {
        if (!canInteract) return;
        
        // Find player if not already found
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        
        if (player == null)
        {
            Debug.LogWarning("CombatShop: Player not found!");
            return;
        }
        
        // Open the shop UI instead of direct purchase
        if (shopUI != null)
        {
            shopUI.ShowShop();
        }
        else
        {
            Debug.LogWarning("CombatShop: Shop UI not available!");
        }
    }
    
    protected override bool TryPurchase()
    {
        // This method is no longer used, but kept for compatibility
        return false;
    }
    
    // Reset upgrade count (useful for new game/level)
    public static void ResetUpgrades()
    {
        CombatShopUI.ResetAllUpgrades();
    }
}

