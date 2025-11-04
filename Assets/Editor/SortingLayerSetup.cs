using UnityEngine;
using UnityEditor;

public class SortingLayerSetup : EditorWindow
{
    [MenuItem("Tools/Fix Sorting Layers")]
    public static void SetupSortingLayers()
    {
        // Create sorting layers if they don't exist
        CreateSortingLayers();
        
        // Fix all sprites in current scene
        FixSpritesInScene();
        
        Debug.Log("Sorting layers setup complete!");
    }
    
    private static void CreateSortingLayers()
    {
        // Get SerializedObject for TagManager
        SerializedObject tagManager = new SerializedObject(
            AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        
        SerializedProperty sortingLayers = tagManager.FindProperty("m_SortingLayers");
        
        // Define desired layers in render order (back to front)
        string[] desiredLayers = new string[]
        {
            "Default",
            "Background",      // Far background elements
            "Midground",       // Middle layer decorations
            "Ground",          // Ground/platforms
            "Enemies",         // Enemy characters
            "Player",          // Player character
            "Foreground",      // Foreground decorations
            "UI"               // UI elements (though Canvas handles this)
        };
        
        // Clear existing custom layers (keep Default)
        while (sortingLayers.arraySize > 1)
        {
            sortingLayers.DeleteArrayElementAtIndex(1);
        }
        
        // Add our layers
        for (int i = 1; i < desiredLayers.Length; i++)
        {
            sortingLayers.InsertArrayElementAtIndex(i);
            SerializedProperty layer = sortingLayers.GetArrayElementAtIndex(i);
            layer.FindPropertyRelative("name").stringValue = desiredLayers[i];
            layer.FindPropertyRelative("uniqueID").intValue = 1000 + i;
        }
        
        tagManager.ApplyModifiedProperties();
        
        Debug.Log("Created sorting layers: " + string.Join(", ", desiredLayers));
    }
    
    private static void FixSpritesInScene()
    {
        int fixedCount = 0;
        
        // Find all SpriteRenderers in scene
        SpriteRenderer[] allSprites = FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        
        foreach (SpriteRenderer sprite in allSprites)
        {
            string objName = sprite.gameObject.name.ToLower();
            string parentName = sprite.transform.parent ? sprite.transform.parent.name.ToLower() : "";
            
            // Determine correct sorting layer based on name
            if (parentName.Contains("background") || objName.Contains("background") || objName.Contains("layer"))
            {
                sprite.sortingLayerName = "Background";
                sprite.sortingOrder = 0;
                fixedCount++;
            }
            else if (objName.Contains("ground") || objName.Contains("platform"))
            {
                sprite.sortingLayerName = "Ground";
                sprite.sortingOrder = 0;
                fixedCount++;
            }
            else if (parentName.Contains("foreground") || objName.Contains("foreground") || 
                     objName.Contains("fence") || objName.Contains("lamp") || objName.Contains("rock") || 
                     objName.Contains("shop") || objName.Contains("tree") || objName.Contains("pine"))
            {
                sprite.sortingLayerName = "Foreground";
                sprite.sortingOrder = GetForegroundOrder(sprite.transform.position.y);
                fixedCount++;
            }
            else if (objName.Contains("character") || objName.Contains("char_") || objName.Contains("ninja"))
            {
                sprite.sortingLayerName = "Player";
                sprite.sortingOrder = GetCharacterOrder(sprite.transform.position.y);
                fixedCount++;
            }
            else if (objName.Contains("enemy") || objName.Contains("goblin") || objName.Contains("skeleton") || 
                     objName.Contains("mushroom") || objName.Contains("eye") || objName.Contains("sword"))
            {
                sprite.sortingLayerName = "Enemies";
                sprite.sortingOrder = GetCharacterOrder(sprite.transform.position.y);
                fixedCount++;
            }
            
            EditorUtility.SetDirty(sprite);
        }
        
        Debug.Log($"Fixed {fixedCount} sprites in scene");
    }
    
    private static int GetForegroundOrder(float yPosition)
    {
        // Lower Y position = further forward (higher order)
        // This creates depth effect for foreground objects
        return Mathf.RoundToInt(-yPosition * 10);
    }
    
    private static int GetCharacterOrder(float yPosition)
    {
        // Characters also use Y position for depth sorting
        // Lower on screen = in front
        return Mathf.RoundToInt(-yPosition * 100);
    }
    
    [MenuItem("Tools/Fix Player Sorting")]
    public static void FixPlayerSorting()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            SpriteRenderer sprite = player.GetComponent<SpriteRenderer>();
            if (sprite != null)
            {
                sprite.sortingLayerName = "Player";
                sprite.sortingOrder = 100;
                EditorUtility.SetDirty(sprite);
                Debug.Log("✅ Player sorting fixed: Layer='Player', Order=100");
            }
            else
            {
                Debug.LogWarning("Player found but has no SpriteRenderer!");
            }
        }
        else
        {
            Debug.LogWarning("No GameObject with 'Player' tag found!");
        }
    }
    
    [MenuItem("Tools/Fix Enemy Prefabs Sorting")]
    public static void FixEnemyPrefabs()
    {
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs" });
        int fixedCount = 0;
        
        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            
            if (prefab != null)
            {
                string prefabName = prefab.name.ToLower();
                
                // Check if it's an enemy prefab
                if (prefabName.Contains("goblin") || prefabName.Contains("skeleton") || 
                    prefabName.Contains("mushroom") || prefabName.Contains("eye") || 
                    prefabName.Contains("sword") || prefabName.Contains("enemy"))
                {
                    SpriteRenderer sprite = prefab.GetComponent<SpriteRenderer>();
                    if (sprite != null)
                    {
                        sprite.sortingLayerName = "Enemies";
                        sprite.sortingOrder = 50;
                        
                        EditorUtility.SetDirty(prefab);
                        AssetDatabase.SaveAssets();
                        
                        fixedCount++;
                        Debug.Log($"✅ Fixed enemy prefab: {prefab.name}");
                    }
                }
            }
        }
        
        AssetDatabase.Refresh();
        Debug.Log($"Fixed {fixedCount} enemy prefabs");
    }
}
