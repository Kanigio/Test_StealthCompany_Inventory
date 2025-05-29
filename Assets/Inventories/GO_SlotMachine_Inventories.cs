using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GO_SlotMachine_Inventories : MonoBehaviour, I_Interactable_FirstPerson
{
    [SerializeField] private string interactionPrompt = "Press F to get a random item";
    [SerializeField] private bool useCustomDatabase = false;
    [SerializeField] private SO_ItemDatabase_Inventories customItemDatabase;
    
    [SerializeField] private int MinDropAmount = 1;
    [SerializeField] private int MaxDropAmount = 3;
    
    [SerializeField] private GameObject itemPickupPrefab;
    

    public bool CanInteract(GameObject interactor)
    {
        return true;
    }

    public void Interact(GameObject interactor)
    {
        Debug.Log("SlotMachine Activated!");

        SO_ItemDatabase_Inventories itemDatabase = useCustomDatabase ? customItemDatabase : Sgl_GameMode_FirstPerson.Instance?.ItemDatabase;
        if (itemDatabase == null || itemDatabase.AllItems == null || itemDatabase.AllItems.Count == 0)
        {
            Debug.LogWarning("No items available in the item database.");
            return;
        }

        // Pick a random item
        var randomItem = itemDatabase.AllItems[Random.Range(0, itemDatabase.AllItems.Count)];
        if (randomItem == null)
        {
            Debug.LogWarning("Random item was null.");
            return;
        }

        // Instantiate pickup
        // Base position: center of the character
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        // Random angle in degrees around forward direction (-45° to +45°)
        float angle = Random.Range(-45f, 45f);

        // Rotate the forward vector by that angle around the Y-axis
        Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;

        // Final drop distance
        float dropDistance = 1.5f;

        // Final position
        Vector3 dropPosition = origin + direction.normalized * dropDistance;

        // Instantiate at final position
        GameObject pickup = Instantiate(itemPickupPrefab, dropPosition, Quaternion.identity);

        var pickupComponent = pickup.GetComponent<GO_PickUpItem_Inventories>();
        if (pickupComponent != null)
        {
            var itemRow = new S_ItemRow_Inventories()
            {
                itemData = randomItem,
                isUnlocked = true,
                usageLeft = randomItem.IsUsable ? ((SO_UsableItemData_Inventories)randomItem).maxUsage : 0
            };
            
            int randomQuantity = Random.Range(MinDropAmount, MaxDropAmount + 1); // RanRange int Exclude the max amount
            pickupComponent.SetItem(itemRow, randomQuantity);
        }
        else
        {
            Debug.LogWarning("Pickup prefab does not contain GO_PickUpItem_Inventories component.");
        }
    }

    public string GetInteractionPrompt()
    {
        return interactionPrompt;
    }
}
