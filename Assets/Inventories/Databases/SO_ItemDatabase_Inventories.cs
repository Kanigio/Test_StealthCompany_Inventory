using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Items/Item Database")]
public class SO_ItemDatabase_Inventories : ScriptableObject
{
    public List<SO_ItemData_Inventories> items;
    
    public SO_ItemData_Inventories GetItemByID(int id)
    {
        return items.Find(item => item.itemID == id);
    }
}
