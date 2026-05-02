using System;
using System.Collections.Generic;
using Speedup.Core;

namespace Speedup.Services.Economy
{
    /// <summary>
    /// Service for managing game currencies.
    /// </summary>
    public interface IEconomyService : IGameService
    {
        /// <summary>
        /// Triggered when any currency balance changes.
        /// Passes the currencyId and the new balance.
        /// </summary>
        event Action<string, int> OnCurrencyChanged;

        /// <summary>
        /// Optional setup for known currencies from a catalog (coins/diamonds/energy, etc.).
        /// Balances are initialized to 0 if missing and IDs are validated for SO-based calls.
        /// </summary>
        void ConfigureCurrencies(IReadOnlyList<CurrencyDefinition> currencyDefinitions);

        /// <summary>
        /// Adds a specified amount to the given currency.
        /// </summary>
        void AddCurrency(string currencyId, int amount);
        void AddCurrency(CurrencyDefinition currencyDefinition, int amount);

        /// <summary>
        /// Deducts a specified amount from the given currency.
        /// Returns true if successful, false if there are insufficient funds.
        /// </summary>
        bool DeductCurrency(string currencyId, int amount);
        bool DeductCurrency(CurrencyDefinition currencyDefinition, int amount);

        /// <summary>
        /// Returns the current balance for the given currency.
        /// </summary>
        int GetBalance(string currencyId);
        int GetBalance(CurrencyDefinition currencyDefinition);

        /// <summary>
        /// Checks if the player has at least the specified amount of the given currency.
        /// </summary>
        bool HasEnough(string currencyId, int amount);
        bool HasEnough(CurrencyDefinition currencyDefinition, int amount);

        /// <summary>
        /// Returns a read-only dictionary of all current balances.
        /// </summary>
        IReadOnlyDictionary<string, int> GetAllBalances();
    }
}
