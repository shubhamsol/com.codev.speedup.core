using System.Collections.Generic;
using UnityEngine;

namespace Speedup.Services.Economy
{
    /// <summary>
    /// Collection of currency definitions used by UI and economy flows.
    /// </summary>
    [CreateAssetMenu(fileName = "CurrencyCatalog", menuName = "Speedup/Economy/Currency Catalog")]
    public class CurrencyCatalog : ScriptableObject
    {
        [SerializeField] private List<CurrencyDefinition> currencies = new List<CurrencyDefinition>();

        public IReadOnlyList<CurrencyDefinition> Currencies => currencies;
    }
}
