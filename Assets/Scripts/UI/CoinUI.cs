using UnityEngine;
using TMPro;

public class CoinUI : MonoBehaviour
{
	[Header("UI Reference")]
	[SerializeField] private TextMeshProUGUI coinText;
	
	private bool subscribed = false;
	
	private void Awake()
	{
		if (coinText == null)
		{
			coinText = GetComponent<TextMeshProUGUI>();
		}
	}
	
	private void OnEnable()
	{
		TrySubscribeAndRefresh();
	}
	
	private void Start()
	{
		TrySubscribeAndRefresh();
	}
	
	private void OnDisable()
	{
		if (subscribed && GameManager.Instance != null)
		{
			GameManager.Instance.OnCoinsChanged -= UpdateCoins;
			subscribed = false;
		}
	}
	
	private void TrySubscribeAndRefresh()
	{
		var gm = GameManager.Instance != null ? GameManager.Instance : FindFirstObjectByType<GameManager>();
		if (gm != null)
		{
			if (!subscribed)
			{
				gm.OnCoinsChanged += UpdateCoins;
				subscribed = true;
			}
			UpdateCoins(gm.Coins);
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


