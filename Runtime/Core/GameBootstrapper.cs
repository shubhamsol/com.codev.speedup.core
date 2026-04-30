using System.Collections.Generic;
using UnityEngine;
using Speedup.Services.Audio;
using Speedup.Services.Analytics;
using Speedup.Services.Ads;
using Speedup.Services.GPGS;
using Speedup.Services.Addressables;
using Speedup.Services.UI;
using Speedup.Services.Economy;
using Speedup.Services.Inventory;
using deVoid.UIFramework;

namespace Speedup.Core
{
    /// <summary>
    /// Initializes and registers all core services on application startup.
    /// Should be attached to an empty GameObject in your first boot scene.
    /// </summary>
    public class GameBootstrapper : MonoBehaviour
    {
        [Header("UI Framework Setup")]
        [Tooltip("Assign the generic UI Settings asset (created via Create -> deVoid UI -> UI Settings)")]
        [SerializeField] private UISettings uiSettings;

        [SerializeField] private List<SoundDatabase> soundDatabases;
        private void Awake()
        {
            // Ensure this bootstrapper persists if needed, or simply let it run once and die.
            // In many architectures, you might have a single persistent "App" object.
            DontDestroyOnLoad(gameObject);

            InitializeServices();
        }

        private void InitializeServices()
        {
            Debug.Log("[Bootstrapper] Initializing Services...");

            // 1. Addressables
            var addressablesManager = new AddressableService();
            addressablesManager.Initialize();
            ServiceLocator.Register<IAssetService>(addressablesManager);

            // 2. UI System (deVoid UI)
            var uiManager = new UIManager(uiSettings);
            ServiceLocator.Register<IUIService>(uiManager);
            uiManager.Initialize();

            // 3. Audio
            var audioManager = gameObject.AddComponent<AudioManager>();
            audioManager.Initialize(soundDatabases);
            ServiceLocator.Register<IAudioService>(audioManager);

            // 3. Analytics
            var analyticsManager = new UnityAnalyticsService();
            analyticsManager.Initialize();
            ServiceLocator.Register<IAnalyticsService>(analyticsManager);

            // 4. Ads
            var adManager = new AdMobService();
            adManager.Initialize();
            ServiceLocator.Register<IAdService>(adManager);

            // 5. GPGS Cloud / Leaderboards
            var playManager = new PlayGamesService();
            playManager.Initialize();
            ServiceLocator.Register<IPlayService>(playManager);

            // 6. Economy
            var economyService = new LocalEconomyService();
            economyService.Initialize();
            ServiceLocator.Register<IEconomyService>(economyService);

            // 7. Inventory
            var inventoryService = new LocalInventoryService();
            inventoryService.Initialize();
            ServiceLocator.Register<IInventoryService>(inventoryService);

            Debug.Log("[Bootstrapper] All services initialized successfully.");
            
            // Example: Open the Home screen immediately upon system boot!
            // uiManager.OpenScreen("HomeScreen");
        }
    }
}
