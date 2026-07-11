using UnityEngine;
using UnityEngine.EventSystems;

namespace InventoryDemo.UI
{
    public sealed class InventorySlotView : MonoBehaviour, IPointerClickHandler
    {
        private InventoryWindowView windowView;

        public void Initialize(InventoryWindowView owner)
        {
            windowView = owner;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                windowView.ShowContextMenu(eventData.position);
            }
        }
    }
}
