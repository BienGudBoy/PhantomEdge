using UnityEngine;

public class Shop : Interactable
{
    [Header("Shop Settings")]
    [SerializeField] protected int cost = 50;
    [SerializeField] protected string shopName = "Shop";
    [SerializeField] protected string shopDescription = "Press E to purchase";
    
    [Header("UI")]
    [SerializeField] protected bool showPurchaseMessage = true;
    
    protected GameObject player;
    
    protected virtual void Start()
    {
		interactPrompt = $"{shopName}\n{shopDescription}\nCost: {cost} Coins";
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
            Debug.LogWarning("Shop: Player not found!");
            return;
        }
        
		// Check currency
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("Shop: GameManager not found!");
            return;
        }
        
        // Try to purchase
        bool purchaseSuccess = false;
        string message = "";
        
		if (GameManager.Instance.Coins < cost)
        {
			message = $"Not enough coins!";
            ShowFloatingText(message, false);
            ShowMessage(message);
            return;
        }
        
        if (TryPurchase())
        {
			// Deduct cost from coins
			if (GameManager.Instance.SpendCoins(cost))
            {
                purchaseSuccess = true;
                message = "Purchase successful!";
                
                // Play purchase sound
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayScoreSound();
                }
            }
            else
            {
				message = "Failed to deduct coins!";
            }
        }
        else
        {
            message = "Purchase failed!";
        }
        
        // Show floating text above the shop
        ShowFloatingText(message, purchaseSuccess);
    }
    
    protected virtual bool TryPurchase()
    {
        // Override in child classes
        return false;
    }
    
    protected virtual void ShowMessage(string message)
    {
        if (showPurchaseMessage)
        {
            Debug.Log($"{shopName}: {message}");
        }
    }
    
    protected virtual void ShowFloatingText(string message, bool isSuccess)
    {
        // Get the shop's position (center of the shop)
        Vector3 shopPosition = transform.position;
        
        // Offset upward to show above the shop
        Vector3 textPosition = shopPosition + Vector3.up * 1.5f;
        
        // Choose color based on success
        Color textColor = isSuccess ? Color.green : Color.red;
        
        // Create floating text
        FloatingText.Create(textPosition, message, textColor, 2f);
        
        // Also log to console
        ShowMessage(message);
    }
}

