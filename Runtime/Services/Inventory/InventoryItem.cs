using UnityEngine;

namespace Speedup.Services.Inventory
{
    /// <summary>
    /// Represents an item that can be stored in the inventory or purchased from a shop.
    /// </summary>
    [CreateAssetMenu(fileName = "New Inventory Item", menuName = "Speedup/Inventory/Inventory Item")]
    public class InventoryItem : ScriptableObject
    {
        [Header("Item Identification")]
        [Tooltip("Unique identifier used by the Inventory Service.")]
        [SerializeField] private string id;

        [Tooltip("Display name shown in UI.")]
        [SerializeField] private string itemName;

        [Tooltip("Visual representation shown in UI.")]
        [SerializeField] private Sprite icon;

        [TextArea(2, 4)]
        [Tooltip("Description of the item.")]
        [SerializeField] private string description;

        [Header("Economy (Optional)")]
        [Tooltip("The currency ID required to purchase this item (e.g., 'Coins', 'Gems').")]
        [SerializeField] private string currencyId = "Coins";

        [Tooltip("The cost to purchase this item.")]
        [SerializeField] private int cost = 0;

        [Tooltip("Can this item be stacked (i.e. hold more than 1 in inventory)?")]
        [SerializeField] private bool isStackable = true;

        // Public accessors
        public string Id => string.IsNullOrEmpty(id) ? name : id;
        public string ItemName => itemName;
        public Sprite Icon => icon;
        public string Description => description;
        public string CurrencyId => currencyId;
        public int Cost => cost;
        public bool IsStackable => isStackable;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(id))
            {
                id = name;
            }
        }
    }
}
