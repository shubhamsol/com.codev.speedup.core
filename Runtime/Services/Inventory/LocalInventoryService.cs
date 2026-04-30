using System;
using System.Collections.Generic;
using UnityEngine;

namespace Speedup.Services.Inventory
{
    /// <summary>
    /// Default implementation of IInventoryService using PlayerPrefs for local persistence.
    /// </summary>
    public class LocalInventoryService : IInventoryService
    {
        private const string PrefsKey = "InventoryService_Items";
        
        public event Action<string, int> OnItemCountChanged;

        private Dictionary<string, int> _items;

        public void Initialize()
        {
            LoadData();
        }

        public void AddItem(string itemId, int amount = 1)
        {
            if (amount < 0)
            {
                Debug.LogWarning($"[Inventory] Cannot add negative amount to item {itemId}");
                return;
            }

            if (!_items.ContainsKey(itemId))
            {
                _items[itemId] = 0;
            }

            _items[itemId] += amount;
            SaveData();
            
            OnItemCountChanged?.Invoke(itemId, _items[itemId]);
        }

        public bool RemoveItem(string itemId, int amount = 1)
        {
            if (amount < 0)
            {
                Debug.LogWarning($"[Inventory] Cannot remove negative amount from item {itemId}");
                return false;
            }

            if (!HasItem(itemId, amount))
            {
                return false;
            }

            _items[itemId] -= amount;
            
            // Optional: clean up dictionary if item count reaches 0
            if (_items[itemId] == 0)
            {
                _items.Remove(itemId);
                SaveData();
                OnItemCountChanged?.Invoke(itemId, 0);
            }
            else
            {
                SaveData();
                OnItemCountChanged?.Invoke(itemId, _items[itemId]);
            }
            
            return true;
        }

        public int GetItemCount(string itemId)
        {
            return _items.TryGetValue(itemId, out int count) ? count : 0;
        }

        public bool HasItem(string itemId, int amount = 1)
        {
            return GetItemCount(itemId) >= amount;
        }

        public IReadOnlyDictionary<string, int> GetAllItems()
        {
            return _items;
        }

        private void LoadData()
        {
            _items = new Dictionary<string, int>();
            
            if (PlayerPrefs.HasKey(PrefsKey))
            {
                string json = PlayerPrefs.GetString(PrefsKey);
                try
                {
                    var data = JsonUtility.FromJson<InventorySaveData>(json);
                    if (data != null && data.keys != null && data.values != null)
                    {
                        for (int i = 0; i < data.keys.Count; i++)
                        {
                            if (i < data.values.Count)
                            {
                                _items[data.keys[i]] = data.values[i];
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"[Inventory] Failed to load inventory data: {e.Message}");
                }
            }
        }

        private void SaveData()
        {
            var data = new InventorySaveData
            {
                keys = new List<string>(_items.Keys),
                values = new List<int>(_items.Values)
            };
            
            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(PrefsKey, json);
            PlayerPrefs.Save();
        }

        [Serializable]
        private class InventorySaveData
        {
            public List<string> keys;
            public List<int> values;
        }
    }
}
