using System.Collections.Generic;
using InventoryDemo.Data;
using InventoryDemo.UI;
using UnityEngine;

namespace InventoryDemo.Controllers
{
    public sealed class InventoryController : MonoBehaviour
    {
        [SerializeField] private ItemDefinition[] itemDefinitions;
        [SerializeField] private InventoryItemData[] inventoryItems;
        [SerializeField] private Transform slotContainer;
        [SerializeField] private InventoryWindowView windowView;
        [SerializeField] private PlayerHealthView playerHealthView;
        [SerializeField] private PlayerHealth playerHealth = new PlayerHealth();

        private InventorySlotView[] slots = System.Array.Empty<InventorySlotView>();

        private void Start()
        {
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

            RefreshPlayerHealth();
            RefreshSlots();
            windowView.HideContextMenu();
        }

        public void RefreshSlots()
        {
            slots = FindSlotsInHierarchyOrder();
            ClearSlots();
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
    }
}
