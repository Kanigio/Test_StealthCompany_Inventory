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
        UpdateSprite();
    }

    public void OnTriggerEnter(Collider other)
    {
        Interact(other.gameObject);
    }

    public void Interact(GameObject interactor)
    {
        // First, try to add to player's inventory
        var inventory = interactor.GetComponent<GO_Inventory_Inventories>()
                        ?? interactor.GetComponentInParent<GO_Inventory_Inventories>();

        if (inventory != null)
        {
            inventory.AddItem(this.item.itemData, this.item.GetQuantity());
            Destroy(gameObject);
            return;
        }

        // Else: try to merge with another pickup
        var otherPickup = interactor.GetComponentInParent<GO_PickUpItem_Inventories>();
        if (otherPickup != null && otherPickup != this)
        {
            var otherItem = otherPickup.item;

            // Check if same item type
            if (this.item.itemData == otherItem.itemData)
            {
                int maxStack = item.itemData.maxStack;
                int currentOtherQty = otherItem.GetQuantity();
                int thisQty = this.item.GetQuantity();

                if (currentOtherQty < maxStack)
                {
                    int spaceLeft = maxStack - currentOtherQty;
                    int transferAmount = Mathf.Min(spaceLeft, thisQty);

                    // Transfer amount
                    otherItem.SetQuantity(currentOtherQty + transferAmount);
                    this.item.SetQuantity(thisQty - transferAmount);

                    // Update sprite for both pickups
                    otherPickup.UpdateSprite();
                    UpdateSprite();

                    // Destroy this if empty
                    if (this.item.GetQuantity() <= 0)
                    {
                        Destroy(gameObject);
                    }

                    return;
                }
            }
        }

        Debug.LogWarning("Interactor is not a valid inventory or compatible pickup.");
    }

    public string GetInteractionPrompt()
    {
        return "Pick up item";
    }

    public bool CanInteract(GameObject interactor)
    {
        return false;
    }

    private void UpdateSprite()
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
}
