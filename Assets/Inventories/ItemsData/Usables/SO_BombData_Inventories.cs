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

        // Use the player's camera to determine direction and position
        Camera cam = user.GetComponentInChildren<Camera>();
        if (cam == null)
        {
            Debug.LogWarning("No Camera found on user.");
            return;
        }

        // Spawn position: in front of camera, slightly elevated
        Vector3 spawnPosition = cam.transform.position + cam.transform.forward * 2f + cam.transform.up * 0.2f;

        // Instantiate the bomb and rotate it forward
        GameObject bomb = GameObject.Instantiate(BombPrefab, spawnPosition, Quaternion.identity);

        // Apply force in the camera's forward direction
        Rigidbody rb = bomb.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(cam.transform.forward * ThrowForce, ForceMode.Impulse);
        }

        // Set bomb behavior parameters
        GO_BombBehavior_Inventories behavior = bomb.GetComponent<GO_BombBehavior_Inventories>();
        if (behavior != null)
        {
            behavior.damage = genericAmount;
            behavior.explosionRadius = DamageRangeArea;
        }
    }
}