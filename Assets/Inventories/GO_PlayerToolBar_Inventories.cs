using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;


public class GO_PlayerToolBar_Inventories : MonoBehaviour
{
    // Currently selected usable item
    public S_ItemRow_Inventories CurrentSelectedItem;

    // Current index in the inventory
    int currentIndex = 0;

    // Reference to the Inventory
    public GO_Inventory_Inventories Inventory;
    
    public event Action OnToolbarChanged;
    
    public int GetCurrentIndex() => currentIndex;

    public void ScrollItems(bool bForward)
    {
        if (Inventory == null || Inventory.inventorySlots == null || Inventory.inventorySlots.Count == 0)
            return;

        int slotsCount = Inventory.inventorySlots.Count;
        int originalIndex = currentIndex;

        // Forward scroll
        if (bForward)
        {
            for (int i = 1; i <= slotsCount; i++)
            {
                int newIndex = (currentIndex + i) % slotsCount;
                var item = Inventory.inventorySlots[newIndex];

                if (item != null && !item.IsEmpty() && item.itemData.IsUsable)
                {
                    CurrentSelectedItem = item;
                    currentIndex = newIndex;
                    break;
                }
            }
        }
        // Backward scroll
        else
        {
            for (int i = 1; i <= slotsCount; i++)
            {
                int newIndex = (currentIndex - i + slotsCount) % slotsCount;
                var item = Inventory.inventorySlots[newIndex];

                if (item != null && !item.IsEmpty() && item.itemData.IsUsable)
                {
                    CurrentSelectedItem = item;
                    currentIndex = newIndex;
                    break;
                }
            }
        }
        OnToolbarChanged?.Invoke(); //Notify UI
    }

    public void UseSelectedItem()
    {
        if (CurrentSelectedItem == null || CurrentSelectedItem.itemData == null)
            return;

        var usableItem = CurrentSelectedItem.itemData as SO_UsableItemData_Inventories;
        if (usableItem == null)
        {
            Debug.LogWarning("Selected item is not a usable item.");
            return;
        }

        // Get the player GameObject from parent component
        var player = GetComponentInParent<GO_PlayerCharacter_FirstPerson>();
        if (player == null)
        {
            Debug.LogWarning("Could not find player component.");
            return;
        }

        usableItem.Use(player.gameObject);

        // Get index of current selected item
        int index = Inventory.inventorySlots.IndexOf(CurrentSelectedItem);
        if (index == -1)
        {
            Debug.LogWarning("Selected item not found in inventory.");
            return;
        }

        // Decrease usage
        var itemRow = Inventory.inventorySlots[index];
        itemRow.usageLeft--;

        if (itemRow.IsDepleted())
        {
            itemRow.SetQuantity(itemRow.GetQuantity() - 1);
        }

        // Update slot back to inventory
        Inventory.inventorySlots[index] = itemRow;

        // Optional: refresh UI if needed
        Inventory.NotifyInventoryChanged();
        OnToolbarChanged?.Invoke(); // Trigger update
    }
    
    public void NotifyToolbarChanged()
    {
        OnToolbarChanged?.Invoke();
    }
}
