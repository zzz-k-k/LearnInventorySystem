using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

namespace InventoryDemo.UI
{
    public sealed class InventoryWindowView : MonoBehaviour
    {
        [SerializeField] private GameObject inventoryWindow;
        [SerializeField] private RectTransform contextMenu;
        [SerializeField] private TMP_InputField searchInputField;

        private RectTransform canvasRect;
        private Canvas canvas;

        public bool IsVisible => inventoryWindow != null && inventoryWindow.activeSelf;

        public void Configure(GameObject window, RectTransform menu)
        {
            inventoryWindow = window;
            contextMenu = menu;
        }

        private void Awake()
        {
            canvas = GetComponent<Canvas>();
            canvasRect = transform as RectTransform;

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

            inventoryWindow.SetActive(visible);
            if (!visible)
            {
                HideContextMenu();
            }
        }

        public void ShowContextMenu(Vector2 screenPosition)
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
            contextMenu.gameObject.SetActive(true);
            contextMenu.SetAsLastSibling();
        }

        public void HideContextMenu()
        {
            if (contextMenu != null)
            {
                contextMenu.gameObject.SetActive(false);
            }
        }
    }
}
