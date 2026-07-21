using System;
using UnityEngine;

namespace InventoryDemo.Data
{
    public enum ItemEffectType
    {
        None,
        RestoreHealth
    }

    [Serializable]
    public sealed class ItemDefinition
    {
        [SerializeField] private string code;
        [SerializeField] private string displayName;
        [SerializeField] private Sprite icon;
        [SerializeField] private ItemEffectType effectType;
        [SerializeField, Min(0)] private int effectValue;
        [SerializeField, Min(1)] private int maxStackSize = 1;

        public string Code => code;
        public string DisplayName => displayName;
        public Sprite Icon => icon;
        public ItemEffectType EffectType => effectType;
        public int EffectValue => effectValue;
        public int MaxStackSize => Mathf.Max(1, maxStackSize);
    }
}
