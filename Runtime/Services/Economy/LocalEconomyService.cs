using System;
using System.Collections.Generic;
using UnityEngine;
using Speedup.Services.Save;

namespace Speedup.Services.Economy
{
    /// <summary>
    /// Default implementation of IEconomyService backed by a data stream.
    /// </summary>
    public class LocalEconomyService : IEconomyService
    {
        private const string StreamKey = "economy.balances";
        
        public event Action<string, int> OnCurrencyChanged;

        private Dictionary<string, int> _balances;
        private readonly HashSet<string> _knownCurrencyIds = new HashSet<string>();
        private readonly IDataStreamService _dataStreamService;
        private IDataStream<EconomySaveData> _stream;

        public LocalEconomyService() : this(null)
        {
        }

        public LocalEconomyService(IDataStreamService dataStreamService)
        {
            _dataStreamService = dataStreamService;
        }

        public void Initialize()
        {
            EnsureStream();
            LoadFromStreamData();
            EnsureKnownCurrenciesExist();
        }

        public void ConfigureCurrencies(IReadOnlyList<CurrencyDefinition> currencyDefinitions)
        {
            _knownCurrencyIds.Clear();

            if (currencyDefinitions == null)
            {
                return;
            }

            for (int i = 0; i < currencyDefinitions.Count; i++)
            {
                CurrencyDefinition definition = currencyDefinitions[i];
                if (definition == null || string.IsNullOrWhiteSpace(definition.CurrencyId))
                {
                    continue;
                }

                _knownCurrencyIds.Add(definition.CurrencyId);
            }

            EnsureKnownCurrenciesExist();
            PersistToStream();
        }

        public void AddCurrency(string currencyId, int amount)
        {
            if (!ValidateCurrencyId(currencyId))
            {
                return;
            }

            if (amount < 0)
            {
                Debug.LogWarning($"[Economy] Cannot add negative amount to currency {currencyId}");
                return;
            }

            if (!_balances.ContainsKey(currencyId))
            {
                _balances[currencyId] = 0;
            }

            _balances[currencyId] += amount;
            PersistToStream();
            
            OnCurrencyChanged?.Invoke(currencyId, _balances[currencyId]);
        }

        public void AddCurrency(CurrencyDefinition currencyDefinition, int amount)
        {
            if (!TryResolveCurrencyId(currencyDefinition, out string currencyId))
            {
                return;
            }

            AddCurrency(currencyId, amount);
        }

        public bool DeductCurrency(string currencyId, int amount)
        {
            if (!ValidateCurrencyId(currencyId))
            {
                return false;
            }

            if (amount < 0)
            {
                Debug.LogWarning($"[Economy] Cannot deduct negative amount from currency {currencyId}");
                return false;
            }

            if (!HasEnough(currencyId, amount))
            {
                return false;
            }

            _balances[currencyId] -= amount;
            PersistToStream();
            
            OnCurrencyChanged?.Invoke(currencyId, _balances[currencyId]);
            return true;
        }

        public bool DeductCurrency(CurrencyDefinition currencyDefinition, int amount)
        {
            if (!TryResolveCurrencyId(currencyDefinition, out string currencyId))
            {
                return false;
            }

            return DeductCurrency(currencyId, amount);
        }

        public int GetBalance(string currencyId)
        {
            if (!ValidateCurrencyId(currencyId))
            {
                return 0;
            }

            return _balances.TryGetValue(currencyId, out int balance) ? balance : 0;
        }

        public int GetBalance(CurrencyDefinition currencyDefinition)
        {
            if (!TryResolveCurrencyId(currencyDefinition, out string currencyId))
            {
                return 0;
            }

            return GetBalance(currencyId);
        }

        public bool HasEnough(string currencyId, int amount)
        {
            return GetBalance(currencyId) >= amount;
        }

        public bool HasEnough(CurrencyDefinition currencyDefinition, int amount)
        {
            return GetBalance(currencyDefinition) >= amount;
        }

        public IReadOnlyDictionary<string, int> GetAllBalances()
        {
            return _balances;
        }

        private void EnsureStream()
        {
            if (_dataStreamService == null)
            {
                return;
            }

            if (!_dataStreamService.TryGetStream(StreamKey, out IDataStream rawStream))
            {
                var createdStream = new DataStream<EconomySaveData>(
                    StreamKey,
                    new EconomySaveData
                    {
                        keys = new List<string>(),
                        values = new List<int>()
                    });
                _dataStreamService.RegisterStream(createdStream);
                _stream = createdStream;
            }
            else
            {
                _stream = rawStream as IDataStream<EconomySaveData>;
                if (_stream == null)
                {
                    Debug.LogError($"[Economy] Stream '{StreamKey}' exists with incompatible type.");
                    return;
                }
            }

            _stream.OnAfterLoad -= HandleAfterStreamLoad;
            _stream.OnAfterLoad += HandleAfterStreamLoad;
        }

        private void LoadFromStreamData()
        {
            _balances = new Dictionary<string, int>();

            if (_stream?.Value == null || _stream.Value.keys == null || _stream.Value.values == null)
            {
                return;
            }

            for (int i = 0; i < _stream.Value.keys.Count; i++)
            {
                if (i < _stream.Value.values.Count)
                {
                    _balances[_stream.Value.keys[i]] = _stream.Value.values[i];
                }
            }
        }

        private void PersistToStream()
        {
            if (_stream == null)
            {
                return;
            }

            var data = new EconomySaveData
            {
                keys = new List<string>(_balances.Keys),
                values = new List<int>(_balances.Values)
            };

            _stream.SetValue(data, true);
        }

        private void HandleAfterStreamLoad()
        {
            LoadFromStreamData();
            EnsureKnownCurrenciesExist();
            foreach (var pair in _balances)
            {
                OnCurrencyChanged?.Invoke(pair.Key, pair.Value);
            }
        }

        private void EnsureKnownCurrenciesExist()
        {
            bool changed = false;

            foreach (string currencyId in _knownCurrencyIds)
            {
                if (!_balances.ContainsKey(currencyId))
                {
                    _balances[currencyId] = 0;
                    changed = true;
                }
            }

            if (changed)
            {
                PersistToStream();
            }
        }

        private static bool ValidateCurrencyId(string currencyId)
        {
            if (!string.IsNullOrWhiteSpace(currencyId))
            {
                return true;
            }

            Debug.LogWarning("[Economy] Currency id is null or empty.");
            return false;
        }

        private static bool TryResolveCurrencyId(CurrencyDefinition currencyDefinition, out string currencyId)
        {
            currencyId = null;

            if (currencyDefinition == null)
            {
                Debug.LogWarning("[Economy] CurrencyDefinition is null.");
                return false;
            }

            currencyId = currencyDefinition.CurrencyId;
            if (ValidateCurrencyId(currencyId))
            {
                return true;
            }

            Debug.LogWarning($"[Economy] Invalid CurrencyDefinition: {currencyDefinition.name}");
            return false;
        }

        [Serializable]
        public class EconomySaveData
        {
            public List<string> keys;
            public List<int> values;
        }
    }
}
