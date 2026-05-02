using System;
using System.Collections.Generic;
using UnityEngine;
using Speedup.Services.Save;

namespace Speedup.Services.Inventory
{
    /// <summary>
    /// Default implementation of IInventoryService backed by a data stream.
    /// </summary>
    public class LocalInventoryService : IInventoryService
    {
        private const string StreamKey = "inventory.items";
        
        public event Action<string, int> OnItemCountChanged;

        private Dictionary<string, int> _items;
        private readonly IDataStreamService _dataStreamService;
        private IDataStream<InventorySaveData> _stream;

        public LocalInventoryService() : this(null)
        {
        }

        public LocalInventoryService(IDataStreamService dataStreamService)
        {
            _dataStreamService = dataStreamService;
        }

        public void Initialize()
        {
            EnsureStream();
            LoadFromStreamData();
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
            PersistToStream();
            
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
                PersistToStream();
                OnItemCountChanged?.Invoke(itemId, 0);
            }
            else
            {
                PersistToStream();
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

        private void EnsureStream()
        {
            if (_dataStreamService == null)
            {
                return;
            }

            if (!_dataStreamService.TryGetStream(StreamKey, out IDataStream rawStream))
            {
                var createdStream = new DataStream<InventorySaveData>(
                    StreamKey,
                    new InventorySaveData
                    {
                        keys = new List<string>(),
                        values = new List<int>()
                    });
                _dataStreamService.RegisterStream(createdStream);
                _stream = createdStream;
            }
            else
            {
                _stream = rawStream as IDataStream<InventorySaveData>;
                if (_stream == null)
                {
                    Debug.LogError($"[Inventory] Stream '{StreamKey}' exists with incompatible type.");
                    return;
                }
            }

            _stream.OnAfterLoad -= HandleAfterStreamLoad;
            _stream.OnAfterLoad += HandleAfterStreamLoad;
        }

        private void LoadFromStreamData()
        {
            _items = new Dictionary<string, int>();

            if (_stream?.Value == null || _stream.Value.keys == null || _stream.Value.values == null)
            {
                return;
            }

            for (int i = 0; i < _stream.Value.keys.Count; i++)
            {
                if (i < _stream.Value.values.Count)
                {
                    _items[_stream.Value.keys[i]] = _stream.Value.values[i];
                }
            }
        }

        private void PersistToStream()
        {
            if (_stream == null)
            {
                return;
            }

            var data = new InventorySaveData
            {
                keys = new List<string>(_items.Keys),
                values = new List<int>(_items.Values)
            };

            _stream.SetValue(data, true);
        }

        private void HandleAfterStreamLoad()
        {
            LoadFromStreamData();
            foreach (var pair in _items)
            {
                OnItemCountChanged?.Invoke(pair.Key, pair.Value);
            }
        }

        [Serializable]
        public class InventorySaveData
        {
            public List<string> keys;
            public List<int> values;
        }
    }
}
