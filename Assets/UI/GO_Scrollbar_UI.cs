using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;


public class GO_Scrollbar_UI : MonoBehaviour
{
     [SerializeField] private Image previousIcon;
    [SerializeField] private Image currentIcon;
    [SerializeField] private Image nextIcon;

    [SerializeField] private TMP_Text previousQuantity;
    [SerializeField] private TMP_Text currentQuantity;
    [SerializeField] private TMP_Text nextQuantity;

    private GO_PlayerToolBar_Inventories toolbarLogic;
    
    [SerializeField] private Image previousBackground;
    [SerializeField] private Image currentBackground;
    [SerializeField] private Image nextBackground;

    public void UpdateToolbarUI()
    {
        var inventory = toolbarLogic.Inventory;
        if (inventory == null || inventory.inventorySlots.Count == 0)
            return;

        var currentIndex = toolbarLogic.GetCurrentIndex();
        var slots = inventory.inventorySlots;
        int count = slots.Count;

        int prevIndex = GetNextUsableIndex(currentIndex, -1);
        int nextIndex = GetNextUsableIndex(currentIndex, +1);

        // Helper
        void SetSlotDisplay(int index, Image icon, TMP_Text quantity, Image background)
        {
            if (index >= 0)
            {
                var slot = slots[index];
                if (slot != null && slot.itemData != null && slot.itemData.IsUsable)
                {
                    icon.sprite = slot.itemData.GetSpriteBasedOnQuantity(slot.GetQuantity());
                    icon.enabled = true;
                    quantity.text = slot.GetQuantity().ToString();

                    if (background != null)
                        background.enabled = true;

                    return;
                }
            }

            // Se non valido, nascondi
            icon.enabled = false;
            quantity.text = "";
        }
        
        SetSlotDisplay(prevIndex, previousIcon, previousQuantity, previousBackground);
        SetSlotDisplay(currentIndex, currentIcon, currentQuantity, currentBackground);
        SetSlotDisplay(nextIndex, nextIcon, nextQuantity, nextBackground);
    }

    private void Start()
    {
        StartCoroutine(WaitForToolbar());
    }

    private IEnumerator WaitForToolbar()
    {
        while (Sgl_GameMode_FirstPerson.Instance == null ||
               Sgl_GameMode_FirstPerson.Instance.PlayerCharacter == null)
        {
            yield return null;
        }

        toolbarLogic = Sgl_GameMode_FirstPerson.Instance.PlayerCharacter.GetComponent<GO_PlayerToolBar_Inventories>();

        if (toolbarLogic != null)
        {
                toolbarLogic.OnToolbarChanged += UpdateToolbarUI;
                UpdateToolbarUI();
        }
        else
        {
            Debug.LogError("Could not find Toolbar on PlayerCharacter.");
        }
    }
    
    private void OnDestroy()
    {
        if (toolbarLogic != null)
            toolbarLogic.OnToolbarChanged -= UpdateToolbarUI;
    }
    
    private int GetNextUsableIndex(int fromIndex, int direction)
    {
        var slots = toolbarLogic.Inventory.inventorySlots;
        int count = slots.Count;

        for (int i = 1; i < count; i++)
        {
            int newIndex = (fromIndex + i * direction + count) % count;
            var item = slots[newIndex];
            if (item != null && !item.IsEmpty() && item.itemData.IsUsable)
                return newIndex;
        }

        return -1; // no usable item found in that direction
    }
}
