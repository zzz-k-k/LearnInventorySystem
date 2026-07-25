using System.Collections.Generic;
using InventoryDemo.Data;
using InventoryDemo.UI;
using UnityEngine;

namespace InventoryDemo.Controllers
{
    public sealed class InventoryController : MonoBehaviour
    {
        [SerializeField] private ItemDefinition[] itemDefinitions;
        [SerializeField] private List<InventoryItemData> inventoryItems;
        [SerializeField] private Transform slotContainer;
        [SerializeField] private InventoryWindowView windowView;
        [SerializeField] private PlayerHealthView playerHealthView;
        [SerializeField] private PlayerHealth playerHealth = new PlayerHealth();

        private InventorySlotView[] slots = System.Array.Empty<InventorySlotView>();

        private void Start()
        {
            if (!TryNormalizeItemStacks())
            {
                Debug.LogError("Inventory data could not be loaded because its stacks do not fit in the available slots.", this);
                RefreshPlayerHealth();
                return;
            }

            RefreshSlots();
            RefreshPlayerHealth();
        }

        public void UseSelectedItem()
        {
            if (windowView == null || windowView.CurrentTargetSlotIndex is not int targetSlotIndex)
            {
                return;
            }

            InventoryItemData item = FindItemAtSlot(targetSlotIndex);
            if (item == null)
            {
                Debug.LogWarning($"No inventory item found at slot index {targetSlotIndex}.", this);
                return;
            }

            Dictionary<string, ItemDefinition> definitionsByCode = BuildDefinitionLookup();
            if (!definitionsByCode.TryGetValue(item.ItemCode, out ItemDefinition definition))
            {
                Debug.LogWarning($"No item definition found for code '{item.ItemCode}'.", this);
                return;
            }

            if (!TryApplyEffect(definition) || !item.ConsumeOne())
            {
                return;
            }

            if (!item.HasItems)
            {
                inventoryItems.Remove(item);
            }

            RefreshPlayerHealth();
            RefreshSlots();
            windowView.HideContextMenu();
        }

        public void RefreshSlots()
        {
            slots = FindSlotsInHierarchyOrder();
            ClearSlots();
            ApplySearch(windowView != null ? windowView.CurrentSearchText : string.Empty);
            Dictionary<string, ItemDefinition> definitionsByCode = BuildDefinitionLookup();

            if (inventoryItems == null)
            {
                return;
            }

            foreach (InventoryItemData item in inventoryItems)
            {
                if (item == null || !item.HasItems)
                {
                    continue;
                }

                if (item.SlotIndex < 0 || item.SlotIndex >= slots.Length)
                {
                    Debug.LogWarning("Skipped inventory item with an invalid slot index.", this);
                    continue;
                }

                if (!definitionsByCode.TryGetValue(item.ItemCode, out ItemDefinition definition))
                {
                    Debug.LogWarning($"No item definition found for code '{item.ItemCode}'.", this);
                    continue;
                }

                InventorySlotView slot = slots[item.SlotIndex];
                if (slot == null)
                {
                    Debug.LogWarning($"No InventorySlotView found at slot index {item.SlotIndex}.", this);
                    continue;
                }

                slot.ShowItem(definition.Icon, item.Quantity);
            }
        }

        public void ApplySearch(string searchText)
        {
            string trimmedText = searchText == null ? string.Empty : searchText.Trim();
            var matchedSlotIndexes = new HashSet<int>();

            if (trimmedText.Length > 0 && inventoryItems != null)
            {
                Dictionary<string, ItemDefinition> definitionsByCode = BuildDefinitionLookup();
                foreach (InventoryItemData item in inventoryItems)
                {
                    if (item == null || !item.HasItems)
                    {
                        continue;
                    }

                    if (definitionsByCode.TryGetValue(item.ItemCode, out ItemDefinition definition) &&
                        definition.DisplayName != null &&
                        definition.DisplayName.Contains(trimmedText, System.StringComparison.OrdinalIgnoreCase))
                    {
                        matchedSlotIndexes.Add(item.SlotIndex);
                    }
                }
            }

            for (int slotIndex = 0; slotIndex < slots.Length; slotIndex++)
            {
                if (slots[slotIndex] != null)
                {
                    slots[slotIndex].SetHighlight(matchedSlotIndexes.Contains(slotIndex));
                }
            }
        }

        public bool TryMoveItem(int sourceSlotIndex, int targetSlotIndex)
        {
            bool targetIsOutsideInventory =
                slotContainer == null ||
                targetSlotIndex < 0 ||
                targetSlotIndex >= slotContainer.childCount;

            if (sourceSlotIndex == targetSlotIndex || targetIsOutsideInventory)
            {
                RefreshSlots();
                return false;
            }

            InventoryItemData sourceItem = FindItemAtSlot(sourceSlotIndex);
            if (sourceItem == null)
            {
                RefreshSlots();
                return false;
            }

            InventoryItemData targetItem = FindItemAtSlot(targetSlotIndex);
            bool moved;

            if (targetItem == null)
            {
                moved = sourceItem.TryMoveToSlot(targetSlotIndex);
            }
            else if (!string.Equals(sourceItem.ItemCode, targetItem.ItemCode, System.StringComparison.Ordinal))
            {
                moved = sourceItem.TrySwapSlotWith(targetItem);
            }
            else
            {
                Dictionary<string, ItemDefinition> definitionsByCode = BuildDefinitionLookup();
                if (!definitionsByCode.TryGetValue(sourceItem.ItemCode, out ItemDefinition definition))
                {
                    Debug.LogWarning($"No item definition found for code '{sourceItem.ItemCode}'.", this);
                    moved = false;
                }
                else if (definition.MaxStackSize > 1 && targetItem.Quantity < definition.MaxStackSize)
                {
                    moved = sourceItem.TransferTo(targetItem, definition.MaxStackSize) > 0;
                    if (moved && !sourceItem.HasItems)
                    {
                        inventoryItems.Remove(sourceItem);
                    }
                }
                else
                {
                    moved = sourceItem.TrySwapSlotWith(targetItem);
                }
            }

            RefreshSlots();

            if (moved && windowView != null)
            {
                windowView.HideContextMenu();
            }

            return moved;
        }

        public bool CanSplitItem(int slotIndex)
        {
            InventoryItemData item = FindItemAtSlot(slotIndex);
            return item != null &&
                   item.Quantity > 1 &&
                   FindFirstEmptySlotIndex() >= 0;
        }

        public void BeginSplitSelectedItem()
        {
            if (windowView == null ||
                windowView.CurrentTargetSlotIndex is not int targetSlotIndex)
            {
                return;
            }

            InventoryItemData item = FindItemAtSlot(targetSlotIndex);
            if (item == null || !CanSplitItem(targetSlotIndex))
            {
                return;
            }

            windowView.ShowSplitPanel(item.Quantity - 1);
        }

        public void ConfirmSplitSelectedItem()
        {
            if (windowView == null ||
                windowView.CurrentTargetSlotIndex is not int sourceSlotIndex)
            {
                return;
            }

            InventoryItemData sourceItem = FindItemAtSlot(sourceSlotIndex);
            int targetSlotIndex = FindFirstEmptySlotIndex();
            int splitQuantity = windowView.SelectedSplitQuantity;

            if (sourceItem == null ||
                targetSlotIndex < 0 ||
                !sourceItem.TrySplit(splitQuantity, targetSlotIndex, out InventoryItemData splitItem))
            {
                return;
            }

            inventoryItems.Add(splitItem);
            RefreshSlots();
            windowView.CloseSplitPanel();
        }

        public void ConfirmDiscardSelectedItem()
        {
            if (windowView == null ||
                !windowView.IsDiscardConfirmationVisible ||
                windowView.CurrentTargetSlotIndex is not int targetSlotIndex)
            {
                return;
            }

            InventoryItemData item = FindItemAtSlot(targetSlotIndex);
            if (item != null)
            {
                inventoryItems.Remove(item);
                RefreshSlots();
            }

            windowView.CloseDiscardConfirmation();
        }

        private void ClearSlots()
        {
            foreach (InventorySlotView slot in slots)
            {
                if (slot != null)
                {
                    slot.ClearItem();
                }
            }
        }

        private InventorySlotView[] FindSlotsInHierarchyOrder()
        {
            if (slotContainer == null)
            {
                Debug.LogWarning("Inventory slot container is not assigned.", this);
                return System.Array.Empty<InventorySlotView>();
            }

            var result = new InventorySlotView[slotContainer.childCount];
            for (int index = 0; index < slotContainer.childCount; index++)
            {
                InventorySlotView slot = slotContainer.GetChild(index).GetComponent<InventorySlotView>();
                result[index] = slot;
                if (slot != null)
                {
                    slot.SetSlotIndex(index);
                }
            }

            return result;
        }

        private InventoryItemData FindItemAtSlot(int slotIndex)
        {
            if (inventoryItems == null)
            {
                return null;
            }

            foreach (InventoryItemData item in inventoryItems)
            {
                if (item != null && item.HasItems && item.SlotIndex == slotIndex)
                {
                    return item;
                }
            }

            return null;
        }

        private int FindFirstEmptySlotIndex()
        {
            if (slotContainer == null)
            {
                return -1;
            }

            for (int slotIndex = 0; slotIndex < slotContainer.childCount; slotIndex++)
            {
                if (FindItemAtSlot(slotIndex) == null)
                {
                    return slotIndex;
                }
            }

            return -1;
        }

        private bool TryApplyEffect(ItemDefinition definition)
        {
            if (playerHealth == null || definition == null)
            {
                return false;
            }

            switch (definition.EffectType)
            {
                case ItemEffectType.RestoreHealth:
                    playerHealth.Restore(definition.EffectValue);
                    return true;
                default:
                    Debug.LogWarning($"Item '{definition.Code}' has no supported use effect.", this);
                    return false;
            }
        }

        private void RefreshPlayerHealth()
        {
            if (playerHealthView != null)
            {
                playerHealthView.ShowHealth(playerHealth);
            }
        }

        private bool TryNormalizeItemStacks()
        {
            if (inventoryItems == null || inventoryItems.Count == 0)
            {
                return true;
            }

            if (slotContainer == null)
            {
                Debug.LogError("Inventory stacks cannot be normalized without a slot container.", this);
                return false;
            }

            int slotCount = slotContainer.childCount;
            var occupiedSlots = new bool[slotCount];
            var orderedItems = new List<InventoryItemData>();
            var itemsToRemove = new List<InventoryItemData>();

            foreach (InventoryItemData item in inventoryItems)
            {
                if (item == null || !item.HasItems)
                {
                    itemsToRemove.Add(item);
                    continue;
                }

                if (item.SlotIndex < 0 || item.SlotIndex >= slotCount)
                {
                    Debug.LogError($"Inventory item '{item.ItemCode}' has invalid slot index {item.SlotIndex}.", this);
                    return false;
                }

                if (occupiedSlots[item.SlotIndex])
                {
                    Debug.LogError($"More than one inventory item occupies slot index {item.SlotIndex}.", this);
                    return false;
                }

                occupiedSlots[item.SlotIndex] = true;
                orderedItems.Add(item);
            }

            orderedItems.Sort((left, right) => left.SlotIndex.CompareTo(right.SlotIndex));
            Dictionary<string, ItemDefinition> definitionsByCode = BuildDefinitionLookup();
            var splitPlans = new List<StackSplitPlan>();

            foreach (InventoryItemData item in orderedItems)
            {
                if (!definitionsByCode.TryGetValue(item.ItemCode, out ItemDefinition definition))
                {
                    Debug.LogError($"Inventory item definition '{item.ItemCode}' could not be found.", this);
                    return false;
                }

                int maxStackSize = definition.MaxStackSize;
                if (item.Quantity <= maxStackSize)
                {
                    continue;
                }

                int remainingQuantity = item.Quantity - maxStackSize;
                var targetSlotIndices = new List<int>();

                while (remainingQuantity > 0)
                {
                    int targetSlotIndex = FindFirstAvailableSlot(occupiedSlots);
                    if (targetSlotIndex < 0)
                    {
                        Debug.LogError($"Inventory item '{item.ItemCode}' cannot be split because the inventory is full.", this);
                        return false;
                    }

                    occupiedSlots[targetSlotIndex] = true;
                    targetSlotIndices.Add(targetSlotIndex);
                    remainingQuantity -= System.Math.Min(remainingQuantity, maxStackSize);
                }

                splitPlans.Add(new StackSplitPlan(item, maxStackSize, targetSlotIndices));
            }

            foreach (StackSplitPlan splitPlan in splitPlans)
            {
                if (!splitPlan.SourceItem.TrySplitOverflow(
                        splitPlan.MaxStackSize,
                        splitPlan.TargetSlotIndices,
                        out List<InventoryItemData> splitItems))
                {
                    Debug.LogError($"Inventory item '{splitPlan.SourceItem.ItemCode}' could not be split as planned.", this);
                    return false;
                }

                inventoryItems.AddRange(splitItems);
            }

            foreach (InventoryItemData item in itemsToRemove)
            {
                inventoryItems.Remove(item);
            }

            return true;
        }

        private static int FindFirstAvailableSlot(bool[] occupiedSlots)
        {
            for (int slotIndex = 0; slotIndex < occupiedSlots.Length; slotIndex++)
            {
                if (!occupiedSlots[slotIndex])
                {
                    return slotIndex;
                }
            }

            return -1;
        }

        private Dictionary<string, ItemDefinition> BuildDefinitionLookup()
        {
            var definitionsByCode = new Dictionary<string, ItemDefinition>();
            if (itemDefinitions == null)
            {
                return definitionsByCode;
            }

            foreach (ItemDefinition definition in itemDefinitions)
            {
                if (definition == null || string.IsNullOrWhiteSpace(definition.Code))
                {
                    continue;
                }

                if (!definitionsByCode.TryAdd(definition.Code, definition))
                {
                    Debug.LogWarning($"Duplicate item definition code '{definition.Code}'.", this);
                }
            }

            return definitionsByCode;
        }

        private sealed class StackSplitPlan
        {
            public StackSplitPlan(
                InventoryItemData sourceItem,
                int maxStackSize,
                List<int> targetSlotIndices)
            {
                SourceItem = sourceItem;
                MaxStackSize = maxStackSize;
                TargetSlotIndices = targetSlotIndices;
            }

            public InventoryItemData SourceItem { get; }
            public int MaxStackSize { get; }
            public List<int> TargetSlotIndices { get; }
        }
    }
}
