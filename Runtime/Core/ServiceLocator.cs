using System;
using System.Collections.Generic;
using UnityEngine;

namespace Speedup.Core
{
    /// <summary>
    /// A simple static Service Locator for decoupled systems architecture.
    /// </summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, IGameService> _services = new Dictionary<Type, IGameService>();

        /// <summary>
        /// Registers a service in the locator.
        /// </summary>
        public static void Register<T>(T service) where T : IGameService
        {
            var type = typeof(T);
            if (_services.ContainsKey(type))
            {
                Debug.LogWarning($"[ServiceLocator] Service of type {type.Name} is already registered. Overwriting.");
                _services[type] = service;
            }
            else
            {
                _services.Add(type, service);
            }
        }

        /// <summary>
        /// Unregisters a service from the locator.
        /// </summary>
        public static void Unregister<T>() where T : IGameService
        {
            var type = typeof(T);
            if (_services.ContainsKey(type))
            {
                _services.Remove(type);
            }
        }

        /// <summary>
        /// Retrieves a service. Throws an exception if not found!
        /// </summary>
        public static T Get<T>() where T : IGameService
        {
            var type = typeof(T);
            if (_services.TryGetValue(type, out var service))
            {
                return (T)service;
            }

            throw new Exception($"[ServiceLocator] Requested service of type {type.Name} is not registered.");
        }

        /// <summary>
        /// Tries to get a service without throwing an exception.
        /// </summary>
        public static bool TryGet<T>(out T service) where T : IGameService
        {
            var type = typeof(T);
            if (_services.TryGetValue(type, out var foundService))
            {
                service = (T)foundService;
                return true;
            }

            service = default;
            return false;
        }

        /// <summary>
        /// Clears all services. Useful for application quitting or total state resets.
        /// </summary>
        public static void Clear()
        {
            _services.Clear();
        }
    }
}
