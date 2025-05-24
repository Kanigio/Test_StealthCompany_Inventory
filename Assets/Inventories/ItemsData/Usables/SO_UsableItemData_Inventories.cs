using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UsableItemData", menuName = "Items/ItemData/UsableItemData")]
public class SO_UsableItemData_Inventories : SO_ItemData_Inventories, I_UsableItem_Inventories
{
    public float genericUseCost;
    public int maxUsage;
    public float genericAmount;

    public override bool IsUsable => true;

    public virtual void Use(GameObject user)
    {
        
    }
}
 