using System;
using System.Collections.Generic;
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

        public static bool TryCreate(
            string itemCode,
            int slotIndex,
            int quantity,
            out InventoryItemData item)
        {
            item = null;
            if (string.IsNullOrWhiteSpace(itemCode) || slotIndex < 0 || quantity <= 0)
            {
                return false;
            }

            item = new InventoryItemData
            {
                itemCode = itemCode,
                slotIndex = slotIndex,
                quantity = quantity
            };
            return true;
        }

        public bool ConsumeOne()
        {
            if (!HasItems)
            {
                return false;
            }

            quantity--;
            return true;
        }

        public bool TryMoveToSlot(int targetSlotIndex)
        {
            if (targetSlotIndex < 0 || targetSlotIndex == slotIndex)
            {
                return false;
            }

            slotIndex = targetSlotIndex;
            return true;
        }

        public bool TrySwapSlotWith(InventoryItemData other)
        {
            if (other == null || ReferenceEquals(this, other) || slotIndex == other.slotIndex)
            {
                return false;
            }

            int previousSlotIndex = slotIndex;
            slotIndex = other.slotIndex;
            other.slotIndex = previousSlotIndex;
            return true;
        }

        public int TransferTo(InventoryItemData target, int targetMaxQuantity)
        {
            if (target == null ||
                ReferenceEquals(this, target) ||
                !HasItems ||
                !string.Equals(itemCode, target.itemCode, StringComparison.Ordinal))
            {
                return 0;
            }

            int availableCapacity = targetMaxQuantity - target.quantity;
            if (availableCapacity <= 0)
            {
                return 0;
            }

            int transferredQuantity = Math.Min(quantity, availableCapacity);
            quantity -= transferredQuantity;
            target.quantity += transferredQuantity;
            return transferredQuantity;
        }

        public bool TrySplit(
            int splitQuantity,
            int targetSlotIndex,
            out InventoryItemData splitItem)
        {
            splitItem = null;
            if (splitQuantity <= 0 ||
                splitQuantity >= quantity ||
                targetSlotIndex < 0 ||
                targetSlotIndex == slotIndex)
            {
                return false;
            }

            splitItem = new InventoryItemData
            {
                itemCode = itemCode,
                slotIndex = targetSlotIndex,
                quantity = splitQuantity
            };
            quantity -= splitQuantity;
            return true;
        }

        public bool TrySplitOverflow(
            int maxStackSize,
            IReadOnlyList<int> targetSlotIndices,
            out List<InventoryItemData> splitItems)
        {
            splitItems = null;
            if (maxStackSize < 1 || quantity <= maxStackSize || targetSlotIndices == null)
            {
                return false;
            }

            int remainingQuantity = quantity - maxStackSize;
            int requiredStackCount = 0;
            for (int amount = remainingQuantity; amount > 0; amount -= Math.Min(amount, maxStackSize))
            {
                requiredStackCount++;
            }

            if (targetSlotIndices.Count != requiredStackCount)
            {
                return false;
            }

            var usedSlotIndices = new HashSet<int>();
            var createdItems = new List<InventoryItemData>(requiredStackCount);

            foreach (int targetSlotIndex in targetSlotIndices)
            {
                if (targetSlotIndex < 0 ||
                    targetSlotIndex == slotIndex ||
                    !usedSlotIndices.Add(targetSlotIndex))
                {
                    return false;
                }

                int splitQuantity = Math.Min(remainingQuantity, maxStackSize);
                createdItems.Add(new InventoryItemData
                {
                    itemCode = itemCode,
                    slotIndex = targetSlotIndex,
                    quantity = splitQuantity
                });
                remainingQuantity -= splitQuantity;
            }

            if (remainingQuantity != 0)
            {
                return false;
            }

            quantity = maxStackSize;
            splitItems = createdItems;
            return true;
        }
    }
}
