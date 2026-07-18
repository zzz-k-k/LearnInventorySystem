using InventoryDemo.Data;
using TMPro;
using UnityEngine;

namespace InventoryDemo.UI
{
    public sealed class PlayerHealthView : MonoBehaviour
    {
        [SerializeField] private TMP_Text healthText;

        public void ShowHealth(PlayerHealth health)
        {
            if (healthText == null || health == null)
            {
                return;
            }

            healthText.text = $"HP: {health.CurrentHealth} / {health.MaxHealth}";
        }
    }
}
