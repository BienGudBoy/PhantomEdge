using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Interactable Settings")]
    [SerializeField] protected string interactPrompt = "Press E to interact";
    [SerializeField] private bool showPrompt = true;
    
    [Header("Event")]
    public UnityEngine.Events.UnityEvent OnInteract;
    
    protected bool canInteract = false;
    protected GameObject nearbyPlayer;
    
    protected virtual void Update()
    {
        if (canInteract && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = true;
            nearbyPlayer = other.gameObject;
            
            if (showPrompt)
            {
                ShowPrompt();
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = false;
            nearbyPlayer = null;
            
            if (showPrompt)
            {
                HidePrompt();
            }
        }
    }
    
    protected virtual void ShowPrompt()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowInteractPrompt(interactPrompt);
        }
        else
        {
            Debug.Log(interactPrompt);
        }
    }
    
    protected virtual void HidePrompt()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideInteractPrompt();
        }
    }
    
    public virtual void Interact()
    {
        if (!canInteract) return;
        
        OnInteract?.Invoke();
    }
}




