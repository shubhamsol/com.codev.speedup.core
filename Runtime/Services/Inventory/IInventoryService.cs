using System;
using System.Collections.Generic;
using Speedup.Core;

namespace Speedup.Services.Inventory
{
    /// <summary>
    /// Service for managing player inventory and items.
    /// </summary>
    public interface IInventoryService : IGameService
    {
        /// <summary>
        /// Triggered when the count of any item changes.
        /// Passes the itemId and the new count.
        /// </summary>
        event Action<string, int> OnItemCountChanged;

        /// <summary>
        /// Adds a specified amount of the given item.
        /// </summary>
        void AddItem(string itemId, int amount = 1);

        /// <summary>
        /// Removes a specified amount of the given item.
        /// Returns true if successful, false if the player doesn't have enough.
        /// </summary>
        bool RemoveItem(string itemId, int amount = 1);

        /// <summary>
        /// Returns the current count of the given item.
        /// </summary>
        int GetItemCount(string itemId);

        /// <summary>
        /// Checks if the player has at least the specified amount of the given item.
        /// </summary>
        bool HasItem(string itemId, int amount = 1);

        /// <summary>
        /// Returns a read-only dictionary of all items and their counts.
        /// </summary>
        IReadOnlyDictionary<string, int> GetAllItems();
    }
}
