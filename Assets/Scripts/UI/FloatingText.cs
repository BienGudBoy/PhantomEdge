using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class FloatingText : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private float fadeOutDuration = 1f;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private Vector2 moveDirection = Vector2.up;
    
    private TextMeshProUGUI textMesh;
    private Canvas canvas;
    private float timer = 0f;
    private Color originalColor;
    private Vector3 startPosition;
    
    private void Awake()
    {
        // Create Canvas for world space rendering
        canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;
            canvas.sortingOrder = 32767; // Maximum sorting order to render on top of everything
            canvas.sortingLayerName = "UI"; // Set to UI layer (highest layer)
        }
        
        // Ensure canvas is always on top
        canvas.sortingOrder = 32767;
        canvas.sortingLayerName = "UI";
        
        // Create RectTransform
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            rectTransform = gameObject.AddComponent<RectTransform>();
        }
        
        // Set proper scale for world space (smaller scale = larger text in world)
        rectTransform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        rectTransform.sizeDelta = new Vector2(500f, 200f);
        
        // Add CanvasScaler for proper scaling
        CanvasScaler scaler = GetComponent<CanvasScaler>();
        if (scaler == null)
        {
            scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            scaler.scaleFactor = 1f;
        }
        
        // Add GraphicRaycaster (needed for UI to render)
        GraphicRaycaster raycaster = GetComponent<GraphicRaycaster>();
        if (raycaster == null)
        {
            raycaster = gameObject.AddComponent<GraphicRaycaster>();
        }
        
        // Create TextMeshProUGUI
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(transform, false);
        
        textMesh = textObj.AddComponent<TextMeshProUGUI>();
        
        // Set default text properties
        textMesh.fontSize = 48; // Larger font for world space
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.fontStyle = FontStyles.Bold;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
        
        originalColor = textMesh.color;
        startPosition = transform.position;
    }
    
    private void Start()
    {
        StartCoroutine(FadeOutAndDestroy());
    }
    
    public void SetText(string text, Color? color = null)
    {
        if (textMesh != null)
        {
            textMesh.text = text;
            if (color.HasValue)
            {
                textMesh.color = color.Value;
                originalColor = color.Value;
            }
        }
    }
    
    public void SetColor(Color color)
    {
        if (textMesh != null)
        {
            textMesh.color = color;
            originalColor = color;
        }
    }
    
    private void Update()
    {
        // Increment timer
        timer += Time.deltaTime;
        
        // Move the text upward
        transform.position = startPosition + (Vector3)(moveDirection * moveSpeed * timer);
        
        // Always face the camera (billboard effect)
        if (Camera.main != null)
        {
            // Update canvas camera reference
            if (canvas != null)
            {
                if (canvas.worldCamera != Camera.main)
                {
                    canvas.worldCamera = Camera.main;
                }
                
                // Ensure canvas is always on top (in case something changes it)
                canvas.sortingOrder = 32767;
                canvas.sortingLayerName = "UI";
            }
        }
    }
    
    private IEnumerator FadeOutAndDestroy()
    {
        float elapsed = 0f;
        float fadeStartTime = lifetime - fadeOutDuration;
        
        // Wait until fade out should start
        yield return new WaitForSeconds(fadeStartTime);
        
        // Fade out
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
            
            if (textMesh != null)
            {
                Color color = originalColor;
                color.a = alpha;
                textMesh.color = color;
            }
            
            yield return null;
        }
        
        // Destroy the floating text
        Destroy(gameObject);
    }
    
    public static FloatingText Create(Vector3 position, string text, Color? color = null, float lifetime = 2f)
    {
        GameObject textObj = new GameObject("FloatingText");
        textObj.transform.position = position;
        
        FloatingText floatingText = textObj.AddComponent<FloatingText>();
        floatingText.lifetime = lifetime;
        
        // Set text and color
        floatingText.SetText(text, color);
        
        return floatingText;
    }
}

