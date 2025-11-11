using UnityEngine;
using TMPro;

public class CoinUI : MonoBehaviour
{
	[Header("UI Reference")]
	[SerializeField] private TextMeshProUGUI coinText;
	
	private void Awake()
	{
		if (coinText == null)
		{
			coinText = GetComponent<TextMeshProUGUI>();
		}
	}
	
	private void OnEnable()
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.OnCoinsChanged += UpdateCoins;
			UpdateCoins(GameManager.Instance.Coins);
		}
	}
	
	private void OnDisable()
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.OnCoinsChanged -= UpdateCoins;
		}
	}
	
	private void UpdateCoins(int amount)
	{
		if (coinText != null)
		{
			coinText.text = $"Coins: {amount}";
		}
	}
}


