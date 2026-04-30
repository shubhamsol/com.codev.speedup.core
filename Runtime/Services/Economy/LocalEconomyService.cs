using System;
using System.Collections.Generic;
using UnityEngine;

namespace Speedup.Services.Economy
{
    /// <summary>
    /// Default implementation of IEconomyService using PlayerPrefs for local persistence.
    /// </summary>
    public class LocalEconomyService : IEconomyService
    {
        private const string PrefsKey = "EconomyService_Balances";
        
        public event Action<string, int> OnCurrencyChanged;

        private Dictionary<string, int> _balances;

        public void Initialize()
        {
            LoadData();
        }

        public void AddCurrency(string currencyId, int amount)
        {
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
            SaveData();
            
            OnCurrencyChanged?.Invoke(currencyId, _balances[currencyId]);
        }

        public bool DeductCurrency(string currencyId, int amount)
        {
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
            SaveData();
            
            OnCurrencyChanged?.Invoke(currencyId, _balances[currencyId]);
            return true;
        }

        public int GetBalance(string currencyId)
        {
            return _balances.TryGetValue(currencyId, out int balance) ? balance : 0;
        }

        public bool HasEnough(string currencyId, int amount)
        {
            return GetBalance(currencyId) >= amount;
        }

        public IReadOnlyDictionary<string, int> GetAllBalances()
        {
            return _balances;
        }

        private void LoadData()
        {
            _balances = new Dictionary<string, int>();
            
            if (PlayerPrefs.HasKey(PrefsKey))
            {
                string json = PlayerPrefs.GetString(PrefsKey);
                try
                {
                    var data = JsonUtility.FromJson<EconomySaveData>(json);
                    if (data != null && data.keys != null && data.values != null)
                    {
                        for (int i = 0; i < data.keys.Count; i++)
                        {
                            if (i < data.values.Count)
                            {
                                _balances[data.keys[i]] = data.values[i];
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"[Economy] Failed to load economy data: {e.Message}");
                }
            }
        }

        private void SaveData()
        {
            var data = new EconomySaveData
            {
                keys = new List<string>(_balances.Keys),
                values = new List<int>(_balances.Values)
            };
            
            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(PrefsKey, json);
            PlayerPrefs.Save();
        }

        [Serializable]
        private class EconomySaveData
        {
            public List<string> keys;
            public List<int> values;
        }
    }
}
