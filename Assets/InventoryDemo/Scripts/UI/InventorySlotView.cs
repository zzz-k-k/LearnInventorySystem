using UnityEngine;
using UnityEngine.EventSystems;

namespace InventoryDemo.UI
{
    public sealed class InventorySlotView : MonoBehaviour, IPointerClickHandler
    {
        private InventoryWindowView windowView;

        private void Awake()
        {
            windowView = GetComponentInParent<InventoryWindowView>(true);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right && windowView != null)
            {
                windowView.ShowContextMenu(eventData.position);
            }
            if (eventData.button == PointerEventData.InputButton.Left && windowView != null)
            {
                windowView.HideContextMenu();
            }
        }
    }
}
