using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BombData", menuName = "Items/ItemData/UsableItemData/BombData")]
public class SO_BombData_Inventories : SO_UsableItemData_Inventories
{
    [SerializeField] private float DamageRangeArea = 5f;
    [SerializeField] private float ThrowForce = 10f;
    [SerializeField] private GameObject BombPrefab;

    public override void Use(GameObject user)
    {
        if (BombPrefab == null)
        {
            Debug.LogWarning("BombPrefab is not assigned!");
            return;
        }

        Transform throwOrigin = user.transform; // Puoi usare la mano/camera per un origin migliore
        GameObject bomb = Instantiate(BombPrefab, throwOrigin.position + throwOrigin.forward, Quaternion.identity);

        Rigidbody rb = bomb.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(throwOrigin.forward * ThrowForce, ForceMode.Impulse);
        }

        // Passa parametri alla bomba
        GO_BombBehavior_Inventories behavior = bomb.GetComponent<GO_BombBehavior_Inventories>();
        if (behavior != null)
        {
            behavior.damage = genericAmount;
            behavior.explosionRadius = DamageRangeArea;
        }
    }
}