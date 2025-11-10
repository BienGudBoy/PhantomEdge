using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class VictoryScreen : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject victoryScreenPanel;
    [SerializeField] private Image darkOverlay;
    [SerializeField] private Image textBackground; // Dark background bar behind "VICTORY ACHIEVED" text
    [SerializeField] private TextMeshProUGUI victoryText;
    
    [Header("Animation Settings")]
    [SerializeField] private float fadeInDuration = 2f;
    [SerializeField] private float textDelay = 0.5f;
    [SerializeField] private float textFadeInDuration = 1.5f;
    [SerializeField] private float displayDuration = 3f; // How long to show the text before fading out
    [SerializeField] private float fadeOutDuration = 2f; // How long to fade out
    
    [Header("Colors")]
    [SerializeField] private Color overlayColor = new Color(0, 0, 0, 0.9f);
    [SerializeField] private Color textColor = new Color(0.9f, 0.7f, 0.1f, 1f); // Golden yellow
    
    private Coroutine fadeCoroutine;
    
    private void Awake()
    {
        Debug.Log("VictoryScreen: Awake() called");
        
        // Initialize UI state
        if (victoryScreenPanel != null)
        {
            victoryScreenPanel.SetActive(false);
            Debug.Log("VictoryScreen: Panel set inactive");
        }
        else
        {
            Debug.LogError("VictoryScreen: victoryScreenPanel is null in Awake!");
        }
        
        if (darkOverlay != null)
        {
            darkOverlay.color = new Color(overlayColor.r, overlayColor.g, overlayColor.b, 0f);
            Debug.Log("VictoryScreen: DarkOverlay initialized");
        }
        else
        {
            Debug.LogError("VictoryScreen: darkOverlay is null in Awake!");
        }
        
        if (textBackground != null)
        {
            textBackground.color = new Color(0, 0, 0, 0f); // Start transparent
            Debug.Log("VictoryScreen: TextBackground initialized");
        }
        
        if (victoryText != null)
        {
            victoryText.color = new Color(textColor.r, textColor.g, textColor.b, 0f);
            Debug.Log("VictoryScreen: VictoryText initialized");
        }
        else
        {
            Debug.LogError("VictoryScreen: victoryText is null in Awake!");
        }
    }
    
    public void ShowVictoryScreen()
    {
        Debug.Log("VictoryScreen.ShowVictoryScreen() called");
        
        if (victoryScreenPanel == null)
        {
            Debug.LogError("VictoryScreen: victoryScreenPanel is null! Please run Tools > Setup VictoryScreen");
            return;
        }
        
        // Play victory sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayVictorySound();
        }
        
        victoryScreenPanel.SetActive(true);
        Debug.Log("VictoryScreen: Panel activated");
        
        // Stop any existing coroutines
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        
        // Start fade-in animation
        fadeCoroutine = StartCoroutine(VictoryScreenAnimation());
        Debug.Log("VictoryScreen: Animation coroutine started");
    }
    
    public void HideVictoryScreen()
    {
        if (victoryScreenPanel != null)
        {
            victoryScreenPanel.SetActive(false);
        }
        
        // Stop animations
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        
        // Reset UI state
        if (darkOverlay != null)
        {
            darkOverlay.color = new Color(overlayColor.r, overlayColor.g, overlayColor.b, 0f);
        }
        
        if (textBackground != null)
        {
            textBackground.color = new Color(0, 0, 0, 0f);
        }
        
        if (victoryText != null)
        {
            victoryText.color = new Color(textColor.r, textColor.g, textColor.b, 0f);
        }
    }
    
    private IEnumerator VictoryScreenAnimation()
    {
        Debug.Log("VictoryScreen: Starting animation");
        
        // Fade in dark overlay
        if (darkOverlay == null)
        {
            Debug.LogError("VictoryScreen: darkOverlay is null!");
            yield break;
        }
        
        float elapsed = 0f;
        Color startColor = new Color(overlayColor.r, overlayColor.g, overlayColor.b, 0f);
        Color targetColor = overlayColor;
        
        Debug.Log($"VictoryScreen: Fading overlay from {startColor} to {targetColor}");
        
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / fadeInDuration;
            darkOverlay.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }
        
        darkOverlay.color = targetColor;
        Debug.Log("VictoryScreen: Overlay fade complete");
        
        // Wait before showing text
        yield return new WaitForSecondsRealtime(textDelay);
        
        // Fade in "VICTORY ACHIEVED" text and background bar together
        if (victoryText != null)
        {
            float textElapsed = 0f;
            Color textStartColor = new Color(textColor.r, textColor.g, textColor.b, 0f);
            Color textTargetColor = textColor;
            
            // Background bar color (dark, semi-transparent)
            Color backgroundStartColor = new Color(0, 0, 0, 0f);
            Color backgroundTargetColor = new Color(0, 0, 0, 0.8f); // Dark background with 80% opacity
            
            while (textElapsed < textFadeInDuration)
            {
                textElapsed += Time.unscaledDeltaTime;
                float t = textElapsed / textFadeInDuration;
                
                // Fade in text
                victoryText.color = Color.Lerp(textStartColor, textTargetColor, t);
                
                // Fade in background bar
                if (textBackground != null)
                {
                    textBackground.color = Color.Lerp(backgroundStartColor, backgroundTargetColor, t);
                }
                
                yield return null;
            }
            
            victoryText.color = textTargetColor;
            if (textBackground != null)
            {
                textBackground.color = backgroundTargetColor;
            }
        }
        
        // Wait for display duration
        yield return new WaitForSecondsRealtime(displayDuration);
        
        // Fade out everything
        float fadeOutElapsed = 0f;
        Color overlayFadeOutStart = overlayColor;
        Color overlayFadeOutEnd = new Color(overlayColor.r, overlayColor.g, overlayColor.b, 0f);
        Color textFadeOutStart = textColor;
        Color textFadeOutEnd = new Color(textColor.r, textColor.g, textColor.b, 0f);
        Color backgroundFadeOutStart = new Color(0, 0, 0, 0.8f);
        Color backgroundFadeOutEnd = new Color(0, 0, 0, 0f);
        
        while (fadeOutElapsed < fadeOutDuration)
        {
            fadeOutElapsed += Time.unscaledDeltaTime;
            float t = fadeOutElapsed / fadeOutDuration;
            
            // Fade out overlay
            if (darkOverlay != null)
            {
                darkOverlay.color = Color.Lerp(overlayFadeOutStart, overlayFadeOutEnd, t);
            }
            
            // Fade out text
            if (victoryText != null)
            {
                victoryText.color = Color.Lerp(textFadeOutStart, textFadeOutEnd, t);
            }
            
            // Fade out background
            if (textBackground != null)
            {
                textBackground.color = Color.Lerp(backgroundFadeOutStart, backgroundFadeOutEnd, t);
            }
            
            yield return null;
        }
        
        // Ensure everything is fully transparent
        if (darkOverlay != null)
        {
            darkOverlay.color = overlayFadeOutEnd;
        }
        if (victoryText != null)
        {
            victoryText.color = textFadeOutEnd;
        }
        if (textBackground != null)
        {
            textBackground.color = backgroundFadeOutEnd;
        }
        
        // Hide the panel
        HideVictoryScreen();
    }
}

