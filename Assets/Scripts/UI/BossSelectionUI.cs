using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class BossSelectionUI : MonoBehaviour
{
	public static BossSelectionUI Instance;
	
	[Header("UI References")]
	[SerializeField] private GameObject panel;
	[SerializeField] private TextMeshProUGUI titleText;
	[SerializeField] private TextMeshProUGUI descriptionText;
	[SerializeField] private Button mushroomButton;
	[SerializeField] private Button swordButton;
	
	private Action<BossManager.BossType> onSelection;
	private bool isShowing = false;
	private float previousTimeScale = 1f;
	
	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		
		if (panel != null)
		{
			panel.SetActive(false);
		}
		
		if (mushroomButton != null)
		{
			mushroomButton.onClick.AddListener(() => Choose(BossManager.BossType.Mushroom));
		}
		if (swordButton != null)
		{
			swordButton.onClick.AddListener(() => Choose(BossManager.BossType.Sword));
		}
	}
	
	public void Show(Action<BossManager.BossType> selectionCallback)
	{
		if (panel == null)
		{
			Debug.LogWarning("BossSelectionUI: Panel reference not set.");
			selectionCallback?.Invoke(BossManager.BossType.Mushroom);
			return;
		}
		
		onSelection = selectionCallback;
		isShowing = true;
		
		if (UIManager.Instance != null)
		{
			UIManager.Instance.HideInteractPrompt();
		}
		
		previousTimeScale = Time.timeScale;
		Time.timeScale = 0f;
		
		panel.SetActive(true);
	}
	
	public void Hide()
	{
		if (!isShowing) return;
		
		panel?.SetActive(false);
		isShowing = false;
		onSelection = null;
		
		Time.timeScale = previousTimeScale;
	}
	
	public void CancelSelection()
	{
		if (!isShowing) return;
		var callback = onSelection;
		Hide();
		callback?.Invoke(BossManager.BossType.Mushroom);
	}
	
	private void Choose(BossManager.BossType bossType)
	{
		if (!isShowing) return;
		
		var callback = onSelection;
		Hide();
		callback?.Invoke(bossType);
	}
}


