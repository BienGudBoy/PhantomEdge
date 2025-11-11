using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatShopUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private TextMeshProUGUI chooseUpgradeText;
    
    [Header("Vital Boost")]
    [SerializeField] private Button vitalBoostBuyButton;
    [SerializeField] private TextMeshProUGUI vitalBoostAmountText;
    [SerializeField] private TextMeshProUGUI vitalBoostCostText;
    [SerializeField] private Button vitalBoostIncButton;
    [SerializeField] private Button vitalBoostDecButton;
    
    [Header("Speed Surge")]
    [SerializeField] private Button speedSurgeBuyButton;
    [SerializeField] private TextMeshProUGUI speedSurgeAmountText;
    [SerializeField] private TextMeshProUGUI speedSurgeCostText;
    [SerializeField] private Button speedSurgeIncButton;
    [SerializeField] private Button speedSurgeDecButton;
    
    [Header("Blade Empower")]
    [SerializeField] private Button bladeEmpowerBuyButton;
    [SerializeField] private TextMeshProUGUI bladeEmpowerAmountText;
    [SerializeField] private TextMeshProUGUI bladeEmpowerCostText;
    [SerializeField] private Button bladeEmpowerIncButton;
    [SerializeField] private Button bladeEmpowerDecButton;
    
    [Header("Upgrade Settings")]
    [SerializeField] private int vitalBoostBaseCost = 20;
    [SerializeField] private int vitalBoostCostIncrease = 10;
    [SerializeField] private int[] vitalBoostAmounts = { 20, 30, 40 }; // HP amounts
    
    [SerializeField] private int speedSurgeBaseCost = 25;
    [SerializeField] private int speedSurgeCostIncrease = 15;
    [SerializeField] private float[] speedSurgeAmounts = { 0.5f, 0.75f, 1.0f }; // Speed amounts
    
    [SerializeField] private int bladeEmpowerBaseCost = 30;
    [SerializeField] private int bladeEmpowerCostIncrease = 20;
    [SerializeField] private int[] bladeEmpowerDamageAmounts = { 2, 3, 4 }; // Damage amounts
    [SerializeField] private float[] bladeEmpowerSpeedAmounts = { 0.05f, 0.08f, 0.10f }; // Attack duration reduction
    
    // Purchase counts (track how many times each upgrade was bought)
    private int vitalBoostPurchaseCount = 0;
    private int speedSurgePurchaseCount = 0;
    private int bladeEmpowerPurchaseCount = 0;
    
    // Current selected option indices (0, 1, or 2)
    private int vitalBoostOptionIndex = 0;
    private int speedSurgeOptionIndex = 0;
    private int bladeEmpowerOptionIndex = 0;
    
    private GameObject player;
    
    private void Awake()
    {
        // Find player
        player = GameObject.FindGameObjectWithTag("Player");
        
        // Auto-find shop panel if not assigned
        if (shopPanel == null)
        {
            shopPanel = gameObject;
        }
        
        // Auto-find UI elements if not assigned (search by name in children)
        if (shopPanel != null)
        {
            if (chooseUpgradeText == null)
                chooseUpgradeText = shopPanel.transform.Find("ChooseUpgradeText")?.GetComponent<TextMeshProUGUI>();
            
            // Vital Boost
            Transform vitalBoost = shopPanel.transform.Find("VitalBoost");
            if (vitalBoost != null)
            {
                if (vitalBoostBuyButton == null)
                    vitalBoostBuyButton = vitalBoost.Find("BuyButton")?.GetComponent<Button>();
                if (vitalBoostAmountText == null)
                    vitalBoostAmountText = vitalBoost.Find("AmountText")?.GetComponent<TextMeshProUGUI>();
                if (vitalBoostCostText == null)
                    vitalBoostCostText = vitalBoost.Find("CostText")?.GetComponent<TextMeshProUGUI>();
                if (vitalBoostIncButton == null)
                    vitalBoostIncButton = vitalBoost.Find("IncAmountButton")?.GetComponent<Button>();
                if (vitalBoostDecButton == null)
                    vitalBoostDecButton = vitalBoost.Find("DecAmountButton")?.GetComponent<Button>();
            }
            
            // Speed Surge
            Transform speedSurge = shopPanel.transform.Find("SpeedSurge");
            if (speedSurge != null)
            {
                if (speedSurgeBuyButton == null)
                    speedSurgeBuyButton = speedSurge.Find("BuyButton")?.GetComponent<Button>();
                if (speedSurgeAmountText == null)
                    speedSurgeAmountText = speedSurge.Find("AmountText")?.GetComponent<TextMeshProUGUI>();
                if (speedSurgeCostText == null)
                    speedSurgeCostText = speedSurge.Find("CostText")?.GetComponent<TextMeshProUGUI>();
                if (speedSurgeIncButton == null)
                    speedSurgeIncButton = speedSurge.Find("IncAmountButton")?.GetComponent<Button>();
                if (speedSurgeDecButton == null)
                    speedSurgeDecButton = speedSurge.Find("DecAmountButton")?.GetComponent<Button>();
            }
            
            // Blade Empower
            Transform bladeEmpower = shopPanel.transform.Find("BladeEmpower");
            if (bladeEmpower != null)
            {
                if (bladeEmpowerBuyButton == null)
                    bladeEmpowerBuyButton = bladeEmpower.Find("BuyButton")?.GetComponent<Button>();
                if (bladeEmpowerAmountText == null)
                    bladeEmpowerAmountText = bladeEmpower.Find("AmountText")?.GetComponent<TextMeshProUGUI>();
                if (bladeEmpowerCostText == null)
                    bladeEmpowerCostText = bladeEmpower.Find("CostText")?.GetComponent<TextMeshProUGUI>();
                if (bladeEmpowerIncButton == null)
                    bladeEmpowerIncButton = bladeEmpower.Find("IncAmountButton")?.GetComponent<Button>();
                if (bladeEmpowerDecButton == null)
                    bladeEmpowerDecButton = bladeEmpower.Find("DecAmountButton")?.GetComponent<Button>();
            }
        }
        
        // Setup button listeners
        if (vitalBoostBuyButton != null)
            vitalBoostBuyButton.onClick.AddListener(OnVitalBoostBuy);
        if (vitalBoostIncButton != null)
            vitalBoostIncButton.onClick.AddListener(() => ChangeVitalBoostOption(1));
        if (vitalBoostDecButton != null)
            vitalBoostDecButton.onClick.AddListener(() => ChangeVitalBoostOption(-1));
        
        if (speedSurgeBuyButton != null)
            speedSurgeBuyButton.onClick.AddListener(OnSpeedSurgeBuy);
        if (speedSurgeIncButton != null)
            speedSurgeIncButton.onClick.AddListener(() => ChangeSpeedSurgeOption(1));
        if (speedSurgeDecButton != null)
            speedSurgeDecButton.onClick.AddListener(() => ChangeSpeedSurgeOption(-1));
        
        if (bladeEmpowerBuyButton != null)
            bladeEmpowerBuyButton.onClick.AddListener(OnBladeEmpowerBuy);
        if (bladeEmpowerIncButton != null)
            bladeEmpowerIncButton.onClick.AddListener(() => ChangeBladeEmpowerOption(1));
        if (bladeEmpowerDecButton != null)
            bladeEmpowerDecButton.onClick.AddListener(() => ChangeBladeEmpowerOption(-1));
    }
    
    private void Start()
    {
        // Restore purchase counts from GameManager (persistent across scenes)
        if (GameManager.Instance != null)
        {
            vitalBoostPurchaseCount = GameManager.Instance.GetVitalBoostPurchaseCount();
            speedSurgePurchaseCount = GameManager.Instance.GetSpeedSurgePurchaseCount();
            bladeEmpowerPurchaseCount = GameManager.Instance.GetBladeEmpowerPurchaseCount();
        }
        
        UpdateAllUI();
        
        // Subscribe to coin changes to update button states
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCoinsChanged += OnCoinsChanged;
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from coin changes
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCoinsChanged -= OnCoinsChanged;
        }
    }
    
    private void OnCoinsChanged(int newCoinAmount)
    {
        // Update button interactability when coins change
        UpdateAllUI();
    }
    
    private bool isShopOpen = false;
    private static bool escapeHandledThisFrame = false;
    
    public void ShowShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
            isShopOpen = true;
            UpdateAllUI();
            
            // Pause time without changing game state (to avoid triggering pause menu)
            Time.timeScale = 0f;
        }
    }
    
    public void HideShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
            isShopOpen = false;
            
            // Resume time
            Time.timeScale = 1f;
        }
    }
    
    public bool IsShopOpen()
    {
        return isShopOpen;
    }
    
    public bool IsShopPanelActive()
    {
        return shopPanel != null && shopPanel.activeSelf;
    }
    
    public static bool WasEscapeHandledThisFrame()
    {
        return escapeHandledThisFrame;
    }
    
    private void UpdateAllUI()
    {
        UpdateVitalBoostUI();
        UpdateSpeedSurgeUI();
        UpdateBladeEmpowerUI();
    }
    
    // Vital Boost Methods
    private void ChangeVitalBoostOption(int direction)
    {
        vitalBoostOptionIndex = Mathf.Clamp(vitalBoostOptionIndex + direction, 0, vitalBoostAmounts.Length - 1);
        UpdateVitalBoostUI();
    }
    
    private void UpdateVitalBoostUI()
    {
        if (vitalBoostAmountText != null)
        {
            vitalBoostAmountText.text = $"+{vitalBoostAmounts[vitalBoostOptionIndex]} HP";
        }
        
        int currentCost = vitalBoostBaseCost + (vitalBoostPurchaseCount * vitalBoostCostIncrease);
        if (vitalBoostCostText != null)
        {
            vitalBoostCostText.text = currentCost.ToString();
        }
        
        // Update button interactability based on coins
        if (vitalBoostBuyButton != null && GameManager.Instance != null)
        {
            vitalBoostBuyButton.interactable = GameManager.Instance.Coins >= currentCost;
        }
    }
    
    private void OnVitalBoostBuy()
    {
        if (player == null) return;
        if (GameManager.Instance == null) return;
        
        int currentCost = vitalBoostBaseCost + (vitalBoostPurchaseCount * vitalBoostCostIncrease);
        
        if (GameManager.Instance.Coins < currentCost)
        {
            ShowMessage("Not enough coins!", false);
            return;
        }
        
        HealthSystem health = player.GetComponent<HealthSystem>();
        if (health == null)
        {
            Debug.LogWarning("CombatShopUI: Player has no HealthSystem!");
            return;
        }
        
        int hpIncrease = vitalBoostAmounts[vitalBoostOptionIndex];
        int newMaxHealth = health.MaxHealth + hpIncrease;
        health.SetMaxHealth(newMaxHealth);
        
        // Deduct coins
        GameManager.Instance.SpendCoins(currentCost);
        vitalBoostPurchaseCount++;
        
        // Save to GameManager for persistence
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddHPUpgrade(hpIncrease);
            GameManager.Instance.SetVitalBoostPurchaseCount(vitalBoostPurchaseCount);
        }
        
        ShowMessage($"Vital Boost purchased! +{hpIncrease} HP", true);
        UpdateVitalBoostUI();
        
        // Play sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayScoreSound();
        }
    }
    
    // Speed Surge Methods
    private void ChangeSpeedSurgeOption(int direction)
    {
        speedSurgeOptionIndex = Mathf.Clamp(speedSurgeOptionIndex + direction, 0, speedSurgeAmounts.Length - 1);
        UpdateSpeedSurgeUI();
    }
    
    private void UpdateSpeedSurgeUI()
    {
        if (speedSurgeAmountText != null)
        {
            speedSurgeAmountText.text = $"+{speedSurgeAmounts[speedSurgeOptionIndex]:F2} Speed";
        }
        
        int currentCost = speedSurgeBaseCost + (speedSurgePurchaseCount * speedSurgeCostIncrease);
        if (speedSurgeCostText != null)
        {
            speedSurgeCostText.text = currentCost.ToString();
        }
        
        // Update button interactability
        if (speedSurgeBuyButton != null && GameManager.Instance != null)
        {
            speedSurgeBuyButton.interactable = GameManager.Instance.Coins >= currentCost;
        }
    }
    
    private void OnSpeedSurgeBuy()
    {
        if (player == null) return;
        if (GameManager.Instance == null) return;
        
        int currentCost = speedSurgeBaseCost + (speedSurgePurchaseCount * speedSurgeCostIncrease);
        
        if (GameManager.Instance.Coins < currentCost)
        {
            ShowMessage("Not enough coins!", false);
            return;
        }
        
        PlayerController controller = player.GetComponent<PlayerController>();
        if (controller == null)
        {
            Debug.LogWarning("CombatShopUI: Player has no PlayerController!");
            return;
        }
        
        float speedIncrease = speedSurgeAmounts[speedSurgeOptionIndex];
        controller.IncreaseMoveSpeed(speedIncrease);
        
        // Deduct coins
        GameManager.Instance.SpendCoins(currentCost);
        speedSurgePurchaseCount++;
        
        // Save to GameManager for persistence
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddSpeedUpgrade(speedIncrease);
            GameManager.Instance.SetSpeedSurgePurchaseCount(speedSurgePurchaseCount);
        }
        
        ShowMessage($"Speed Surge purchased! +{speedIncrease:F2} Speed", true);
        UpdateSpeedSurgeUI();
        
        // Play sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayScoreSound();
        }
    }
    
    // Blade Empower Methods
    private void ChangeBladeEmpowerOption(int direction)
    {
        bladeEmpowerOptionIndex = Mathf.Clamp(bladeEmpowerOptionIndex + direction, 0, bladeEmpowerDamageAmounts.Length - 1);
        UpdateBladeEmpowerUI();
    }
    
    private void UpdateBladeEmpowerUI()
    {
        if (bladeEmpowerAmountText != null)
        {
            int damage = bladeEmpowerDamageAmounts[bladeEmpowerOptionIndex];
            float speedReduction = bladeEmpowerSpeedAmounts[bladeEmpowerOptionIndex];
            bladeEmpowerAmountText.text = $"+{damage} DMG, -{speedReduction:F2}s ATK";
        }
        
        int currentCost = bladeEmpowerBaseCost + (bladeEmpowerPurchaseCount * bladeEmpowerCostIncrease);
        if (bladeEmpowerCostText != null)
        {
            bladeEmpowerCostText.text = currentCost.ToString();
        }
        
        // Update button interactability
        if (bladeEmpowerBuyButton != null && GameManager.Instance != null)
        {
            bladeEmpowerBuyButton.interactable = GameManager.Instance.Coins >= currentCost;
        }
    }
    
    private void OnBladeEmpowerBuy()
    {
        if (player == null) return;
        if (GameManager.Instance == null) return;
        
        int currentCost = bladeEmpowerBaseCost + (bladeEmpowerPurchaseCount * bladeEmpowerCostIncrease);
        
        if (GameManager.Instance.Coins < currentCost)
        {
            ShowMessage("Not enough coins!", false);
            return;
        }
        
        PlayerCombat combat = player.GetComponent<PlayerCombat>();
        if (combat == null)
        {
            Debug.LogWarning("CombatShopUI: Player has no PlayerCombat!");
            return;
        }
        
        int damageIncrease = bladeEmpowerDamageAmounts[bladeEmpowerOptionIndex];
        float speedReduction = bladeEmpowerSpeedAmounts[bladeEmpowerOptionIndex];
        
        combat.IncreaseDamage(damageIncrease);
        combat.ReduceAttackDuration(speedReduction);
        
        // Deduct coins
        GameManager.Instance.SpendCoins(currentCost);
        bladeEmpowerPurchaseCount++;
        
        // Save to GameManager for persistence
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddDamageUpgrade(damageIncrease);
            GameManager.Instance.AddAttackSpeedUpgrade(speedReduction);
            GameManager.Instance.SetBladeEmpowerPurchaseCount(bladeEmpowerPurchaseCount);
        }
        
        ShowMessage($"Blade Empower purchased! +{damageIncrease} DMG, -{speedReduction:F2}s ATK", true);
        UpdateBladeEmpowerUI();
        
        // Play sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayScoreSound();
        }
    }
    
    private void ShowMessage(string message, bool isSuccess)
    {
        Debug.Log($"[CombatShopUI] {message}");
        // Could also show floating text here if needed
    }
    
    // Reset all purchase counts (useful for new game/level)
    public static void ResetAllUpgrades()
    {
        CombatShopUI instance = FindFirstObjectByType<CombatShopUI>();
        if (instance != null)
        {
            instance.vitalBoostPurchaseCount = 0;
            instance.speedSurgePurchaseCount = 0;
            instance.bladeEmpowerPurchaseCount = 0;
            instance.vitalBoostOptionIndex = 0;
            instance.speedSurgeOptionIndex = 0;
            instance.bladeEmpowerOptionIndex = 0;
            instance.UpdateAllUI();
        }
        
        // Also reset in GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetAllUpgrades();
        }
    }
    
    private void LateUpdate()
    {
        // Reset the flag at the end of each frame (after all Updates have run)
        escapeHandledThisFrame = false;
    }
    
    private void Update()
    {
        // Close shop with Escape key
        if (shopPanel != null && shopPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                escapeHandledThisFrame = true;
                HideShop();
            }
        }
    }
}

