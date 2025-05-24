using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class GO_Inventory_UI : MonoBehaviour
{
    public GO_Inventory_Inventories inventory;
    public GameObject slotPrefab;
    public Transform slotParent;

    private List<GameObject> slotInstances = new List<GameObject>();

    private void Start()
    {
        StartCoroutine(WaitForInventory());
    }

    private IEnumerator WaitForInventory()
    {
        while (Sgl_GameMode_FirstPerson.Instance == null ||
               Sgl_GameMode_FirstPerson.Instance.PlayerCharacter == null)
        {
            yield return null;
        }

        inventory = Sgl_GameMode_FirstPerson.Instance.PlayerCharacter.GetComponent<GO_Inventory_Inventories>();

        if (inventory != null)
        {
            inventory.OnInventoryChanged += RenderInventory;
            RenderInventory(); // Initial draw
        }
        else
        {
            Debug.LogError("Could not find inventory on PlayerCharacter.");
        }
    }

    private void OnDisable()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged -= RenderInventory;
        }
    }

    public void RenderInventory()
    {
        if (slotPrefab == null || slotParent == null || inventory == null)
        {
            Debug.LogError("Inventory UI not configured properly.");
            return;
        }

        foreach (Transform child in slotParent)
        {
            Destroy(child.gameObject);
        }

        slotInstances.Clear();

        foreach (var slot in inventory.inventorySlots)
        {
            GameObject slotGO = Instantiate(slotPrefab, slotParent);
            slotInstances.Add(slotGO);

            // Connect drag handler
            var dragHandler = slotGO.AddComponent<GO_InventorySlotDragHandler_UI>();
            dragHandler.slotIndex = slotInstances.Count - 1;
            dragHandler.inventoryUI = this;

            // Required: CanvasGroup for drag to work properly
            if (!slotGO.GetComponent<CanvasGroup>())
                slotGO.AddComponent<CanvasGroup>();

            // Assign icon and quantity
            Image icon = slotGO.transform.Find("Icon")?.GetComponent<Image>();
            TMP_Text quantityText = slotGO.transform.Find("QuantityText")?.GetComponent<TMP_Text>();

            if (icon == null || quantityText == null)
            {
                Debug.LogError("Missing Icon or QuantityText components in prefab.");
                continue;
            }

            if (slot.itemData != null)
            {
                icon.sprite = slot.itemData.GetSpriteBasedOnQuantity(slot.GetQuantity());
                icon.enabled = true;
                quantityText.text = slot.GetQuantity().ToString();
            }
            else
            {
                icon.enabled = false;
                quantityText.text = "";
            }
        }
    }
}
