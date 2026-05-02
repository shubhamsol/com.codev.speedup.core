using UnityEngine;

namespace Speedup.Services.Economy
{
    /// <summary>
    /// Designer-facing currency definition.
    /// Example: Coins, Diamonds, Energy.
    /// </summary>
    [CreateAssetMenu(fileName = "Currency_", menuName = "Speedup/Economy/Currency Definition")]
    public class CurrencyDefinition : ScriptableObject
    {
        [SerializeField] private string currencyId;
        [SerializeField] private string displayName;
        [SerializeField] private Sprite icon;

        public string CurrencyId => currencyId;
        public string DisplayName => displayName;
        public Sprite Icon => icon;
    }
}
