using UnityEngine;
using UnityEditor;

public class GroundColliderSetup : EditorWindow
{
    [MenuItem("Tools/Fix Ground Colliders")]
    public static void SetupGroundColliders()
    {
        Debug.Log("=== Setting up Ground Colliders ===");
        
        int fixedCount = 0;
        
        // Find all SpriteRenderers in scene
        SpriteRenderer[] allSprites = FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        
        foreach (SpriteRenderer sprite in allSprites)
        {
            GameObject obj = sprite.gameObject;
            string objName = obj.name.ToLower();
            string parentName = obj.transform.parent ? obj.transform.parent.name.ToLower() : "";
            
            // Check if this is a ground/platform object
            bool isGround = objName.Contains("ground") || 
                          objName.Contains("platform") ||
                          objName.Contains("floor") ||
                          parentName.Contains("ground");
            
            // Also check sorting layer
            bool isGroundLayer = sprite.sortingLayerName == "Ground";
            
            if (isGround || isGroundLayer)
            {
                // Check if it already has a collider
                if (obj.GetComponent<Collider2D>() == null)
                {
                    // Add BoxCollider2D
                    BoxCollider2D collider = obj.AddComponent<BoxCollider2D>();
                    
                    // Auto-size based on sprite
                    if (sprite.sprite != null)
                    {
                        Bounds bounds = sprite.sprite.bounds;
                        collider.size = bounds.size;
                        collider.offset = bounds.center;
                    }
                    
                    Debug.Log($"✅ Added collider to: {obj.name}");
                    fixedCount++;
                    
                    EditorUtility.SetDirty(obj);
                }
            }
        }
        
        Debug.Log($"\n=== Fixed {fixedCount} ground objects ===");
        
        if (fixedCount == 0)
        {
            Debug.LogWarning("⚠️ No ground objects found! Make sure ground objects:");
            Debug.LogWarning("  - Have 'ground' or 'platform' in their name");
            Debug.LogWarning("  - OR are on the 'Ground' sorting layer");
        }
    }
    
    [MenuItem("Tools/Create Ground Platform")]
    public static void CreateGroundPlatform()
    {
        // Create a simple ground platform for testing
        GameObject ground = new GameObject("Ground");
        
        // Add sprite renderer
        SpriteRenderer sr = ground.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = "Ground";
        sr.color = new Color(0.4f, 0.3f, 0.2f); // Brown color
        
        // Create a simple square sprite
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
        sr.sprite = sprite;
        
        // Scale it to be platform-sized
        ground.transform.localScale = new Vector3(50, 2, 1);
        ground.transform.position = new Vector3(0, -4, 0);
        
        // Add collider
        BoxCollider2D collider = ground.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(1, 1);
        
        Debug.Log("✅ Created test ground platform at (0, -4, 0)");
        Debug.Log("   Scale: (50, 2, 1) - adjust as needed");
        
        Selection.activeGameObject = ground;
    }
    
    [MenuItem("Tools/Fix All Scene Physics")]
    public static void FixAllScenePhysics()
    {
        Debug.Log("=== Fixing All Scene Physics ===\n");
        
        // 1. Fix ground colliders
        Debug.Log("Step 1: Adding ground colliders...");
        SetupGroundColliders();
        
        // 2. Verify player physics
        Debug.Log("\nStep 2: Verifying player physics...");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                if (rb.bodyType == RigidbodyType2D.Kinematic)
                {
                    rb.bodyType = RigidbodyType2D.Dynamic;
                    Debug.Log("✅ Fixed player Rigidbody2D - set to Dynamic");
                }
                if (rb.gravityScale == 0)
                {
                    rb.gravityScale = 3f;
                    Debug.Log("✅ Fixed player gravity scale - set to 3");
                }
                Debug.Log($"✅ Player physics OK: Type={rb.bodyType}, Gravity={rb.gravityScale}");
            }
            else
            {
                Debug.LogError("❌ Player has no Rigidbody2D!");
            }
            
            Collider2D col = player.GetComponent<Collider2D>();
            if (col == null)
            {
                Debug.LogError("❌ Player has no Collider2D!");
            }
            else
            {
                Debug.Log($"✅ Player collider OK: {col.GetType().Name}");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ No player found in scene (tag: 'Player')");
        }
        
        // 3. Check collision matrix
        Debug.Log("\nStep 3: Checking Physics2D collision matrix...");
        int playerLayer = LayerMask.NameToLayer("Default");
        int groundLayer = LayerMask.NameToLayer("Default");
        
        if (Physics2D.GetIgnoreLayerCollision(playerLayer, groundLayer))
        {
            Debug.LogWarning("⚠️ Player and Ground layers are set to ignore collision!");
            Debug.LogWarning("   Fix in: Edit > Project Settings > Physics 2D > Layer Collision Matrix");
        }
        else
        {
            Debug.Log("✅ Collision matrix looks good");
        }
        
        Debug.Log("\n=== Physics Fix Complete ===");
    }
    
    [MenuItem("Tools/List All Ground Objects")]
    public static void ListGroundObjects()
    {
        Debug.Log("=== Ground Objects in Scene ===\n");
        
        SpriteRenderer[] allSprites = FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        int count = 0;
        
        foreach (SpriteRenderer sprite in allSprites)
        {
            GameObject obj = sprite.gameObject;
            string objName = obj.name.ToLower();
            string parentName = obj.transform.parent ? obj.transform.parent.name.ToLower() : "";
            
            bool isGround = objName.Contains("ground") || 
                          objName.Contains("platform") ||
                          sprite.sortingLayerName == "Ground";
            
            if (isGround)
            {
                Collider2D col = obj.GetComponent<Collider2D>();
                string status = col != null ? "✅ Has Collider" : "❌ NO COLLIDER";
                
                Debug.Log($"{status} - {obj.name} (Sorting: {sprite.sortingLayerName})");
                count++;
            }
        }
        
        Debug.Log($"\n=== Found {count} ground objects ===");
    }
}
