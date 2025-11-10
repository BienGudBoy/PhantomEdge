using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class VictoryScreenSetup : EditorWindow
{
    [MenuItem("Tools/Setup VictoryScreen")]
    public static void SetupVictoryScreen()
    {
        // Find or create Canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }
        
        // Find or create VictoryScreen GameObject
        GameObject victoryScreenGO = GameObject.Find("VictoryScreen");
        if (victoryScreenGO == null)
        {
            victoryScreenGO = new GameObject("VictoryScreen");
            victoryScreenGO.transform.SetParent(canvas.transform, false);
        }
        
        // Ensure VictoryScreen is properly positioned at center
        RectTransform victoryScreenRect = victoryScreenGO.GetComponent<RectTransform>();
        if (victoryScreenRect == null)
        {
            victoryScreenRect = victoryScreenGO.AddComponent<RectTransform>();
        }
        victoryScreenRect.anchorMin = new Vector2(0.5f, 0.5f);
        victoryScreenRect.anchorMax = new Vector2(0.5f, 0.5f);
        victoryScreenRect.pivot = new Vector2(0.5f, 0.5f);
        victoryScreenRect.anchoredPosition = Vector2.zero;
        victoryScreenRect.sizeDelta = Vector2.zero;
        
        // Add VictoryScreen component
        VictoryScreen victoryScreen = victoryScreenGO.GetComponent<VictoryScreen>();
        if (victoryScreen == null)
        {
            victoryScreen = victoryScreenGO.AddComponent<VictoryScreen>();
        }
        
        // Create VictoryScreenPanel
        GameObject panelGO = GameObject.Find("VictoryScreenPanel");
        if (panelGO == null)
        {
            panelGO = new GameObject("VictoryScreenPanel");
            panelGO.transform.SetParent(victoryScreenGO.transform, false);
        }
        
        RectTransform panelRect = panelGO.GetComponent<RectTransform>();
        if (panelRect == null)
        {
            panelRect = panelGO.AddComponent<RectTransform>();
        }
        
        // Set panel to full screen
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        panelRect.anchoredPosition = Vector2.zero;
        
        Image panelImage = panelGO.GetComponent<Image>();
        if (panelImage == null)
        {
            panelImage = panelGO.AddComponent<Image>();
        }
        panelImage.color = new Color(0, 0, 0, 0);
        
        // Set panel inactive by default
        panelGO.SetActive(false);
        
        // Create DarkOverlay
        GameObject overlayGO = GameObject.Find("VictoryDarkOverlay");
        if (overlayGO == null)
        {
            overlayGO = new GameObject("VictoryDarkOverlay");
            overlayGO.transform.SetParent(panelGO.transform, false);
        }
        
        RectTransform overlayRect = overlayGO.GetComponent<RectTransform>();
        if (overlayRect == null)
        {
            overlayRect = overlayGO.AddComponent<RectTransform>();
        }
        
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.sizeDelta = Vector2.zero;
        overlayRect.anchoredPosition = Vector2.zero;
        
        Image overlayImage = overlayGO.GetComponent<Image>();
        if (overlayImage == null)
        {
            overlayImage = overlayGO.AddComponent<Image>();
        }
        overlayImage.color = new Color(0, 0, 0, 0);
        
        // Create background bar for "VICTORY ACHIEVED" text
        GameObject textBackgroundGO = GameObject.Find("VictoryTextBackground");
        if (textBackgroundGO == null)
        {
            textBackgroundGO = new GameObject("VictoryTextBackground");
            textBackgroundGO.transform.SetParent(panelGO.transform, false);
        }
        
        RectTransform backgroundRect = textBackgroundGO.GetComponent<RectTransform>();
        if (backgroundRect == null)
        {
            backgroundRect = textBackgroundGO.AddComponent<RectTransform>();
        }
        
        backgroundRect.anchorMin = new Vector2(0.5f, 0.5f);
        backgroundRect.anchorMax = new Vector2(0.5f, 0.5f);
        backgroundRect.pivot = new Vector2(0.5f, 0.5f);
        backgroundRect.sizeDelta = new Vector2(1200, 250); // Wider than text for better effect
        backgroundRect.anchoredPosition = new Vector2(0, 50);
        
        Image backgroundImage = textBackgroundGO.GetComponent<Image>();
        if (backgroundImage == null)
        {
            backgroundImage = textBackgroundGO.AddComponent<Image>();
        }
        backgroundImage.color = new Color(0, 0, 0, 0); // Start transparent, will fade in with text
        
        // Set background to appear behind text (lower sibling index)
        textBackgroundGO.transform.SetSiblingIndex(0);
        
        // Create VictoryText
        GameObject victoryTextGO = GameObject.Find("VictoryText");
        if (victoryTextGO == null)
        {
            victoryTextGO = new GameObject("VictoryText");
            victoryTextGO.transform.SetParent(panelGO.transform, false);
        }
        
        RectTransform textRect = victoryTextGO.GetComponent<RectTransform>();
        if (textRect == null)
        {
            textRect = victoryTextGO.AddComponent<RectTransform>();
        }
        
        textRect.anchorMin = new Vector2(0.5f, 0.5f);
        textRect.anchorMax = new Vector2(0.5f, 0.5f);
        textRect.pivot = new Vector2(0.5f, 0.5f);
        textRect.sizeDelta = new Vector2(1000, 200);
        textRect.anchoredPosition = new Vector2(0, 50); // Centered, slightly above center
        
        TextMeshProUGUI victoryText = victoryTextGO.GetComponent<TextMeshProUGUI>();
        if (victoryText == null)
        {
            victoryText = victoryTextGO.AddComponent<TextMeshProUGUI>();
        }
        
        victoryText.text = "VICTORY ACHIEVED";
        victoryText.fontSize = 120;
        victoryText.alignment = TextAlignmentOptions.Center;
        victoryText.color = new Color(0.9f, 0.7f, 0.1f, 0f); // Golden yellow, transparent initially
        victoryText.fontStyle = FontStyles.Bold;
        
        // Assign references to VictoryScreen component using reflection
        SerializedObject serializedObject = new SerializedObject(victoryScreen);
        
        SerializedProperty panelProp = serializedObject.FindProperty("victoryScreenPanel");
        if (panelProp != null) panelProp.objectReferenceValue = panelGO;
        
        SerializedProperty overlayProp = serializedObject.FindProperty("darkOverlay");
        if (overlayProp != null) overlayProp.objectReferenceValue = overlayImage;
        
        SerializedProperty textBackgroundProp = serializedObject.FindProperty("textBackground");
        if (textBackgroundProp != null) textBackgroundProp.objectReferenceValue = backgroundImage;
        
        SerializedProperty victoryTextProp = serializedObject.FindProperty("victoryText");
        if (victoryTextProp != null) victoryTextProp.objectReferenceValue = victoryText;
        
        serializedObject.ApplyModifiedProperties();
        
        // Also mark the scene as dirty
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        
        Debug.Log("VictoryScreen setup complete!");
        Debug.Log($"Panel: {panelGO.name}, Overlay: {overlayImage != null}, VictoryText: {victoryText != null}");
        EditorUtility.SetDirty(victoryScreen);
    }
}

