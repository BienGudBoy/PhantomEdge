using UnityEngine;
using UnityEditor;
using System.Linq;

public class PrefabComponentSetup : EditorWindow
{
    [MenuItem("Tools/Setup All Prefabs (Player + Enemies)")]
    public static void SetupAllPrefabs()
    {
        Debug.Log("=== Starting Prefab Setup ===");
        
        SetupPlayerPrefabs();
        SetupEnemyPrefabs();
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("=== Prefab Setup Complete! ===");
    }
    
    private static void SetupPlayerPrefabs()
    {
        Debug.Log("\n--- Setting up Player Prefabs ---");
        
        string[] playerPaths = new string[]
        {
            "Assets/Prefabs/Player/Ninja.prefab",
            "Assets/Prefabs/Player/char_blue_0.prefab"
        };
        
        foreach (string path in playerPaths)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
            {
                Debug.LogWarning($"Could not find prefab at {path}");
                continue;
            }
            
            Debug.Log($"Setting up: {prefab.name}");
            
            // Add Rigidbody2D
            if (prefab.GetComponent<Rigidbody2D>() == null)
            {
                var rb = prefab.AddComponent<Rigidbody2D>();
                rb.gravityScale = 3f;
                rb.freezeRotation = true;
                rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                Debug.Log($"  ✅ Added Rigidbody2D");
            }
            
            // Add CapsuleCollider2D
            if (prefab.GetComponent<CapsuleCollider2D>() == null)
            {
                var col = prefab.AddComponent<CapsuleCollider2D>();
                col.size = new Vector2(0.5f, 1f);
                col.offset = new Vector2(0f, 0f);
                Debug.Log($"  ✅ Added CapsuleCollider2D");
            }
            
            // Add PlayerController
            if (prefab.GetComponent<PlayerController>() == null)
            {
                prefab.AddComponent<PlayerController>();
                Debug.Log($"  ✅ Added PlayerController");
            }
            
            // Add HealthSystem
            if (prefab.GetComponent<HealthSystem>() == null)
            {
                prefab.AddComponent<HealthSystem>();
                Debug.Log($"  ✅ Added HealthSystem");
            }
            
            // Add PlayerCombat
            if (prefab.GetComponent<PlayerCombat>() == null)
            {
                prefab.AddComponent<PlayerCombat>();
                Debug.Log($"  ✅ Added PlayerCombat");
            }
            
            // Set tag
            if (prefab.tag != "Player")
            {
                prefab.tag = "Player";
                Debug.Log($"  ✅ Set tag to 'Player'");
            }
            
            // Set sorting layer
            var spriteRenderer = prefab.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingLayerName = "Player";
                spriteRenderer.sortingOrder = 100;
                Debug.Log($"  ✅ Set sorting layer to 'Player'");
            }
            
            EditorUtility.SetDirty(prefab);
            Debug.Log($"✅ {prefab.name} setup complete!\n");
        }
    }
    
    private static void SetupEnemyPrefabs()
    {
        Debug.Log("\n--- Setting up Enemy Prefabs ---");
        
        string[] enemyPaths = new string[]
        {
            "Assets/Prefabs/Enemy/Globlin.prefab",
            "Assets/Prefabs/Enemy/Skeleton.prefab",
            "Assets/Prefabs/Enemy/Mushroom.prefab",
            "Assets/Prefabs/Enemy/Flying eye.prefab",
            "Assets/Prefabs/Enemy/Sword.prefab"
        };
        
        foreach (string path in enemyPaths)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
            {
                Debug.LogWarning($"Could not find prefab at {path}");
                continue;
            }
            
            Debug.Log($"Setting up: {prefab.name}");
            
            // Add Rigidbody2D
            if (prefab.GetComponent<Rigidbody2D>() == null)
            {
                var rb = prefab.AddComponent<Rigidbody2D>();
                
                // Flying eye doesn't need gravity
                if (prefab.name.ToLower().Contains("flying") || prefab.name.ToLower().Contains("eye"))
                {
                    rb.gravityScale = 0f;
                    rb.bodyType = RigidbodyType2D.Kinematic;
                    Debug.Log($"  ✅ Added Rigidbody2D (Kinematic, no gravity for flying enemy)");
                }
                else
                {
                    rb.gravityScale = 2f;
                    rb.freezeRotation = true;
                    rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                    Debug.Log($"  ✅ Added Rigidbody2D");
                }
            }
            
            // Add Collider
            if (prefab.GetComponent<Collider2D>() == null)
            {
                var col = prefab.AddComponent<CapsuleCollider2D>();
                col.size = new Vector2(0.5f, 0.8f);
                col.offset = new Vector2(0f, 0f);
                Debug.Log($"  ✅ Added CapsuleCollider2D");
            }
            
            // Add EnemyController
            if (prefab.GetComponent<EnemyController>() == null)
            {
                prefab.AddComponent<EnemyController>();
                Debug.Log($"  ✅ Added EnemyController");
            }
            
            // Add EnemyHealth
            if (prefab.GetComponent<EnemyHealth>() == null)
            {
                prefab.AddComponent<EnemyHealth>();
                Debug.Log($"  ✅ Added EnemyHealth");
            }
            
            // Set tag
            if (prefab.tag != "Enemy")
            {
                prefab.tag = "Enemy";
                Debug.Log($"  ✅ Set tag to 'Enemy'");
            }
            
            // Set sorting layer
            var spriteRenderer = prefab.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingLayerName = "Enemies";
                spriteRenderer.sortingOrder = 50;
                Debug.Log($"  ✅ Set sorting layer to 'Enemies'");
            }
            
            EditorUtility.SetDirty(prefab);
            Debug.Log($"✅ {prefab.name} setup complete!\n");
        }
    }
    
    [MenuItem("Tools/Verify Prefab Setup")]
    public static void VerifyPrefabSetup()
    {
        Debug.Log("=== Verifying Prefab Setup ===\n");
        
        VerifyPlayerPrefabs();
        VerifyEnemyPrefabs();
        
        Debug.Log("\n=== Verification Complete ===");
    }
    
    private static void VerifyPlayerPrefabs()
    {
        Debug.Log("--- Player Prefabs ---");
        
        string[] playerPaths = new string[]
        {
            "Assets/Prefabs/Player/Ninja.prefab",
            "Assets/Prefabs/Player/char_blue_0.prefab"
        };
        
        foreach (string path in playerPaths)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) continue;
            
            Debug.Log($"\n{prefab.name}:");
            Debug.Log($"  Rigidbody2D: {(prefab.GetComponent<Rigidbody2D>() != null ? "✅" : "❌")}");
            Debug.Log($"  Collider2D: {(prefab.GetComponent<Collider2D>() != null ? "✅" : "❌")}");
            Debug.Log($"  PlayerController: {(prefab.GetComponent<PlayerController>() != null ? "✅" : "❌")}");
            Debug.Log($"  HealthSystem: {(prefab.GetComponent<HealthSystem>() != null ? "✅" : "❌")}");
            Debug.Log($"  PlayerCombat: {(prefab.GetComponent<PlayerCombat>() != null ? "✅" : "❌")}");
            Debug.Log($"  Tag: {prefab.tag} {(prefab.tag == "Player" ? "✅" : "❌")}");
            
            var sr = prefab.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Debug.Log($"  Sorting Layer: {sr.sortingLayerName} {(sr.sortingLayerName == "Player" ? "✅" : "❌")}");
            }
        }
    }
    
    private static void VerifyEnemyPrefabs()
    {
        Debug.Log("\n--- Enemy Prefabs ---");
        
        string[] enemyPaths = new string[]
        {
            "Assets/Prefabs/Enemy/Globlin.prefab",
            "Assets/Prefabs/Enemy/Skeleton.prefab",
            "Assets/Prefabs/Enemy/Mushroom.prefab",
            "Assets/Prefabs/Enemy/Flying eye.prefab",
            "Assets/Prefabs/Enemy/Sword.prefab"
        };
        
        foreach (string path in enemyPaths)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) continue;
            
            Debug.Log($"\n{prefab.name}:");
            Debug.Log($"  Rigidbody2D: {(prefab.GetComponent<Rigidbody2D>() != null ? "✅" : "❌")}");
            Debug.Log($"  Collider2D: {(prefab.GetComponent<Collider2D>() != null ? "✅" : "❌")}");
            Debug.Log($"  EnemyController: {(prefab.GetComponent<EnemyController>() != null ? "✅" : "❌")}");
            Debug.Log($"  EnemyHealth: {(prefab.GetComponent<EnemyHealth>() != null ? "✅" : "❌")}");
            Debug.Log($"  Tag: {prefab.tag} {(prefab.tag == "Enemy" ? "✅" : "❌")}");
            
            var sr = prefab.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Debug.Log($"  Sorting Layer: {sr.sortingLayerName} {(sr.sortingLayerName == "Enemies" ? "✅" : "❌")}");
            }
        }
    }
}
