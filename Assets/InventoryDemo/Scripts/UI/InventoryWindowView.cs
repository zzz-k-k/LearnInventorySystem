using UnityEngine;
using UnityEngine.InputSystem;

namespace InventoryDemo.UI
{
    public sealed class InventoryWindowView : MonoBehaviour
    {
        private GameObject inventoryWindow;
        private RectTransform contextMenu;
        private RectTransform canvasRect;
        private Canvas canvas;

        public bool IsVisible => inventoryWindow != null && inventoryWindow.activeSelf;

        public void Initialize(
            GameObject window,
            RectTransform menu,
            RectTransform rootCanvasRect,
            Canvas rootCanvas)
        {
            inventoryWindow = window;
            contextMenu = menu;
            canvasRect = rootCanvasRect;
            canvas = rootCanvas;

            SetVisible(false);
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.bKey.wasPressedThisFrame)
            {
                ToggleInventory();
            }
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
            if (!IsVisible || contextMenu == null || canvasRect == null)
            {
                return;
            }

            Camera eventCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : canvas.worldCamera;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRect,
                    screenPosition,
                    eventCamera,
                    out Vector2 localPosition))
            {
                return;
            }

            Vector2 menuHalfSize = contextMenu.rect.size * 0.5f;
            Rect canvasBounds = canvasRect.rect;
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
