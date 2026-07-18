using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace InventoryDemo.UI
{
    public sealed class InventorySlotView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image itemIcon;
        [SerializeField] private TMP_Text quantityText;

        private InventoryWindowView windowView;
        private bool hasItem;
        private int slotIndex = -1;

        private void Awake()
        {
            windowView = GetComponentInParent<InventoryWindowView>(true);
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
                quantityText.text = string.Empty;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (windowView != null && windowView.IsDiscardConfirmationVisible)
            {
                return;
            }

            if (eventData.button == PointerEventData.InputButton.Right &&
                hasItem &&
                slotIndex >= 0 &&
                windowView != null)
            {
                windowView.ShowContextMenu(eventData.position, slotIndex);
            }
            if (eventData.button == PointerEventData.InputButton.Left && windowView != null)
            {
                windowView.HideContextMenu();
            }
        }
    }
}
