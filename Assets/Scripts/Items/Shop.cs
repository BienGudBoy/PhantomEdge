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
        interactPrompt = $"{shopName}\n{shopDescription}\nCost: {cost} Score";
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
        
        // Check if player has enough score
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("Shop: GameManager not found!");
            return;
        }
        
        if (GameManager.Instance.Score < cost)
        {
            ShowMessage($"Not enough score! Need {cost}, have {GameManager.Instance.Score}");
            return;
        }
        
        // Try to purchase
        if (TryPurchase())
        {
            // Deduct cost from score
            if (GameManager.Instance.SpendScore(cost))
            {
                ShowMessage("Purchase successful!");
                
                // Play purchase sound
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayScoreSound();
                }
            }
            else
            {
                ShowMessage("Failed to deduct score!");
            }
        }
        else
        {
            ShowMessage("Purchase failed!");
        }
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
            // TODO: Show UI message
        }
    }
}

