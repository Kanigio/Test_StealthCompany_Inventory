using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GO_PickUpItem_Inventories : MonoBehaviour, I_Interactable_FirstPerson
{
    // The Data That Reference the Item
    [SerializeField] private S_ItemRow_Inventories item;
    
    [SerializeField] private string PickUpPrompt;

    public void SetItem(S_ItemRow_Inventories itemData, int quantity)
    {
        // Assign the item data
        this.item = itemData;

        // Set the new quantity
        this.item.SetQuantity(quantity);

        // Update the SpriteRenderer component if available
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null && item.itemData != null)
        {
            // Call SO method to get correct sprite based on quantity
            spriteRenderer.sprite = item.itemData.GetSpriteBasedOnQuantity(quantity);
        }
        else
        {
            Debug.LogWarning("SpriteRenderer not found or item data is missing!");
        }
    }

    public void Start()
    {
        // Update the SpriteRenderer component if available
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null && item.itemData != null)
        {
            // Call SO method to get correct sprite based on quantity
            spriteRenderer.sprite = item.itemData.GetSpriteBasedOnQuantity(item.GetQuantity());
        }
        else
        {
            Debug.LogWarning("SpriteRenderer not found or item data is missing!");
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        Interact(other.gameObject);
    }

    public void Interact(GameObject interactor)
    {
        // Try to get the inventory component from the interacting GameObject
        var inventory = interactor.GetComponent<GO_Inventory_Inventories>()
                        ?? interactor.GetComponentInParent<GO_Inventory_Inventories>();
        if (inventory != null)
        {
            // Add the item to the inventory
            inventory.AddItem(this.item.itemData, this.item.GetQuantity());

            // Optionally notify the interactor if it implements the interface
            var interactorInterface = interactor.GetComponent<I_Interactor_FirstPerson>();
            if (interactorInterface != null)
            {
                interactorInterface.InteractWith(this.gameObject);
            }

            // Destroy the pickup object after being collected
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("Interactor does not have a GO_Inventory_Inventories component!");
        }
    }

    public string GetInteractionPrompt()
    {
        return "Pick up item";
    }

    public bool CanInteract(GameObject interactor)
    {
        return false;
    }
}
