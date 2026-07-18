using System;
using UnityEngine;

namespace InventoryDemo.Data
{
    [Serializable]
    public sealed class InventoryItemData
    {
        [SerializeField] private string itemCode;
        [SerializeField, Min(0)] private int slotIndex;
        [SerializeField, Min(1)] private int quantity = 1;

        public string ItemCode => itemCode;
        public int SlotIndex => slotIndex;
        public int Quantity => quantity;
        public bool HasItems => quantity > 0;

        public bool ConsumeOne()
        {
            if (!HasItems)
            {
                return false;
            }

            quantity--;
            return true;
        }
    }
}
