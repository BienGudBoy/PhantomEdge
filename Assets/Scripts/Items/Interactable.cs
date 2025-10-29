using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Interactable Settings")]
    [SerializeField] private string interactPrompt = "Press E to interact";
    [SerializeField] private bool showPrompt = true;
    
    [Header("Event")]
    public UnityEngine.Events.UnityEvent OnInteract;
    
    private bool canInteract = false;
    private GameObject nearbyPlayer;
    
    private void Update()
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
                // TODO: Show UI prompt
                Debug.Log(interactPrompt);
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = false;
            nearbyPlayer = null;
        }
    }
    
    public virtual void Interact()
    {
        if (!canInteract) return;
        
        OnInteract?.Invoke();
    }
}




