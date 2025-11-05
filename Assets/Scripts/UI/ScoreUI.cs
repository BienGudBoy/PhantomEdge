using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI scoreText;

    private int currentScore;

    private void Awake()
    {
        // Auto-find scoreText if not assigned
        if (scoreText == null)
        {
            scoreText = GetComponent<TextMeshProUGUI>();
        }
    }

    private void Start()
    {
        // Subscribe to GameManager score changes
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged += UpdateScore;
            currentScore = GameManager.Instance.Score;
            UpdateScoreDisplay();
        }
        else
        {
            Debug.LogWarning("ScoreUI: GameManager not found!");
        }
    }

    private void UpdateScore(int newScore)
    {
        currentScore = newScore;
        UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore}";
        }
    }

    public void AddScore(int points)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(points);
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged -= UpdateScore;
        }
    }
}
