using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Items/ItemData")]
public class SO_ItemData_Inventories : ScriptableObject
{
    public int itemID;
    public string itemName;
    public Sprite icon;
    public int value;
    public int maxStack;

    public virtual bool IsUsable => false;
    
    // Virtual method to update sprite based on value, can be overridden
    public virtual Sprite GetSpriteBasedOnQuantity(int quantity)
    {
        // Default implementation returns the icon
        return icon;
    }
}
