using UnityEngine;

public class CastleEntrance : Interactable
{
	[SerializeField] private string bossSceneName = "Scene2";
	[SerializeField] private bool alwaysAllowEntry = true;
	
	private void Reset()
	{
		interactPrompt = "Press E to enter the castle (Boss Fight)";
	}
	
	public override void Interact()
	{
		if (!canInteract) return;
		
		// Entry is always allowed per design, regardless of buffs
		if (GameManager.Instance != null)
		{
			GameManager.Instance.LoadScene(bossSceneName);
		}
		else
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene(bossSceneName);
		}
		
		base.Interact();
	}
}


