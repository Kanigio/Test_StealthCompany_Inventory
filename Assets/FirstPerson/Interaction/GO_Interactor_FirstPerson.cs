using System.Collections;
using UnityEngine;

public class GO_Interactor_FirstPerson : MonoBehaviour, I_Interactor_FirstPerson
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float interactRange = 3f;

    public I_Interactable_FirstPerson CurrentInteractable { get; private set; }

    private void Update()
    {
        DetectInteractable();
    }

    private void DetectInteractable()
    {
        CurrentInteractable = null;

        if (cameraTransform == null)
        {
            Debug.LogWarning("Interactor missing cameraTransform reference.");
            return;
        }

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange))
        {
            // Try getting any MonoBehaviour that might implement the interface
            var components = hit.collider.GetComponentsInParent<MonoBehaviour>();
            foreach (var comp in components)
            {
                if (comp is I_Interactable_FirstPerson interactable &&
                    interactable.CanInteract(gameObject))
                {
                    CurrentInteractable = interactable;
                    break;
                }
            }
        }
    }

    public void InteractWith(GameObject interactable)
    {
        var components = interactable.GetComponentsInParent<MonoBehaviour>();
        foreach (var comp in components)
        {
            if (comp is I_Interactable_FirstPerson interactableComp &&
                interactableComp.CanInteract(gameObject))
            {
                interactableComp.Interact(gameObject);
                break;
            }
        }
    }

    public void TryInteract()
    {
        if (CurrentInteractable != null)
        {
            InteractWith(((MonoBehaviour)CurrentInteractable).gameObject);
        }
    }
}