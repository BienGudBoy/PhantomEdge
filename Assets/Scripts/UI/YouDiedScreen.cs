using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class YouDiedScreen : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject deathScreenPanel;
    [SerializeField] private Image darkOverlay;
    [SerializeField] private Image textBackground; // Dark background bar behind "YOU DIED" text
    [SerializeField] private TextMeshProUGUI youDiedText;
    [SerializeField] private TextMeshProUGUI continueText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;
    
    [Header("Animation Settings")]
    [SerializeField] private float fadeInDuration = 2f;
    [SerializeField] private float textDelay = 0.5f;
    [SerializeField] private float textFadeInDuration = 1.5f;
    [SerializeField] private float continueTextDelay = 3f;
    [SerializeField] private float continueTextBlinkSpeed = 1f;
    
    [Header("Colors")]
    [SerializeField] private Color overlayColor = new Color(0, 0, 0, 0.9f);
    [SerializeField] private Color textColor = new Color(0.8f, 0.1f, 0.1f, 1f); // Dark red
    
    private Coroutine fadeCoroutine;
    private Coroutine blinkCoroutine;
    
    private void Awake()
    {
        Debug.Log("YouDiedScreen: Awake() called");
        
        // Initialize UI state
        if (deathScreenPanel != null)
        {
            deathScreenPanel.SetActive(false);
            Debug.Log("YouDiedScreen: Panel set inactive");
        }
        else
        {
            Debug.LogError("YouDiedScreen: deathScreenPanel is null in Awake!");
        }
        
        if (darkOverlay != null)
        {
            darkOverlay.color = new Color(overlayColor.r, overlayColor.g, overlayColor.b, 0f);
            Debug.Log("YouDiedScreen: DarkOverlay initialized");
        }
        else
        {
            Debug.LogError("YouDiedScreen: darkOverlay is null in Awake!");
        }
        
        if (textBackground != null)
        {
            textBackground.color = new Color(0, 0, 0, 0f); // Start transparent
            Debug.Log("YouDiedScreen: TextBackground initialized");
        }
        
        if (youDiedText != null)
        {
            youDiedText.color = new Color(textColor.r, textColor.g, textColor.b, 0f);
            Debug.Log("YouDiedScreen: YouDiedText initialized");
        }
        else
        {
            Debug.LogError("YouDiedScreen: youDiedText is null in Awake!");
        }
        
        if (continueText != null)
        {
            continueText.color = new Color(1f, 1f, 1f, 0f);
            Debug.Log("YouDiedScreen: ContinueText initialized");
        }
        else
        {
            Debug.LogError("YouDiedScreen: continueText is null in Awake!");
        }
        
        // Setup buttons
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartButton);
            restartButton.gameObject.SetActive(false);
            Debug.Log("YouDiedScreen: RestartButton initialized");
        }
        else
        {
            Debug.LogError("YouDiedScreen: restartButton is null in Awake!");
        }
        
        if (menuButton != null)
        {
            menuButton.onClick.AddListener(OnMenuButton);
            menuButton.gameObject.SetActive(false);
            Debug.Log("YouDiedScreen: MenuButton initialized");
        }
        else
        {
            Debug.LogError("YouDiedScreen: menuButton is null in Awake!");
        }
    }
    
    public void ShowDeathScreen()
    {
        Debug.Log("YouDiedScreen.ShowDeathScreen() called");
        
        if (deathScreenPanel == null)
        {
            Debug.LogError("YouDiedScreen: deathScreenPanel is null! Please run Tools > Setup YouDiedScreen");
            return;
        }
        
        // Play death sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayYouDiedSound();
        }
        
        deathScreenPanel.SetActive(true);
        Debug.Log("YouDiedScreen: Panel activated");
        
        // Stop any existing coroutines
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }
        
        // Start fade-in animation
        fadeCoroutine = StartCoroutine(DeathScreenAnimation());
        Debug.Log("YouDiedScreen: Animation coroutine started");
    }
    
    public void HideDeathScreen()
    {
        if (deathScreenPanel != null)
        {
            deathScreenPanel.SetActive(false);
        }
        
        // Stop animations
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
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
        
        if (youDiedText != null)
        {
            youDiedText.color = new Color(textColor.r, textColor.g, textColor.b, 0f);
        }
        
        if (continueText != null)
        {
            continueText.color = new Color(1f, 1f, 1f, 0f);
        }
        
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(false);
        }
        
        if (menuButton != null)
        {
            menuButton.gameObject.SetActive(false);
        }
    }
    
    private IEnumerator DeathScreenAnimation()
    {
        Debug.Log("YouDiedScreen: Starting animation");
        
        // Fade in dark overlay
        if (darkOverlay == null)
        {
            Debug.LogError("YouDiedScreen: darkOverlay is null!");
            yield break;
        }
        
        float elapsed = 0f;
        Color startColor = new Color(overlayColor.r, overlayColor.g, overlayColor.b, 0f);
        Color targetColor = overlayColor;
        
        Debug.Log($"YouDiedScreen: Fading overlay from {startColor} to {targetColor}");
        
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / fadeInDuration;
            darkOverlay.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }
        
        darkOverlay.color = targetColor;
        Debug.Log("YouDiedScreen: Overlay fade complete");
        
        // Wait before showing text
        yield return new WaitForSecondsRealtime(textDelay);
        
        // Fade in "YOU DIED" text and background bar together
        if (youDiedText != null)
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
                youDiedText.color = Color.Lerp(textStartColor, textTargetColor, t);
                
                // Fade in background bar
                if (textBackground != null)
                {
                    textBackground.color = Color.Lerp(backgroundStartColor, backgroundTargetColor, t);
                }
                
                yield return null;
            }
            
            youDiedText.color = textTargetColor;
            if (textBackground != null)
            {
                textBackground.color = backgroundTargetColor;
            }
        }
        
        // Wait before showing continue text
        yield return new WaitForSecondsRealtime(continueTextDelay);
        
        // Show continue text with blinking effect
        if (continueText != null)
        {
            continueText.gameObject.SetActive(true);
            blinkCoroutine = StartCoroutine(BlinkContinueText());
        }
        
        // Show buttons
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(true);
        }
        
        if (menuButton != null)
        {
            menuButton.gameObject.SetActive(true);
        }
    }
    
    private IEnumerator BlinkContinueText()
    {
        if (continueText == null) yield break;
        
        while (true)
        {
            // Fade in
            float elapsed = 0f;
            while (elapsed < continueTextBlinkSpeed)
            {
                elapsed += Time.unscaledDeltaTime;
                float alpha = Mathf.Lerp(0f, 1f, elapsed / continueTextBlinkSpeed);
                continueText.color = new Color(1f, 1f, 1f, alpha);
                yield return null;
            }
            
            // Fade out
            elapsed = 0f;
            while (elapsed < continueTextBlinkSpeed)
            {
                elapsed += Time.unscaledDeltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / continueTextBlinkSpeed);
                continueText.color = new Color(1f, 1f, 1f, alpha);
                yield return null;
            }
        }
    }
    
    private void OnRestartButton()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayClickSound();
        }
        
        HideDeathScreen();
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }
    
    private void OnMenuButton()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayClickSound();
        }
        
        HideDeathScreen();
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToMenu();
        }
    }
    
    private void OnDestroy()
    {
        // Clean up button listeners
        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(OnRestartButton);
        }
        
        if (menuButton != null)
        {
            menuButton.onClick.RemoveListener(OnMenuButton);
        }
    }
}

