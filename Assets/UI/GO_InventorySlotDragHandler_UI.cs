using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class GO_InventorySlotDragHandler_UI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public int slotIndex; // Assigned by inventory UI when creating slots
    public GO_Inventory_UI inventoryUI; // Reference to the inventory UI script

    private CanvasGroup canvasGroup;  // Used to control transparency and block raycasts during drag
    private GameObject dragIcon;      // The floating icon that follows the mouse while dragging
    private static GameObject currentDragIcon; // Static to ensure only one drag icon exists at a time
    private static GO_InventorySlotDragHandler_UI currentDraggedHandler; // Reference to the dragged slot handler
    
    private PointerEventData.InputButton currentDragButton;
    private bool isSplitDrag = false;
    
    private int draggingQauntity = 0;
    private S_ItemRow_Inventories draggingItemData;

    private void Awake()
    {
        // Ensure this GameObject has a CanvasGroup to manipulate alpha and raycast blocking
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        currentDragButton = eventData.button;
        isSplitDrag = (currentDragButton == PointerEventData.InputButton.Right);
        
        // Destroy any existing drag icon before creating a new one (prevents duplicates)
        if (currentDragIcon != null)
        {
            Destroy(currentDragIcon);
            currentDragIcon = null;
        }
        if (inventoryUI.inventory.inventorySlots[slotIndex].IsEmpty())
        {
            return;
        }

        // Create a new GameObject to visually represent the dragged icon
        dragIcon = new GameObject("DragIcon");

        // Parent the drag icon to the main UI Canvas
        Canvas mainCanvas = GetComponentInParent<Canvas>();
        if (mainCanvas == null)
            mainCanvas = inventoryUI.GetComponentInParent<Canvas>();

        dragIcon.transform.SetParent(mainCanvas.transform, false);
        dragIcon.transform.SetAsLastSibling();

        // Add Image component and set the sprite
        var image = dragIcon.AddComponent<Image>();
        image.sprite = inventoryUI.inventory.inventorySlots[slotIndex].itemData.GetSpriteBasedOnQuantity(
            inventoryUI.inventory.inventorySlots[slotIndex].GetQuantity());
        image.raycastTarget = false;

        // Configure transform
        RectTransform rt = dragIcon.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(64, 64);
        dragIcon.transform.localScale = Vector3.one;
        dragIcon.transform.position = Input.mousePosition;
        
        // Add Text to Display the quantity
        GameObject quantityTextGO = new GameObject("QuantityText");
        quantityTextGO.transform.SetParent(dragIcon.transform, false);
        TextMeshProUGUI quantityTMP = quantityTextGO.AddComponent<TextMeshProUGUI>();
        
        int fullQuantity = inventoryUI.inventory.inventorySlots[slotIndex].GetQuantity();
        int displayedQuantity = isSplitDrag ? fullQuantity / 2 : fullQuantity;
        
        draggingQauntity = displayedQuantity;
        draggingItemData = inventoryUI.inventory.inventorySlots[slotIndex];
        
        quantityTMP.text = displayedQuantity.ToString();
        quantityTMP.fontSize = 18;
        quantityTMP.color = Color.white;
        quantityTMP.alignment = TextAlignmentOptions.BottomRight;
        quantityTMP.raycastTarget = false;

        // Optional: Black Border for Better visual
        var outline = quantityTextGO.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(1f, -1f);

        // Extend all RectTranform to contain the Icon
        RectTransform quantityRT = quantityTextGO.GetComponent<RectTransform>();
        quantityRT.anchorMin = Vector2.zero;
        quantityRT.anchorMax = Vector2.one;
        quantityRT.offsetMin = new Vector2(0, -10); 
        quantityRT.offsetMax = new Vector2(10, 0); 

        // Track current drag globally
        currentDragIcon = dragIcon;
        currentDraggedHandler = this;

        // Make the original slot semi-transparent and ignore raycasts
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
        {
            dragIcon.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // NOTE: Only called if drop target is invalid (or null)

        if (currentDragIcon != null)
        {
            Destroy(currentDragIcon);
            currentDragIcon = null;
        }
        dragIcon = null;
        currentDraggedHandler = null;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        
        if (draggingItemData.itemData == null) return;
        inventoryUI.inventory.DropItem(draggingItemData.itemData, draggingQauntity);
        inventoryUI.inventory.RemoveItem(draggingItemData, draggingQauntity);

        inventoryUI?.RenderInventory();
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Only process valid drops from other slots
        var dragged = eventData.pointerDrag?.GetComponent<GO_InventorySlotDragHandler_UI>();
        if (dragged == null || dragged.slotIndex == slotIndex)
            return;

        // Move item data in inventory
        if (dragged.isSplitDrag)
        {
            // Call Split if Right Click
            inventoryUI.inventory.SplitItemQuantity(dragged.slotIndex, slotIndex);
        }
        else
        {
            // Otherwise move normally
            inventoryUI.inventory.MoveItem(dragged.slotIndex, slotIndex);
        }

        // Cleanup drag icon manually, because OnEndDrag won't be called
        if (currentDragIcon != null)
        {
            Destroy(currentDragIcon);
            currentDragIcon = null;
        }

        if (currentDraggedHandler != null)
        {
            currentDraggedHandler.canvasGroup.alpha = 1f;
            currentDraggedHandler.canvasGroup.blocksRaycasts = true;
            currentDraggedHandler.dragIcon = null;
            currentDraggedHandler = null;
        }

        // Refresh UI once after successful drop
        inventoryUI.RenderInventory();
    }
}

