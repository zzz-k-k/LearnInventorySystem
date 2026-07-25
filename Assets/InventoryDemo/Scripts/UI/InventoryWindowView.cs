using InventoryDemo.Controllers;
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
        [SerializeField] private Button splitButton;
        [SerializeField] private GameObject splitPanel;
        [SerializeField] private Slider splitSlider;
        [SerializeField] private TMP_Text splitAmountText;

        private RectTransform canvasRect;
        private Canvas canvas;
        private InventoryController controller;
        private int? currentTargetSlotIndex;

        public bool IsVisible => inventoryWindow != null && inventoryWindow.activeSelf;
        public bool IsDiscardConfirmationVisible =>
            discardConfirmationPanel != null && discardConfirmationPanel.activeSelf;
        public bool IsSplitPanelVisible => splitPanel != null && splitPanel.activeSelf;
        public int? CurrentTargetSlotIndex => currentTargetSlotIndex;
        public int SelectedSplitQuantity =>
            splitSlider != null ? Mathf.RoundToInt(splitSlider.value) : 0;
        public string CurrentSearchText =>
            searchInputField != null ? searchInputField.text : string.Empty;

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

            if (splitSlider != null)
            {
                splitSlider.onValueChanged.AddListener(UpdateSplitAmountText);
            }

            controller = GetComponentInParent<InventoryController>(true);
            if (controller == null)
            {
                controller = GetComponentInChildren<InventoryController>(true);
            }

            if (searchInputField != null)
            {
                searchInputField.onValueChanged.AddListener(OnSearchTextChanged);
            }
        }

        private void OnDestroy()
        {
            if (splitSlider != null)
            {
                splitSlider.onValueChanged.RemoveListener(UpdateSplitAmountText);
            }

            if (searchInputField != null)
            {
                searchInputField.onValueChanged.RemoveListener(OnSearchTextChanged);
            }
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
                CloseSplitPanel();
            }
        }

        public void ShowContextMenu(
            Vector2 screenPosition,
            int targetSlotIndex,
            bool canSplit)
        {
            if (!IsVisible ||
                IsDiscardConfirmationVisible ||
                IsSplitPanelVisible ||
                contextMenu == null ||
                canvasRect == null ||
                canvas == null)
            {
                return;
            }

            Camera eventCamera = GetEventCamera();

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
            if (splitButton != null)
            {
                splitButton.interactable = canSplit;
            }

            contextMenu.gameObject.SetActive(true);
            contextMenu.SetAsLastSibling();
        }

        public bool BeginItemDrag(Sprite icon, Vector2 screenPosition)
        {
            if (!IsVisible ||
                IsDiscardConfirmationVisible ||
                IsSplitPanelVisible ||
                dragIcon == null ||
                icon == null)
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

            Camera eventCamera = GetEventCamera();

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
            RefreshSearchInteractable();
        }

        public void ShowDiscardConfirmationForSlot(int targetSlotIndex)
        {
            if (!IsVisible ||
                IsSplitPanelVisible ||
                targetSlotIndex < 0)
            {
                return;
            }

            currentTargetSlotIndex = targetSlotIndex;
            ShowDiscardConfirmation();
        }

        public bool IsScreenPointInsideInventory(Vector2 screenPosition)
        {
            if (!IsVisible ||
                inventoryWindow == null ||
                inventoryWindow.transform is not RectTransform windowRect)
            {
                return false;
            }

            Camera eventCamera = GetEventCamera();

            return RectTransformUtility.RectangleContainsScreenPoint(
                windowRect,
                screenPosition,
                eventCamera);
        }

        public void ShowSplitPanel(int maximumAmount)
        {
            if (currentTargetSlotIndex == null ||
                maximumAmount < 1 ||
                splitPanel == null ||
                splitSlider == null)
            {
                return;
            }

            if (contextMenu != null)
            {
                contextMenu.gameObject.SetActive(false);
            }

            splitSlider.wholeNumbers = true;
            splitSlider.minValue = 1;
            splitSlider.maxValue = maximumAmount;
            splitSlider.SetValueWithoutNotify(1);
            UpdateSplitAmountText(1);
            splitPanel.SetActive(true);
            splitPanel.transform.SetAsLastSibling();
            RefreshSearchInteractable();
        }

        public void CloseSplitPanel()
        {
            if (splitPanel != null)
            {
                splitPanel.SetActive(false);
            }

            currentTargetSlotIndex = null;
            RefreshSearchInteractable();
        }

        public void CancelDiscard()
        {
            CloseDiscardConfirmation();
        }

        public void CloseDiscardConfirmation()
        {
            if (discardConfirmationPanel != null)
            {
                discardConfirmationPanel.SetActive(false);
            }

            currentTargetSlotIndex = null;
            RefreshSearchInteractable();
        }

        private void UpdateSplitAmountText(float value)
        {
            if (splitAmountText != null)
            {
                splitAmountText.text = Mathf.RoundToInt(value).ToString();
            }
        }

        private Camera GetEventCamera()
        {
            return canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? canvas.worldCamera
                : null;
        }

        private void OnSearchTextChanged(string searchText)
        {
            if (controller != null)
            {
                controller.ApplySearch(searchText);
            }
        }

        private void RefreshSearchInteractable()
        {
            if (searchInputField != null)
            {
                searchInputField.interactable =
                    !IsDiscardConfirmationVisible && !IsSplitPanelVisible;
            }
        }
    }
}
