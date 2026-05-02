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
using Speedup.Services.Rewards;
using Speedup.Services.Save;
using deVoid.UIFramework;

namespace Speedup.Core
{
    /// <summary>
    /// Initializes and registers all core services on application startup.
    /// Should be attached to an empty GameObject in your first boot scene.
    /// </summary>
    public class GameBootstrapper : MonoBehaviour
    {
        private const string DefaultSaveSlot = "player-profile";

        [Header("UI Framework Setup")]
        [Tooltip("Assign the generic UI Settings asset (created via Create -> deVoid UI -> UI Settings)")]
        [SerializeField] private UISettings uiSettings;
        [SerializeField] private CurrencyCatalog currencyCatalog;

        [SerializeField] private List<SoundDatabase> soundDatabases;

        private ISaveService _saveService;

        public void Init()
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

            // 6. Data Streams
            var dataStreamService = new DataStreamService();
            dataStreamService.Initialize();
            ServiceLocator.Register<IDataStreamService>(dataStreamService);

            // 7. Save Orchestrator + Provider Fallback Chain
            var saveService = new SaveService(
                dataStreamService,
                new JsonSaveSerializer(),
                SaveProviderFactory.CreateDefaultFallbackChain());
            saveService.Initialize();
            ServiceLocator.Register<ISaveService>(saveService);
            _saveService = saveService;

            // 6. Economy
            var economyService = new LocalEconomyService(dataStreamService);
            economyService.Initialize();
            if (currencyCatalog != null)
            {
                economyService.ConfigureCurrencies(currencyCatalog.Currencies);
            }
            ServiceLocator.Register<IEconomyService>(economyService);

            // 7. Inventory
            var inventoryService = new LocalInventoryService(dataStreamService);
            inventoryService.Initialize();
            ServiceLocator.Register<IInventoryService>(inventoryService);

            // 8. Rewards
            var rewardService = new RewardService();
            rewardService.Initialize();
            ServiceLocator.Register<IRewardService>(rewardService);

            
            // 2. UI System (deVoid UI)
            var uiManager = new UIManager(uiSettings);
            ServiceLocator.Register<IUIService>(uiManager);
            uiManager.Initialize();

            // Load save data after streams are registered by services.
            _saveService.Load(DefaultSaveSlot, success =>
            {
                if (success)
                {
                    Debug.Log("[Bootstrapper] Save data loaded successfully.");
                }
                else
                {
                    Debug.Log("[Bootstrapper] No save data found in provider chain. Starting fresh.");
                }
            });

            Debug.Log("[Bootstrapper] All services initialized successfully.");
            
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus || _saveService == null)
            {
                return;
            }

            _saveService.Save(DefaultSaveSlot);
        }

        private void OnApplicationQuit()
        {
            _saveService?.Save(DefaultSaveSlot);
        }
    }
}
