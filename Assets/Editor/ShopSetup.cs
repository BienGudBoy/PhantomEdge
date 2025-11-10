using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class ShopSetup : EditorWindow
{
    [MenuItem("Tools/Setup Shops in Scene1")]
    public static void SetupShops()
    {
        // Load Scene1 if not already loaded
        if (EditorSceneManager.GetActiveScene().name != "Scene1")
        {
            if (EditorUtility.DisplayDialog("Load Scene1?", 
                "Scene1 needs to be loaded to setup shops. Load it now?", 
                "Yes", "Cancel"))
            {
                EditorSceneManager.OpenScene("Assets/Scenes/Scene1.unity");
            }
            else
            {
                Debug.LogWarning("Shop setup cancelled - Scene1 not loaded.");
                return;
            }
        }
        
        // Find all shop GameObjects in the scene
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        GameObject[] shops = System.Array.FindAll(allObjects, obj => 
            obj.name.Contains("shop") || obj.name.Contains("Shop"));
        
        if (shops.Length < 2)
        {
            Debug.LogWarning($"Found {shops.Length} shop(s) in scene. Need at least 2 shops. " +
                "Please ensure there are 2 shop GameObjects in Scene1.");
            return;
        }
        
        Debug.Log($"Found {shops.Length} shop(s) in scene. Setting up first 2...");
        
        // Setup first shop as Health Shop
        SetupHealthShop(shops[0]);
        
        // Setup second shop as Combat Shop
        SetupCombatShop(shops[1]);
        
        // Mark scene as dirty
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        
        Debug.Log("Shop setup complete!");
        Debug.Log($"Health Shop: {shops[0].name}");
        Debug.Log($"Combat Shop: {shops[1].name}");
    }
    
    private static void SetupHealthShop(GameObject shopGO)
    {
        // Remove existing shop components if any
        Shop existingShop = shopGO.GetComponent<Shop>();
        if (existingShop != null)
        {
            DestroyImmediate(existingShop);
        }
        
        HealthShop existingHealthShop = shopGO.GetComponent<HealthShop>();
        if (existingHealthShop != null)
        {
            DestroyImmediate(existingHealthShop);
        }
        
        CombatShop existingCombatShop = shopGO.GetComponent<CombatShop>();
        if (existingCombatShop != null)
        {
            DestroyImmediate(existingCombatShop);
        }
        
        // Add HealthShop component
        HealthShop healthShop = shopGO.AddComponent<HealthShop>();
        
        // Configure HealthShop using SerializedObject
        SerializedObject serializedShop = new SerializedObject(healthShop);
        
        // Set cost (50 score)
        SerializedProperty costProp = serializedShop.FindProperty("cost");
        if (costProp != null) costProp.intValue = 50;
        
        // Set heal amount (0 = heal to full)
        SerializedProperty healAmountProp = serializedShop.FindProperty("healAmount");
        if (healAmountProp != null) healAmountProp.intValue = 0;
        
        serializedShop.ApplyModifiedProperties();
        
        // Setup collider for interaction (HealthShop extends Interactable, so no need to add it separately)
        SetupShopCollider(shopGO);
        
        // Rename for clarity
        shopGO.name = "HealthShop";
        
        EditorUtility.SetDirty(healthShop);
        EditorUtility.SetDirty(shopGO);
    }
    
    private static void SetupCombatShop(GameObject shopGO)
    {
        // Remove existing shop components if any
        Shop existingShop = shopGO.GetComponent<Shop>();
        if (existingShop != null)
        {
            DestroyImmediate(existingShop);
        }
        
        HealthShop existingHealthShop = shopGO.GetComponent<HealthShop>();
        if (existingHealthShop != null)
        {
            DestroyImmediate(existingHealthShop);
        }
        
        CombatShop existingCombatShop = shopGO.GetComponent<CombatShop>();
        if (existingCombatShop != null)
        {
            DestroyImmediate(existingCombatShop);
        }
        
        // Add CombatShop component
        CombatShop combatShop = shopGO.AddComponent<CombatShop>();
        
        // Configure CombatShop using SerializedObject
        SerializedObject serializedShop = new SerializedObject(combatShop);
        
        // Set cost (50 score)
        SerializedProperty costProp = serializedShop.FindProperty("cost");
        if (costProp != null) costProp.intValue = 50;
        
        // Set damage increase (5 per upgrade)
        SerializedProperty damageIncreaseProp = serializedShop.FindProperty("damageIncrease");
        if (damageIncreaseProp != null) damageIncreaseProp.intValue = 5;
        
        // Set max upgrades (3)
        SerializedProperty maxUpgradesProp = serializedShop.FindProperty("maxUpgrades");
        if (maxUpgradesProp != null) maxUpgradesProp.intValue = 3;
        
        serializedShop.ApplyModifiedProperties();
        
        // Setup collider for interaction (CombatShop extends Interactable, so no need to add it separately)
        SetupShopCollider(shopGO);
        
        // Rename for clarity
        shopGO.name = "CombatShop";
        
        EditorUtility.SetDirty(combatShop);
        EditorUtility.SetDirty(shopGO);
    }
    
    private static void SetupShopCollider(GameObject shopGO)
    {
        // Check if collider already exists
        Collider2D existingCollider = shopGO.GetComponent<Collider2D>();
        
        if (existingCollider == null)
        {
            // Add BoxCollider2D
            BoxCollider2D collider = shopGO.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            
            // Set size to reasonable interaction range
            collider.size = new Vector2(2f, 2f);
            
            EditorUtility.SetDirty(shopGO);
        }
        else
        {
            // Ensure existing collider is a trigger
            existingCollider.isTrigger = true;
            EditorUtility.SetDirty(existingCollider);
        }
    }
}

