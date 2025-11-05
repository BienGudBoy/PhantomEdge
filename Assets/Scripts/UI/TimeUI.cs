using UnityEngine;
using TMPro;

public class TimeUI : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI timeText;

    [Header("Time Settings")]
    [SerializeField] private bool countUp = true; // true = count up, false = countdown
    [SerializeField] private float startTime = 0f; // Starting time (for countdown)
    [SerializeField] private bool startOnAwake = true;

    private float currentTime;
    private bool isRunning;

    private void Awake()
    {
        // Auto-find timeText if not assigned
        if (timeText == null)
        {
            timeText = GetComponent<TextMeshProUGUI>();
        }

        currentTime = startTime;
    }

    private void Start()
    {
        if (startOnAwake)
        {
            StartTimer();
        }

        UpdateTimeDisplay();
    }

    private void Update()
    {
        if (isRunning)
        {
            if (countUp)
            {
                currentTime += Time.deltaTime;
            }
            else
            {
                currentTime -= Time.deltaTime;
                if (currentTime <= 0f)
                {
                    currentTime = 0f;
                    StopTimer();
                    OnTimeUp();
                }
            }

            UpdateTimeDisplay();
        }
    }

    private void UpdateTimeDisplay()
    {
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);
            timeText.text = $"Time: {minutes:00}:{seconds:00}";
        }
    }

    public void StartTimer()
    {
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void PauseTimer()
    {
        isRunning = false;
    }

    public void ResumeTimer()
    {
        isRunning = true;
    }

    public void ResetTimer()
    {
        currentTime = startTime;
        UpdateTimeDisplay();
    }

    public void SetTime(float time)
    {
        currentTime = time;
        UpdateTimeDisplay();
    }

    public float GetCurrentTime()
    {
        return currentTime;
    }

    private void OnTimeUp()
    {
        // Called when countdown reaches zero
        Debug.Log("Time's up!");
        
        // You can trigger game over or other events here
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetState(GameManager.GameState.GameOver);
        }
    }
}
