using InventoryDemo.Controllers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace InventoryDemo.UI
{
    public sealed class InventorySlotView : MonoBehaviour,
        IPointerClickHandler,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
    {
        [SerializeField] private Image itemIcon;
        [SerializeField] private TMP_Text quantityText;

        private InventoryWindowView windowView;
        private InventoryController controller;
        private bool hasItem;
        private bool isDragging;
        private int slotIndex = -1;

        public int SlotIndex => slotIndex;

        private void Awake()
        {
            windowView = GetComponentInParent<InventoryWindowView>(true);
            controller = GetComponentInParent<InventoryController>(true);
        }

        public void SetSlotIndex(int index)
        {
            slotIndex = index;
        }

        public void ShowItem(Sprite icon, int quantity)
        {
            hasItem = true;

            if (itemIcon != null)
            {
                itemIcon.sprite = icon;
                itemIcon.enabled = icon != null;
            }

            if (quantityText != null)
            {
                quantityText.enabled = true;
                quantityText.text = quantity.ToString();
            }
        }

        public void ClearItem()
        {
            hasItem = false;

            if (itemIcon != null)
            {
                itemIcon.sprite = null;
                itemIcon.enabled = false;
            }

            if (quantityText != null)
            {
                quantityText.enabled = true;
                quantityText.text = string.Empty;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (windowView != null &&
                (windowView.IsDiscardConfirmationVisible || windowView.IsSplitPanelVisible))
            {
                return;
            }

            if (eventData.button == PointerEventData.InputButton.Right &&
                hasItem &&
                slotIndex >= 0 &&
                windowView != null)
            {
                bool canSplit = controller != null && controller.CanSplitItem(slotIndex);
                windowView.ShowContextMenu(eventData.position, slotIndex, canSplit);
            }
            if (eventData.button == PointerEventData.InputButton.Left && windowView != null)
            {
                windowView.HideContextMenu();
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left ||
                !hasItem ||
                slotIndex < 0 ||
                windowView == null ||
                itemIcon == null)
            {
                return;
            }

            isDragging = windowView.BeginItemDrag(itemIcon.sprite, eventData.position);
            if (isDragging)
            {
                SetItemVisualVisible(false);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isDragging && windowView != null)
            {
                windowView.UpdateItemDrag(eventData.position);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!isDragging)
            {
                return;
            }

            isDragging = false;
            SetItemVisualVisible(true);
            windowView.EndItemDrag();

            if (!windowView.IsVisible ||
                windowView.IsDiscardConfirmationVisible ||
                windowView.IsSplitPanelVisible ||
                controller == null)
            {
                controller?.RefreshSlots();
                return;
            }

            GameObject hitObject = eventData.pointerCurrentRaycast.gameObject;
            InventorySlotView targetSlot = hitObject != null
                ? hitObject.GetComponentInParent<InventorySlotView>()
                : null;

            if (targetSlot == null)
            {
                controller.RefreshSlots();
                if (!windowView.IsScreenPointInsideInventory(eventData.position))
                {
                    windowView.ShowDiscardConfirmationForSlot(slotIndex);
                }

                return;
            }

            controller.TryMoveItem(slotIndex, targetSlot.SlotIndex);
        }

        private void OnDisable()
        {
            isDragging = false;
            SetItemVisualVisible(true);
        }

        private void SetItemVisualVisible(bool visible)
        {
            if (itemIcon != null)
            {
                itemIcon.enabled = visible && itemIcon.sprite != null;
            }

            if (quantityText != null)
            {
                quantityText.enabled = visible;
            }
        }
    }
}
