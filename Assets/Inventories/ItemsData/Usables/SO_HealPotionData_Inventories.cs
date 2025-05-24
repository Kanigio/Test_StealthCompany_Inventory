using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HealPotionData", menuName = "Items/ItemData/UsableItemData/HealPotionData")]
public class SO_HealPotionData_Inventories : SO_UsableItemData_Inventories
{
    public override void Use(GameObject user)
    {
        var health = user.GetComponent<GO_HealthComponent_FirstPerson>();
        if (health != null)
        {
            health.Heal(genericAmount);
        }
        else
        {
            Debug.LogWarning("PlayerHealth component not found on user!");
        }
    }
}
