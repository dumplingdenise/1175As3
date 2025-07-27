using UnityEngine;

public class TutorialPlayerInteract : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private IInteractable currentInteractable;
    private bool isNearInteractable = false;

    void Update()
    {
        if (isNearInteractable && currentInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Pressed E near NPC");
            if (currentInteractable.CanInteract())
            {
                currentInteractable.Interact();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Found interactable: " + other.name);
        if (other.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            Debug.Log("Found interactable: " + other.name);
            isNearInteractable = true;
            currentInteractable = interactable;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<IInteractable>(out IInteractable interactable) && interactable == currentInteractable)
        {
            isNearInteractable = false;
            currentInteractable = null;
        }
    }
}
