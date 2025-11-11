using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHPBarController : MonoBehaviour
{
	[Header("References")]
	[SerializeField] private GameObject panel; // BossHPBar root (disabled by default)
	[SerializeField] private Image hpBackgroundImage; // Dark background under the red bar (full width)
	[SerializeField] private Image hpFillImage; // Image with Fill method
	[SerializeField] private TextMeshProUGUI bossNameText;
	
	private EnemyHealth boundHealth;
	private int lastMax = 1;
	private int lastCurrent = 1;
	
	private void Awake()
	{
		if (panel == null)
		{
			panel = gameObject;
		}
		
		if (hpFillImage != null)
		{
			hpFillImage.type = Image.Type.Filled;
			hpFillImage.fillMethod = Image.FillMethod.Horizontal;
			hpFillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
			hpFillImage.fillAmount = 1f;
		}
		
		// Ensure background is visible and full width
		if (hpBackgroundImage != null)
		{
			hpBackgroundImage.type = Image.Type.Filled;
			hpBackgroundImage.fillMethod = Image.FillMethod.Horizontal;
			hpBackgroundImage.fillOrigin = (int)Image.OriginHorizontal.Left;
			hpBackgroundImage.fillAmount = 1f; // always full to show max HP width
			Color c = hpBackgroundImage.color;
			if (c.a > 0.9f) // make sure it's darkened if an opaque sprite was assigned
			{
				c.a = 0.5f;
				hpBackgroundImage.color = c;
			}
		}
		
		if (panel != null)
		{
			panel.SetActive(false);
		}
	}
	
	public void Bind(EnemyHealth enemyHealth, string displayName = null)
	{
		Unbind();
		boundHealth = enemyHealth;
		if (boundHealth != null)
		{
			boundHealth.OnHealthChanged += OnHealthChanged;
			OnHealthChanged(boundHealth.CurrentHealth, boundHealth.MaxHealth);
		}
		
		if (bossNameText != null && !string.IsNullOrEmpty(displayName))
		{
			bossNameText.text = displayName;
		}
	}
	
	public void Unbind()
	{
		if (boundHealth != null)
		{
			boundHealth.OnHealthChanged -= OnHealthChanged;
			boundHealth = null;
		}
	}
	
	public void Show()
	{
		if (panel == null) panel = gameObject;
		
		// Ensure parent canvas is active (it may be disabled in the scene)
		var canvas = panel.GetComponentInParent<Canvas>(true);
		if (canvas != null && !canvas.gameObject.activeSelf)
		{
			canvas.gameObject.SetActive(true);
		}
		
		panel.SetActive(true);
	}
	
	public void Hide()
	{
		if (panel != null)
		{
			panel.SetActive(false);
		}
	}
	
	private void OnDestroy()
	{
		Unbind();
	}
	
	private void OnHealthChanged(int current, int max)
	{
		lastMax = Mathf.Max(1, max);
		lastCurrent = Mathf.Clamp(current, 0, lastMax);
		
		if (hpFillImage != null)
		{
			float fill = (float)lastCurrent / lastMax;
			hpFillImage.fillAmount = Mathf.Clamp01(fill);
		}
	}
}


