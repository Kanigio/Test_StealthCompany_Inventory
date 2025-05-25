using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Main Inventory MonoBehaviour managing the item list, adding/removing, moving, and dropping logic.
/// </summary>
public class GO_Inventory_Inventories : MonoBehaviour
{
    // Reference to the item database (catalog of all possible items)
    public SO_ItemDatabase_Inventories itemDatabase;

    // Maximum number of inventory slots the player can have
    public int maxSlots = 20;

    // Prefab used when dropping an item into the world
    public GameObject itemPickupPrefab;

    // List holding all inventory slots; each slot can hold one item stack
    public List<S_ItemRow_Inventories> inventorySlots;

    // Event triggered whenever inventory is changed (e.g., UI can subscribe to this)
    public event Action OnInventoryChanged;

    private void Awake()
    {
        // Ensure the list is initialized
        if (inventorySlots == null)
            inventorySlots = new List<S_ItemRow_Inventories>();

        // Fill the list with empty but unlocked slots up to maxSlots
        while (inventorySlots.Count < maxSlots)
        {
            inventorySlots.Add(new S_ItemRow_Inventories
            {
                itemData = null,        // No item in the slot
                isUnlocked = true,      // Slot is available to use
                usageLeft = 0           // No usage left because no item is present
            });
        }

        Debug.Log($"Inventory initialized with {inventorySlots.Count} slots.");
    }

    /// <summary>
    /// Adds the specified item and quantity to the inventory.
    /// Tries to stack with existing items first, then fills empty slots, and drops overflow.
    /// </summary>
    public void AddItem(SO_ItemData_Inventories itemData, int quantity = 1)
    {
        int remainingQuantity = quantity;

        Debug.Log($"Adding {quantity} of {itemData.itemName} to inventory");

        // 1. Try to add quantity to existing stacks (not full) with the same item
        foreach (var slot in inventorySlots)
        {
            if (slot.itemData == itemData && slot.GetQuantity() < itemData.maxStack)
            {
                int spaceLeft = itemData.maxStack - slot.GetQuantity();
                int addAmount = Mathf.Min(spaceLeft, remainingQuantity);

                slot.SetQuantity(slot.GetQuantity() + addAmount);
                remainingQuantity -= addAmount;

                Debug.Log($"Added {addAmount} to existing stack, remaining: {remainingQuantity}");

                // Exit early if everything was added
                if (remainingQuantity <= 0)
                    break;
            }
        }

        // 2. Fill empty slots with what's left
        int lastRemaining = remainingQuantity;

        while (remainingQuantity > 0)
        {
            // Find first empty slot
            int emptySlotIndex = inventorySlots.FindIndex(slot => slot.IsEmpty());
            if (emptySlotIndex == -1)
            {
                Debug.LogWarning("No empty slot found, dropping remaining items.");
                break;
            }

            var emptySlot = inventorySlots[emptySlotIndex];

            int addAmount = Mathf.Min(itemData.maxStack, remainingQuantity);
            if (addAmount <= 0)
            {
                Debug.LogError("addAmount is zero or negative, breaking to avoid infinite loop.");
                break;
            }

            // Fill this slot with item and usage data
            emptySlot.itemData = itemData;
            emptySlot.SetQuantity(addAmount);
            emptySlot.usageLeft = itemData.IsUsable ? ((SO_UsableItemData_Inventories)itemData).maxUsage : 0;
            emptySlot.isUnlocked = true;

            inventorySlots[emptySlotIndex] = emptySlot;

            remainingQuantity -= addAmount;

            // Safety check to avoid infinite loop
            if (remainingQuantity == lastRemaining)
            {
                Debug.LogError("remainingQuantity didn't change, stopping to avoid infinite loop.");
                break;
            }

            lastRemaining = remainingQuantity;
        }

        // 3. Drop any leftover items that couldn't fit
        if (remainingQuantity > 0)
        {
            DropItem(itemData, remainingQuantity);
        }

        // Notify listeners (like UI) that inventory changed
        OnInventoryChanged?.Invoke();
    }

    /// <summary>
    /// Drops item into the game world using a prefab when inventory is full.
    /// </summary>
    private void DropItem(SO_ItemData_Inventories itemData, int quantity)
    {
        Debug.Log($"[DropItem] Dropping {itemData.itemName} x{quantity}");

        if (itemPickupPrefab == null)
        {
            Debug.LogWarning("Item pickup prefab not assigned!");
            return;
        }

        // Drop item slightly in front of the inventory holder
        Vector3 dropPosition = transform.position + transform.forward * 2 + transform.up;
        GameObject pickup = Instantiate(itemPickupPrefab, dropPosition, Quaternion.identity);

        // Assign item and quantity to the pickup
        var pickupComponent = pickup.GetComponent<GO_PickUpItem_Inventories>();
        if (pickupComponent != null)
        {
            var itemRow = new S_ItemRow_Inventories()
            {
                itemData = itemData,
                isUnlocked = true,
                usageLeft = itemData.IsUsable ? ((SO_UsableItemData_Inventories)itemData).maxUsage : 0
            };
            pickupComponent.SetItem(itemRow, quantity);
        }
    }

    /// <summary>
    /// Removes a given quantity of an item from its inventory slot.
    /// </summary>
    public void RemoveItem(S_ItemRow_Inventories itemRow, int quantity)
    {
        if (itemRow == null || itemRow.itemData == null)
            return;

        itemRow.SetQuantity(itemRow.GetQuantity() - quantity);

        // If quantity falls to zero, clear the slot
        if (itemRow.GetQuantity() <= 0)
        {
            itemRow.itemData = null;
            itemRow.SetQuantity(0);
            itemRow.usageLeft = 0;
            itemRow.isUnlocked = false;
        }

        OnInventoryChanged?.Invoke();
    }

    /// <summary>
    /// Moves an item from one slot to another.
    /// Merges items if same type and not full, otherwise swaps them.
    /// </summary>
    public void MoveItem(int fromIndex, int toIndex)
    {
        // Validate slot indices
        if (fromIndex < 0 || fromIndex >= inventorySlots.Count) return;
        if (toIndex < 0 || toIndex >= inventorySlots.Count) return;
        if (fromIndex == toIndex) return;

        var fromSlot = inventorySlots[fromIndex];
        var toSlot = inventorySlots[toIndex];

        // Case 1: Move directly if target is empty
        if (toSlot.IsEmpty())
        {
            inventorySlots[toIndex] = fromSlot;
            inventorySlots[fromIndex] = new S_ItemRow_Inventories() { isUnlocked = false };
        }
        // Case 2: Merge stacks if same item type and not full
        else if (fromSlot.itemData == toSlot.itemData && toSlot.GetQuantity() < toSlot.itemData.maxStack)
        {
            int combined = fromSlot.GetQuantity() + toSlot.GetQuantity();
            int maxStack = toSlot.itemData.maxStack;

            if (combined <= maxStack)
            {
                toSlot.SetQuantity(combined);
                inventorySlots[toIndex] = toSlot;
                inventorySlots[fromIndex] = new S_ItemRow_Inventories() { isUnlocked = false };
            }
            else
            {
                toSlot.SetQuantity(maxStack);
                inventorySlots[toIndex] = toSlot;

                int remainder = combined - maxStack;

                // Try to find a free slot for remainder
                var emptySlot = inventorySlots.Find(slot => slot.IsEmpty());
                if (emptySlot != null)
                {
                    int emptySlotIndex = inventorySlots.IndexOf(emptySlot);
                    inventorySlots[emptySlotIndex] = new S_ItemRow_Inventories()
                    {
                        itemData = fromSlot.itemData,
                        isUnlocked = true,
                        usageLeft = fromSlot.usageLeft
                    };
                    inventorySlots[emptySlotIndex].SetQuantity(remainder);
                    inventorySlots[fromIndex] = new S_ItemRow_Inventories() { isUnlocked = false };
                }
                else
                {
                    // No room for remainder: leave it in the original slot
                    fromSlot.SetQuantity(remainder);
                    inventorySlots[fromIndex] = fromSlot;
                }
            }
        }
        // Case 3: Items are different or cannot be merged: swap them
        else
        {
            inventorySlots[fromIndex] = toSlot;
            inventorySlots[toIndex] = fromSlot;
        }

        OnInventoryChanged?.Invoke();
    }

    /// <summary>
    /// Split an Item and move it from one slot to another.
    /// Merges items if same type and not full, otherwise swaps them.
    /// </summary>
    public void SplitItemQuantity(int fromIndex, int toIndex)
    {
        // Validate indices
        if (fromIndex < 0 || fromIndex >= inventorySlots.Count) return;
        if (toIndex < 0 || toIndex >= inventorySlots.Count) return;
        if (fromIndex == toIndex) return;

        var fromSlot = inventorySlots[fromIndex];
        var toSlot = inventorySlots[toIndex];

        if (fromSlot.IsEmpty()) return;

        int fromQuantity = fromSlot.GetQuantity();
        if (fromQuantity <= 1) return; // Not enough to split

        int splitAmount = fromQuantity / 2;
        int remainingAmount = fromQuantity - splitAmount;

        // Remove half from source
        fromSlot.SetQuantity(remainingAmount);
        inventorySlots[fromIndex] = fromSlot;

        // Prepare split stack
        S_ItemRow_Inventories splitStack = new S_ItemRow_Inventories()
        {
            itemData = fromSlot.itemData,
            isUnlocked = true,
            usageLeft = fromSlot.usageLeft
        };
        splitStack.SetQuantity(splitAmount);

        // Case 1: Target is empty → place split
        if (toSlot.IsEmpty())
        {
            inventorySlots[toIndex] = splitStack;
        }
        // Case 2: Same item and target has space → merge
        else if (toSlot.itemData == fromSlot.itemData && toSlot.GetQuantity() < toSlot.itemData.maxStack)
        {
            int availableSpace = toSlot.itemData.maxStack - toSlot.GetQuantity();
            int toAdd = Mathf.Min(availableSpace, splitAmount);
            toSlot.SetQuantity(toSlot.GetQuantity() + toAdd);
            inventorySlots[toIndex] = toSlot;

            int leftover = splitAmount - toAdd;
            if (leftover > 0)
            {
                // Try to place leftover in a new empty slot
                int emptyIndex = inventorySlots.FindIndex(slot => slot.IsEmpty());
                if (emptyIndex != -1)
                {
                    S_ItemRow_Inventories leftoverStack = new S_ItemRow_Inventories()
                    {
                        itemData = fromSlot.itemData,
                        isUnlocked = true,
                        usageLeft = fromSlot.usageLeft
                    };
                    leftoverStack.SetQuantity(leftover);
                    inventorySlots[emptyIndex] = leftoverStack;
                }
                else
                {
                    // No space: put leftover back in original slot
                    fromSlot.SetQuantity(fromSlot.GetQuantity() + leftover);
                    inventorySlots[fromIndex] = fromSlot;
                }
            }
        }
        // Case 3: Cannot merge → try placing in another empty slot
        else
        {
            int emptyIndex = inventorySlots.FindIndex(slot => slot.IsEmpty());
            if (emptyIndex != -1)
            {
                inventorySlots[emptyIndex] = splitStack;
            }
            else
            {
                // Nowhere to place → drop or put back
                fromSlot.SetQuantity(fromSlot.GetQuantity() + splitAmount);
                inventorySlots[fromIndex] = fromSlot;
                // Optional: Trigger drop behavior here
            }
        }

        OnInventoryChanged?.Invoke();
    }

    /// <summary>
    /// Sorts the inventory by pushing empty slots to the end of the list.
    /// </summary>
    public void UpdateList()
    {
        inventorySlots.Sort((a, b) =>
        {
            if (a.IsEmpty() && b.IsEmpty()) return 0;
            if (a.IsEmpty()) return 1;
            if (b.IsEmpty()) return -1;
            return 0; // Maintain order of non-empty slots
        });

        OnInventoryChanged?.Invoke();
    }
    
    public void NotifyInventoryChanged()
    {
        OnInventoryChanged?.Invoke();
    }
}