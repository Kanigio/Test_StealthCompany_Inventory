using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_Interactable_FirstPerson
{
    // The Functions to Implements in the Interactable Objects
    // The Function Called From The Interactor
    void Interact(GameObject interactor);
    // Function To Get The Interaction Propmt. es “Press E”
    string GetInteractionPrompt();
    // Function That Returns a bool
    bool CanInteract(GameObject interactor);
}
