using System;
using UnityEngine;

namespace InventoryDemo.Data
{
    [Serializable]
    public sealed class PlayerHealth
    {
        [SerializeField, Min(1)] private int maxHealth = 100;
        [SerializeField, Min(0)] private int currentHealth = 50;

        public int MaxHealth => Mathf.Max(1, maxHealth);
        public int CurrentHealth => Mathf.Clamp(currentHealth, 0, MaxHealth);

        public void Restore(int amount)
        {
            currentHealth = Mathf.Clamp(CurrentHealth + Mathf.Max(0, amount), 0, MaxHealth);
        }
    }
}
