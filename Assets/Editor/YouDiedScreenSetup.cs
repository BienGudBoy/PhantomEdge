using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class YouDiedScreenSetup : EditorWindow
{
    [MenuItem("Tools/Setup YouDiedScreen")]
    public static void SetupYouDiedScreen()
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
        
        // Find or create YouDiedScreen GameObject
        GameObject youDiedScreenGO = GameObject.Find("YouDiedScreen");
        if (youDiedScreenGO == null)
        {
            youDiedScreenGO = new GameObject("YouDiedScreen");
            youDiedScreenGO.transform.SetParent(canvas.transform, false);
        }
        
        // Ensure YouDiedScreen is properly positioned at center
        RectTransform youDiedScreenRect = youDiedScreenGO.GetComponent<RectTransform>();
        if (youDiedScreenRect == null)
        {
            youDiedScreenRect = youDiedScreenGO.AddComponent<RectTransform>();
        }
        youDiedScreenRect.anchorMin = new Vector2(0.5f, 0.5f);
        youDiedScreenRect.anchorMax = new Vector2(0.5f, 0.5f);
        youDiedScreenRect.pivot = new Vector2(0.5f, 0.5f);
        youDiedScreenRect.anchoredPosition = Vector2.zero;
        youDiedScreenRect.sizeDelta = Vector2.zero;
        
        // Add YouDiedScreen component
        YouDiedScreen youDiedScreen = youDiedScreenGO.GetComponent<YouDiedScreen>();
        if (youDiedScreen == null)
        {
            youDiedScreen = youDiedScreenGO.AddComponent<YouDiedScreen>();
        }
        
        // Create DeathScreenPanel
        GameObject panelGO = GameObject.Find("DeathScreenPanel");
        if (panelGO == null)
        {
            panelGO = new GameObject("DeathScreenPanel");
            panelGO.transform.SetParent(youDiedScreenGO.transform, false);
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
        GameObject overlayGO = GameObject.Find("DarkOverlay");
        if (overlayGO == null)
        {
            overlayGO = new GameObject("DarkOverlay");
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
        
        // Create background bar for "YOU DIED" text
        GameObject textBackgroundGO = GameObject.Find("YouDiedTextBackground");
        if (textBackgroundGO == null)
        {
            textBackgroundGO = new GameObject("YouDiedTextBackground");
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
        backgroundRect.sizeDelta = new Vector2(1000, 250); // Wider than text for better effect
        backgroundRect.anchoredPosition = new Vector2(0, 50);
        
        Image backgroundImage = textBackgroundGO.GetComponent<Image>();
        if (backgroundImage == null)
        {
            backgroundImage = textBackgroundGO.AddComponent<Image>();
        }
        backgroundImage.color = new Color(0, 0, 0, 0); // Start transparent, will fade in with text
        
        // Set background to appear behind text (lower sibling index)
        textBackgroundGO.transform.SetSiblingIndex(0);
        
        // Create YouDiedText
        GameObject youDiedTextGO = GameObject.Find("YouDiedText");
        if (youDiedTextGO == null)
        {
            youDiedTextGO = new GameObject("YouDiedText");
            youDiedTextGO.transform.SetParent(panelGO.transform, false);
        }
        
        RectTransform textRect = youDiedTextGO.GetComponent<RectTransform>();
        if (textRect == null)
        {
            textRect = youDiedTextGO.AddComponent<RectTransform>();
        }
        
        textRect.anchorMin = new Vector2(0.5f, 0.5f);
        textRect.anchorMax = new Vector2(0.5f, 0.5f);
        textRect.pivot = new Vector2(0.5f, 0.5f);
        textRect.sizeDelta = new Vector2(800, 200);
        textRect.anchoredPosition = new Vector2(0, 50); // Centered, slightly above center
        
        TextMeshProUGUI youDiedText = youDiedTextGO.GetComponent<TextMeshProUGUI>();
        if (youDiedText == null)
        {
            youDiedText = youDiedTextGO.AddComponent<TextMeshProUGUI>();
        }
        
        youDiedText.text = "YOU DIED";
        youDiedText.fontSize = 120;
        youDiedText.alignment = TextAlignmentOptions.Center;
        youDiedText.color = new Color(0.8f, 0.1f, 0.1f, 0f); // Dark red, transparent initially
        youDiedText.fontStyle = FontStyles.Bold;
        
        // Create ContinueText
        GameObject continueTextGO = GameObject.Find("ContinueText");
        if (continueTextGO == null)
        {
            continueTextGO = new GameObject("ContinueText");
            continueTextGO.transform.SetParent(panelGO.transform, false);
        }
        
        RectTransform continueRect = continueTextGO.GetComponent<RectTransform>();
        if (continueRect == null)
        {
            continueRect = continueTextGO.AddComponent<RectTransform>();
        }
        
        continueRect.anchorMin = new Vector2(0.5f, 0.5f);
        continueRect.anchorMax = new Vector2(0.5f, 0.5f);
        continueRect.pivot = new Vector2(0.5f, 0.5f);
        continueRect.sizeDelta = new Vector2(600, 100);
        continueRect.anchoredPosition = new Vector2(0, -100); // Centered, below "YOU DIED" text
        
        TextMeshProUGUI continueText = continueTextGO.GetComponent<TextMeshProUGUI>();
        if (continueText == null)
        {
            continueText = continueTextGO.AddComponent<TextMeshProUGUI>();
        }
        
        continueText.text = "Press any button to continue";
        continueText.fontSize = 36;
        continueText.alignment = TextAlignmentOptions.Center;
        continueText.color = new Color(1f, 1f, 1f, 0f); // White, transparent initially
        continueTextGO.SetActive(false);
        
        // Create RestartButton
        GameObject restartButtonGO = GameObject.Find("RestartButton");
        if (restartButtonGO == null)
        {
            restartButtonGO = new GameObject("RestartButton");
            restartButtonGO.transform.SetParent(panelGO.transform, false);
        }
        
        RectTransform restartRect = restartButtonGO.GetComponent<RectTransform>();
        if (restartRect == null)
        {
            restartRect = restartButtonGO.AddComponent<RectTransform>();
        }
        
        restartRect.anchorMin = new Vector2(0.5f, 0.5f);
        restartRect.anchorMax = new Vector2(0.5f, 0.5f);
        restartRect.pivot = new Vector2(0.5f, 0.5f);
        restartRect.sizeDelta = new Vector2(200, 50);
        restartRect.anchoredPosition = new Vector2(-120, -200); // Centered horizontally, below continue text
        
        Image restartImage = restartButtonGO.GetComponent<Image>();
        if (restartImage == null)
        {
            restartImage = restartButtonGO.AddComponent<Image>();
        }
        restartImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        Button restartButton = restartButtonGO.GetComponent<Button>();
        if (restartButton == null)
        {
            restartButton = restartButtonGO.AddComponent<Button>();
        }
        
        // Add text to button
        GameObject restartTextGO = new GameObject("Text");
        restartTextGO.transform.SetParent(restartButtonGO.transform, false);
        RectTransform restartTextRect = restartTextGO.AddComponent<RectTransform>();
        restartTextRect.anchorMin = Vector2.zero;
        restartTextRect.anchorMax = Vector2.one;
        restartTextRect.sizeDelta = Vector2.zero;
        restartTextRect.anchoredPosition = Vector2.zero;
        
        TextMeshProUGUI restartText = restartTextGO.AddComponent<TextMeshProUGUI>();
        restartText.text = "Restart";
        restartText.fontSize = 24;
        restartText.alignment = TextAlignmentOptions.Center;
        restartText.color = Color.white;
        
        restartButtonGO.SetActive(false);
        
        // Create MenuButton
        GameObject menuButtonGO = GameObject.Find("MenuButton");
        if (menuButtonGO == null)
        {
            menuButtonGO = new GameObject("MenuButton");
            menuButtonGO.transform.SetParent(panelGO.transform, false);
        }
        
        RectTransform menuRect = menuButtonGO.GetComponent<RectTransform>();
        if (menuRect == null)
        {
            menuRect = menuButtonGO.AddComponent<RectTransform>();
        }
        
        menuRect.anchorMin = new Vector2(0.5f, 0.5f);
        menuRect.anchorMax = new Vector2(0.5f, 0.5f);
        menuRect.pivot = new Vector2(0.5f, 0.5f);
        menuRect.sizeDelta = new Vector2(200, 50);
        menuRect.anchoredPosition = new Vector2(120, -200); // Centered horizontally, below continue text
        
        Image menuImage = menuButtonGO.GetComponent<Image>();
        if (menuImage == null)
        {
            menuImage = menuButtonGO.AddComponent<Image>();
        }
        menuImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        Button menuButton = menuButtonGO.GetComponent<Button>();
        if (menuButton == null)
        {
            menuButton = menuButtonGO.AddComponent<Button>();
        }
        
        // Add text to button
        GameObject menuTextGO = new GameObject("Text");
        menuTextGO.transform.SetParent(menuButtonGO.transform, false);
        RectTransform menuTextRect = menuTextGO.AddComponent<RectTransform>();
        menuTextRect.anchorMin = Vector2.zero;
        menuTextRect.anchorMax = Vector2.one;
        menuTextRect.sizeDelta = Vector2.zero;
        menuTextRect.anchoredPosition = Vector2.zero;
        
        TextMeshProUGUI menuText = menuTextGO.AddComponent<TextMeshProUGUI>();
        menuText.text = "Main Menu";
        menuText.fontSize = 24;
        menuText.alignment = TextAlignmentOptions.Center;
        menuText.color = Color.white;
        
        menuButtonGO.SetActive(false);
        
        // Assign references to YouDiedScreen component using reflection
        SerializedObject serializedObject = new SerializedObject(youDiedScreen);
        
        SerializedProperty panelProp = serializedObject.FindProperty("deathScreenPanel");
        if (panelProp != null) panelProp.objectReferenceValue = panelGO;
        
        SerializedProperty overlayProp = serializedObject.FindProperty("darkOverlay");
        if (overlayProp != null) overlayProp.objectReferenceValue = overlayImage;
        
        SerializedProperty textBackgroundProp = serializedObject.FindProperty("textBackground");
        if (textBackgroundProp != null) textBackgroundProp.objectReferenceValue = backgroundImage;
        
        SerializedProperty youDiedTextProp = serializedObject.FindProperty("youDiedText");
        if (youDiedTextProp != null) youDiedTextProp.objectReferenceValue = youDiedText;
        
        SerializedProperty continueTextProp = serializedObject.FindProperty("continueText");
        if (continueTextProp != null) continueTextProp.objectReferenceValue = continueText;
        
        SerializedProperty restartButtonProp = serializedObject.FindProperty("restartButton");
        if (restartButtonProp != null) restartButtonProp.objectReferenceValue = restartButton;
        
        SerializedProperty menuButtonProp = serializedObject.FindProperty("menuButton");
        if (menuButtonProp != null) menuButtonProp.objectReferenceValue = menuButton;
        
        serializedObject.ApplyModifiedProperties();
        
        // Also mark the scene as dirty
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        
        Debug.Log("YouDiedScreen setup complete!");
        Debug.Log($"Panel: {panelGO.name}, Overlay: {overlayImage != null}, YouDiedText: {youDiedText != null}, ContinueText: {continueText != null}, RestartButton: {restartButton != null}, MenuButton: {menuButton != null}");
        EditorUtility.SetDirty(youDiedScreen);
    }
}

