using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class S_ItemRow_Inventories
{
    // The Asset Reference
    public SO_ItemData_Inventories itemData;
    [SerializeField] private int quantity;
    public bool isUnlocked;

    // Only for Usable Items
    public int usageLeft;
    
    public bool IsEmpty()
    {
        return itemData == null || GetQuantity() <= 0;
    }
    public int GetQuantity()
    {
        return quantity;
    }

    public void SetQuantity(int newQuantity)
    {
        quantity = Mathf.Max(0, newQuantity);
        Debug.Log($"SetQuantity called with {newQuantity}, final quantity = {quantity}");
        if (quantity == 0)
        {
            Debug.Log("Quantity is 0, clearing itemData and unlocking");
            itemData = null;
            usageLeft = 0;
            isUnlocked = false;
        }
        itemData?.GetSpriteBasedOnQuantity(quantity);
    }

    public bool IsDepleted()
    {
        if (itemData.IsUsable)
        {
            return usageLeft <= 0;
        }
        return false;
    }

    // Return The ItemID from the ItemData if Valid
    public int GetItemID() => itemData != null ? itemData.itemID : -1;
    
}
