using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEditor.SceneManagement;

public class InteractPromptSetup : EditorWindow
{
    [MenuItem("Tools/Setup Interact Prompt UI")]
    public static void SetupInteractPrompt()
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
        
        // Find or create UIManager
        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager == null)
        {
            GameObject uiManagerGO = new GameObject("UIManager");
            uiManager = uiManagerGO.AddComponent<UIManager>();
        }
        
        // Create InteractPromptPanel
        GameObject promptPanelGO = GameObject.Find("InteractPromptPanel");
        if (promptPanelGO == null)
        {
            promptPanelGO = new GameObject("InteractPromptPanel");
            promptPanelGO.transform.SetParent(canvas.transform, false);
        }
        
        RectTransform panelRect = promptPanelGO.GetComponent<RectTransform>();
        if (panelRect == null)
        {
            panelRect = promptPanelGO.AddComponent<RectTransform>();
        }
        
        // Position at bottom center of screen
        panelRect.anchorMin = new Vector2(0.5f, 0f);
        panelRect.anchorMax = new Vector2(0.5f, 0f);
        panelRect.pivot = new Vector2(0.5f, 0f);
        panelRect.sizeDelta = new Vector2(600, 80);
        panelRect.anchoredPosition = new Vector2(0, 100); // 100 pixels from bottom
        
        // Add background image (optional, for better visibility)
        Image panelImage = promptPanelGO.GetComponent<Image>();
        if (panelImage == null)
        {
            panelImage = promptPanelGO.AddComponent<Image>();
        }
        panelImage.color = new Color(0, 0, 0, 0.7f); // Semi-transparent black background
        
        // Set panel inactive by default
        promptPanelGO.SetActive(false);
        
        // Create InteractPromptText
        GameObject promptTextGO = GameObject.Find("InteractPromptText");
        if (promptTextGO == null)
        {
            promptTextGO = new GameObject("InteractPromptText");
            promptTextGO.transform.SetParent(promptPanelGO.transform, false);
        }
        
        RectTransform textRect = promptTextGO.GetComponent<RectTransform>();
        if (textRect == null)
        {
            textRect = promptTextGO.AddComponent<RectTransform>();
        }
        
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
        
        TextMeshProUGUI promptText = promptTextGO.GetComponent<TextMeshProUGUI>();
        if (promptText == null)
        {
            promptText = promptTextGO.AddComponent<TextMeshProUGUI>();
        }
        
        promptText.text = "Press E to interact";
        promptText.fontSize = 32;
        promptText.alignment = TextAlignmentOptions.Center;
        promptText.color = Color.white;
        promptText.fontStyle = FontStyles.Bold;
        
        // Assign references to UIManager using SerializedObject
        SerializedObject serializedUIManager = new SerializedObject(uiManager);
        
        SerializedProperty panelProp = serializedUIManager.FindProperty("interactPromptPanel");
        if (panelProp != null) panelProp.objectReferenceValue = promptPanelGO;
        
        SerializedProperty textProp = serializedUIManager.FindProperty("interactPromptText");
        if (textProp != null) textProp.objectReferenceValue = promptText;
        
        serializedUIManager.ApplyModifiedProperties();
        
        // Mark scene as dirty
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        
        Debug.Log("Interact Prompt UI setup complete!");
        Debug.Log($"Panel: {promptPanelGO.name}, Text: {promptText != null}");
        EditorUtility.SetDirty(uiManager);
    }
}

