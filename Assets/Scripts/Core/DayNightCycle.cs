using UnityEngine;
using System.Collections;
using System;

public class DayNightCycle : MonoBehaviour
{
    [Header("Time Settings")]
    [SerializeField] private float dayDuration = 60f; // Duration in seconds before transitioning to night (1 minute)
    [SerializeField] private float nightDuration = 60f; // Duration in seconds before transitioning back to day (1 minute)
    [SerializeField] private float transitionDuration = 2f; // Duration of the fade transition
    [SerializeField] private bool startAutomatically = true; // If true, start timer immediately regardless of game state
    [SerializeField] private bool cycleContinuously = true; // If true, cycle between day and night continuously
    
    [Header("Background References")]
    [SerializeField] private Transform lightBackgroundParent; // Parent GameObject containing light backgrounds
    [SerializeField] private Transform darkBackgroundParent; // Parent GameObject containing dark backgrounds
    
    [Header("Alternative: Color Tinting")]
    [SerializeField] private bool useColorTinting = false; // If true, tint existing backgrounds instead of swapping
    [SerializeField] private Color dayColor = Color.white;
    [SerializeField] private Color nightColor = new Color(0.3f, 0.3f, 0.4f, 1f); // Darker blue tint
    
    private float elapsedTime = 0f;
    private bool isDay = true;
    private bool isTransitioning = false;
    private SpriteRenderer[] allBackgroundRenderers;
    
    // Events
    public event Action<bool> OnDayNightChanged; // true = day, false = night
    
    private void Start()
    {
        // Auto-find background parents if not assigned
        if (!useColorTinting)
        {
            if (lightBackgroundParent == null)
            {
                GameObject lightBg = GameObject.Find("Background_Light");
                if (lightBg != null)
                {
                    lightBackgroundParent = lightBg.transform;
                    Debug.Log("DayNightCycle: Auto-found Background_Light");
                }
                else
                {
                    Debug.LogWarning("DayNightCycle: Background_Light not found!");
                }
            }
            
            if (darkBackgroundParent == null)
            {
                GameObject darkBg = GameObject.Find("Background_Dark");
                if (darkBg != null)
                {
                    darkBackgroundParent = darkBg.transform;
                    Debug.Log("DayNightCycle: Auto-found Background_Dark");
                }
                else
                {
                    Debug.LogWarning("DayNightCycle: Background_Dark not found!");
                }
            }
            
            // Ensure light is active and dark is inactive
            if (lightBackgroundParent != null)
            {
                lightBackgroundParent.gameObject.SetActive(true);
                Debug.Log("DayNightCycle: Background_Light activated");
            }
            if (darkBackgroundParent != null)
            {
                darkBackgroundParent.gameObject.SetActive(false);
                Debug.Log("DayNightCycle: Background_Dark deactivated");
            }
            
            // Verify both are assigned
            if (lightBackgroundParent == null || darkBackgroundParent == null)
            {
                Debug.LogError($"DayNightCycle: Missing background references! Light: {lightBackgroundParent != null}, Dark: {darkBackgroundParent != null}");
            }
        }
        else
        {
            // Find all background renderers if using color tinting
            FindBackgroundRenderers();
        }
        
        // Subscribe to game state changes to reset timer
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged += OnGameStateChanged;
            
            // Check current state - if already playing, reset timer
            if (GameManager.Instance.CurrentState == GameManager.GameState.Playing)
            {
                ResetCycle();
                Debug.Log("DayNightCycle: Game already playing, timer reset");
            }
        }
        else
        {
            Debug.LogWarning("DayNightCycle: GameManager.Instance is null!");
        }
    }
    
    private void OnGameStateChanged(GameManager.GameState newState)
    {
        Debug.Log($"DayNightCycle: Game state changed to {newState}");
        if (newState == GameManager.GameState.Playing)
        {
            // Reset timer when game starts playing
            ResetCycle();
            Debug.Log("DayNightCycle: Game started playing, timer reset");
        }
    }
    
    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged -= OnGameStateChanged;
        }
    }
    
    private void Update()
    {
        // Check if we should run the timer
        bool shouldRun = startAutomatically;
        
        if (!startAutomatically)
        {
            // Only run if game is playing
            if (GameManager.Instance != null)
            {
                shouldRun = GameManager.Instance.CurrentState == GameManager.GameState.Playing;
            }
            else
            {
                // If no GameManager, assume we're playing (for testing)
                shouldRun = true;
            }
        }
        
        if (!shouldRun)
        {
            // Reset timer when not running
            if (elapsedTime > 0f)
            {
                elapsedTime = 0f;
            }
            return;
        }
        
        elapsedTime += Time.deltaTime;
        
        // Debug logging every 5 seconds
        int currentSecond = Mathf.FloorToInt(elapsedTime);
        if (currentSecond > 0 && currentSecond % 5 == 0 && Mathf.FloorToInt(elapsedTime * 10) % 50 == 0)
        {
            Debug.Log($"DayNightCycle: Elapsed time = {elapsedTime:F1}s, Time until night = {TimeUntilNight:F1}s, GameState = {GameManager.Instance?.CurrentState}");
        }
        
        // Check if we need to transition
        if (!isTransitioning)
        {
            if (isDay && elapsedTime >= dayDuration)
            {
                Debug.Log($"DayNightCycle: Starting transition to night at {elapsedTime:F1}s");
                StartCoroutine(TransitionToNight());
            }
            else if (!isDay && elapsedTime >= nightDuration && cycleContinuously)
            {
                Debug.Log($"DayNightCycle: Starting transition to day at {elapsedTime:F1}s");
                StartCoroutine(TransitionToDay());
            }
        }
    }
    
    private void FindBackgroundRenderers()
    {
        // Find Background GameObject in scene
        GameObject backgroundObj = GameObject.Find("Background");
        if (backgroundObj != null)
        {
            allBackgroundRenderers = backgroundObj.GetComponentsInChildren<SpriteRenderer>();
        }
    }
    
    private IEnumerator TransitionToNight()
    {
        isTransitioning = true;
        
        Debug.Log("Transitioning from day to night...");
        
        if (useColorTinting)
        {
            // Fade color tint
            yield return StartCoroutine(FadeColorTint(dayColor, nightColor));
        }
        else
        {
            // Swap backgrounds with fade
            yield return StartCoroutine(SwapBackgrounds(true));
        }
        
        isDay = false;
        elapsedTime = 0f; // Reset timer for night duration
        OnDayNightChanged?.Invoke(false);
        isTransitioning = false;
        Debug.Log("DayNightCycle: Transition to night complete. Timer reset.");
    }
    
    private IEnumerator TransitionToDay()
    {
        isTransitioning = true;
        
        Debug.Log("Transitioning from night to day...");
        
        if (useColorTinting)
        {
            // Fade color tint
            yield return StartCoroutine(FadeColorTint(nightColor, dayColor));
        }
        else
        {
            // Swap backgrounds with fade
            yield return StartCoroutine(SwapBackgrounds(false));
        }
        
        isDay = true;
        elapsedTime = 0f; // Reset timer for day duration
        OnDayNightChanged?.Invoke(true);
        isTransitioning = false;
        Debug.Log("DayNightCycle: Transition to day complete. Timer reset.");
    }
    
    private IEnumerator FadeColorTint(Color fromColor, Color toColor)
    {
        if (allBackgroundRenderers == null || allBackgroundRenderers.Length == 0)
        {
            FindBackgroundRenderers();
        }
        
        float elapsed = 0f;
        
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;
            
            Color currentColor = Color.Lerp(fromColor, toColor, t);
            
            if (allBackgroundRenderers != null)
            {
                foreach (SpriteRenderer renderer in allBackgroundRenderers)
                {
                    if (renderer != null)
                    {
                        renderer.color = currentColor;
                    }
                }
            }
            
            yield return null;
        }
        
        // Ensure final color is set
        if (allBackgroundRenderers != null)
        {
            foreach (SpriteRenderer renderer in allBackgroundRenderers)
            {
                if (renderer != null)
                {
                    renderer.color = toColor;
                }
            }
        }
    }
    
    private IEnumerator SwapBackgrounds(bool toNight)
    {
        if (lightBackgroundParent == null || darkBackgroundParent == null)
        {
            Debug.LogWarning("DayNightCycle: Light or dark background parent is not assigned!");
            yield break;
        }
        
        // Get all renderers from both backgrounds
        SpriteRenderer[] lightRenderers = lightBackgroundParent.GetComponentsInChildren<SpriteRenderer>();
        SpriteRenderer[] darkRenderers = darkBackgroundParent.GetComponentsInChildren<SpriteRenderer>();
        
        if (toNight)
        {
            // Transition to night: fade from light to dark
            // Activate dark background and set initial alpha to 0
            darkBackgroundParent.gameObject.SetActive(true);
            foreach (SpriteRenderer renderer in darkRenderers)
            {
                if (renderer != null)
                {
                    Color color = renderer.color;
                    color.a = 0f;
                    renderer.color = color;
                }
            }
            
            float elapsed = 0f;
            
            while (elapsed < transitionDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / transitionDuration;
                
                // Fade out light background
                foreach (SpriteRenderer renderer in lightRenderers)
                {
                    if (renderer != null)
                    {
                        Color color = renderer.color;
                        color.a = 1f - t;
                        renderer.color = color;
                    }
                }
                
                // Fade in dark background
                foreach (SpriteRenderer renderer in darkRenderers)
                {
                    if (renderer != null)
                    {
                        Color color = renderer.color;
                        color.a = t;
                        renderer.color = color;
                    }
                }
                
                yield return null;
            }
            
            // Deactivate light background and ensure dark is fully visible
            lightBackgroundParent.gameObject.SetActive(false);
            foreach (SpriteRenderer renderer in darkRenderers)
            {
                if (renderer != null)
                {
                    Color color = renderer.color;
                    color.a = 1f;
                    renderer.color = color;
                }
            }
        }
        else
        {
            // Transition to day: fade from dark to light
            // Activate light background and set initial alpha to 0
            lightBackgroundParent.gameObject.SetActive(true);
            foreach (SpriteRenderer renderer in lightRenderers)
            {
                if (renderer != null)
                {
                    Color color = renderer.color;
                    color.a = 0f;
                    renderer.color = color;
                }
            }
            
            float elapsed = 0f;
            
            while (elapsed < transitionDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / transitionDuration;
                
                // Fade out dark background
                foreach (SpriteRenderer renderer in darkRenderers)
                {
                    if (renderer != null)
                    {
                        Color color = renderer.color;
                        color.a = 1f - t;
                        renderer.color = color;
                    }
                }
                
                // Fade in light background
                foreach (SpriteRenderer renderer in lightRenderers)
                {
                    if (renderer != null)
                    {
                        Color color = renderer.color;
                        color.a = t;
                        renderer.color = color;
                    }
                }
                
                yield return null;
            }
            
            // Deactivate dark background and ensure light is fully visible
            darkBackgroundParent.gameObject.SetActive(false);
            foreach (SpriteRenderer renderer in lightRenderers)
            {
                if (renderer != null)
                {
                    Color color = renderer.color;
                    color.a = 1f;
                    renderer.color = color;
                }
            }
        }
    }
    
    public void ResetCycle()
    {
        elapsedTime = 0f;
        isDay = true;
        isTransitioning = false;
        
        if (useColorTinting)
        {
            if (allBackgroundRenderers != null)
            {
                foreach (SpriteRenderer renderer in allBackgroundRenderers)
                {
                    if (renderer != null)
                    {
                        renderer.color = dayColor;
                    }
                }
            }
        }
        else
        {
            if (lightBackgroundParent != null)
            {
                lightBackgroundParent.gameObject.SetActive(true);
                SpriteRenderer[] lightRenderers = lightBackgroundParent.GetComponentsInChildren<SpriteRenderer>();
                foreach (SpriteRenderer renderer in lightRenderers)
                {
                    if (renderer != null)
                    {
                        Color color = renderer.color;
                        color.a = 1f;
                        renderer.color = color;
                    }
                }
            }
            if (darkBackgroundParent != null)
            {
                darkBackgroundParent.gameObject.SetActive(false);
            }
        }
    }
    
    public bool IsDay => isDay;
    public float ElapsedTime => elapsedTime;
    public float TimeUntilNextTransition => isDay ? Mathf.Max(0f, dayDuration - elapsedTime) : Mathf.Max(0f, nightDuration - elapsedTime);
    public float TimeUntilNight => isDay ? Mathf.Max(0f, dayDuration - elapsedTime) : 0f;
}
