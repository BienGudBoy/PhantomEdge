using UnityEngine;
using UnityEngine.SceneManagement;

public class CastleEntrance : Interactable
{
	[SerializeField] private string bossSceneName = "Scene2";
	[SerializeField] private bool alwaysAllowEntry = true;
	
	private bool selectionInProgress = false;
	
	private void Reset()
	{
		interactPrompt = "Press E for Mushroom Boss, R for Final Boss";
	}
	
	protected override void Update()
	{
		base.Update(); // keeps E-to-interact behavior (selection UI)
		
		if (canInteract && !selectionInProgress && Input.GetKeyDown(KeyCode.R))
		{
			// Directly go to Sword (final boss)
			selectionInProgress = true;
			
			if (GameManager.Instance != null)
			{
				GameManager.Instance.SetNextBoss(BossManager.BossType.Sword);
				GameManager.Instance.LoadScene(bossSceneName);
			}
			else
			{
				SceneManager.LoadScene(bossSceneName);
			}
		}
	}
	
	public override void Interact()
	{
		if (!canInteract || selectionInProgress) return;
		
		selectionInProgress = true;
		
		if (BossSelectionUI.Instance != null)
		{
			BossSelectionUI.Instance.Show(OnBossSelected);
		}
		else
		{
			Debug.LogWarning("CastleEntrance: BossSelectionUI not found, defaulting to Mushroom boss.");
			OnBossSelected(BossManager.BossType.Mushroom);
		}
		
		base.Interact();
	}
	
	private void OnBossSelected(BossManager.BossType bossType)
	{
		selectionInProgress = false;
		
		if (GameManager.Instance != null)
		{
			GameManager.Instance.SetNextBoss(bossType);
			GameManager.Instance.LoadScene(bossSceneName);
		}
		else
		{
			SceneManager.LoadScene(bossSceneName);
		}
	}
}


