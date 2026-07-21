using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

namespace InventoryDemo.UI
{
    public sealed class InventoryWindowView : MonoBehaviour
    {
        [SerializeField] private GameObject inventoryWindow;
        [SerializeField] private RectTransform contextMenu;
        [SerializeField] private GameObject discardConfirmationPanel;
        [SerializeField] private TMP_InputField searchInputField;
        [SerializeField] private Image dragIcon;

        private RectTransform canvasRect;
        private Canvas canvas;
        private int? currentTargetSlotIndex;

        public bool IsVisible => inventoryWindow != null && inventoryWindow.activeSelf;
        public bool IsDiscardConfirmationVisible =>
            discardConfirmationPanel != null && discardConfirmationPanel.activeSelf;
        public int? CurrentTargetSlotIndex => currentTargetSlotIndex;

        public void Configure(GameObject window, RectTransform menu)
        {
            inventoryWindow = window;
            contextMenu = menu;
        }

        private void Awake()
        {
            canvas = GetComponent<Canvas>();
            canvasRect = transform as RectTransform;

            EndItemDrag();
            SetVisible(false);
        }

        private void Update()
        {
            if (Keyboard.current == null || !Keyboard.current.bKey.wasPressedThisFrame)
            {
                return;
            }

            if (searchInputField != null && searchInputField.isFocused)
            {
                return;
            }

            ToggleInventory();
        }

        public void ToggleInventory()
        {
            SetVisible(!IsVisible);
        }

        public void SetVisible(bool visible)
        {
            if (inventoryWindow == null)
            {
                return;
            }

            if (!visible)
            {
                EndItemDrag();
            }

            inventoryWindow.SetActive(visible);
            if (!visible)
            {
                HideContextMenu();
                CloseDiscardConfirmation();
            }
        }

        public void ShowContextMenu(Vector2 screenPosition, int targetSlotIndex)
        {
            if (!IsVisible || contextMenu == null || canvasRect == null || canvas == null)
            {
                return;
            }

            Camera eventCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : canvas.worldCamera;

            RectTransform menuParent = contextMenu.parent as RectTransform;
            if (menuParent == null ||
                !RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    menuParent,
                    screenPosition,
                    eventCamera,
                    out Vector2 localPosition))
            {
                return;
            }

            Vector2 menuHalfSize = contextMenu.rect.size * 0.5f;
            Rect canvasBounds = menuParent.rect;
            localPosition.x = Mathf.Clamp(
                localPosition.x,
                canvasBounds.xMin + menuHalfSize.x,
                canvasBounds.xMax - menuHalfSize.x);
            localPosition.y = Mathf.Clamp(
                localPosition.y,
                canvasBounds.yMin + menuHalfSize.y,
                canvasBounds.yMax - menuHalfSize.y);

            contextMenu.anchoredPosition = localPosition;
            currentTargetSlotIndex = targetSlotIndex;
            contextMenu.gameObject.SetActive(true);
            contextMenu.SetAsLastSibling();
        }

        public bool BeginItemDrag(Sprite icon, Vector2 screenPosition)
        {
            if (!IsVisible || IsDiscardConfirmationVisible || dragIcon == null || icon == null)
            {
                return false;
            }

            HideContextMenu();
            dragIcon.sprite = icon;
            dragIcon.raycastTarget = false;
            dragIcon.gameObject.SetActive(true);
            dragIcon.transform.SetAsLastSibling();
            UpdateItemDrag(screenPosition);
            return true;
        }

        public void UpdateItemDrag(Vector2 screenPosition)
        {
            if (dragIcon == null || !dragIcon.gameObject.activeSelf || canvas == null)
            {
                return;
            }

            RectTransform dragRect = dragIcon.rectTransform;
            RectTransform parentRect = dragRect.parent as RectTransform;
            if (parentRect == null)
            {
                return;
            }

            Camera eventCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : canvas.worldCamera;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    parentRect,
                    screenPosition,
                    eventCamera,
                    out Vector2 localPosition))
            {
                dragRect.localPosition = localPosition;
            }
        }

        public void EndItemDrag()
        {
            if (dragIcon == null)
            {
                return;
            }

            dragIcon.sprite = null;
            dragIcon.gameObject.SetActive(false);
        }

        public void HideContextMenu()
        {
            currentTargetSlotIndex = null;

            if (contextMenu != null)
            {
                contextMenu.gameObject.SetActive(false);
            }
        }

        public void ShowDiscardConfirmation()
        {
            if (currentTargetSlotIndex == null || discardConfirmationPanel == null)
            {
                return;
            }

            if (contextMenu != null)
            {
                contextMenu.gameObject.SetActive(false);
            }

            discardConfirmationPanel.SetActive(true);
        }

        public void ConfirmDiscard()
        {
            CloseDiscardConfirmation();
        }

        public void CancelDiscard()
        {
            CloseDiscardConfirmation();
        }

        private void CloseDiscardConfirmation()
        {
            if (discardConfirmationPanel != null)
            {
                discardConfirmationPanel.SetActive(false);
            }

            currentTargetSlotIndex = null;
        }
    }
}
