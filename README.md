# Speedup Casual Game Framework (Core Package)

Welcome to the **Speedup** Unity template Core Package (`com.codev.speedup.core`)! This package provides a robust, decoupled foundation designed to aggressively accelerate casual mobile game development. It relies on a rigorous **Service Locator architecture**, the **deVoid UI Framework**, and a structured Event/Signal system to maintain a clean codebase that scales easily.

> [!IMPORTANT]
> **AI AGENT & LLM DIRECTIVES**
> If you are an AI assisting in building a game on top of this package, you **MUST** adhere to the following rules:
> 1. **No Singletons/FindObject**: **Never** use `FindObjectOfType`, `GameObject.Find()`, or standard Singletons. Retrieve global managers via `Speedup.Core.ServiceLocator.Get<T>()`.
> 2. **Decoupled UI**: **Never** tightly couple UI and Gameplay. Use the **Payloads/Properties** system from the UI Framework or a decoupling **Signal System** to pass data.
> 3. **Package Paths**: Remember this is a UPM Package. Core scripts are in `Packages/com.codev.speedup.core/Runtime/`. Do NOT look for them in `Assets/`.
> 4. **Namespaces**: Always include appropriate namespaces: `Speedup.Core`, `Speedup.Services.Audio`, `Speedup.Services.GPGS`, `deVoid.UIFramework`.
> 5. **Extensibility**: New SDK integrations (Ads, IAP, Analytics) should be defined with an interface (e.g., `INewService : IGameService`) and registered in the `GameBootstrapper`.

---

## 🏗️ 1. Service Locator & Modular Architecture

This project abolishes singletons in favor of a static `ServiceLocator`. All core services (Audio, GPGS, Ads, Analytics) sit behind abstracted interfaces (`IGameService`). 

### Retrieving Services
From any script in your game, grab a service effortlessly:
```csharp
using Speedup.Core;
using Speedup.Services.Audio;
using Speedup.Services.GPGS;

public class GameplayManager : MonoBehaviour
{
    private void Start()
    {
        // 🤖 AI Note: ALWAYS retrieve services this way.
        var audioService = ServiceLocator.Get<IAudioService>();
        var gpgsService = ServiceLocator.Get<IPlayService>();

        audioService.PlayMusic(myClip);
    }
}
```

### Adding a New Service
If asked to integrate a new SDK:
1. Create an interface extending `IGameService` (e.g. `IInAppPurchaseService`).
2. Create a concrete class implementing it (e.g. `UnityIAPService`).
3. Open `Packages/com.codev.speedup.core/Runtime/Core/GameBootstrapper.cs` (or the project's extended Bootstrapper) and instantiate + register it:
```csharp
var iapService = new UnityIAPService();
iapService.Initialize();
ServiceLocator.Register<IInAppPurchaseService>(iapService);
```

---

## 🎨 2. UI Framework (deVoid UI)

We utilize Yanko Oliveira's **deVoid UI Framework** to handle complex screen layering, history, and popups efficiently.

### Creating Screens and Panels
1. **Screens** are the main views (Main Menu, Gameplay, Shop). Only one screen is active at a time.
2. **Panels** (Popups/Dialogs) float on top of Screens (Settings Popup, Pause Menu, Error Dialog).

**To create a new UI element:**
1. Create a Canvas Prefab. Attach a class extending `AWindowController` (for screens) or `APanelController` (for panels).
2. Register the prefab in the `UI Settings` Scriptable Object.
3. Show it from code by retrieving the injected UIFrame instance (usually from a UI Manager service or Bootstrapper):
```csharp
uiFrame.OpenWindow("MainMenuScreen");
uiFrame.ShowPanel("SettingsPanel");
```

> [!TIP]
> **Animations**: We have built-in transitions! Instead of manually animating popups, use the `BounceScaleTransition.cs` provided in `Packages/com.codev.speedup.core/Runtime/ThirdParty/deVoidUIFramework/ScreenTransitions/` for perfect algorithmic bounce flow through/overshoot. Create via `Create -> deVoid UI -> Transitions -> Bounce Scale`.

### Passing Data (Payloads / Properties)
To pass data (Payloads) to a UI Screen without coupling, define a generic property container.

```csharp
using deVoid.UIFramework;

// 1. Define the properties
[System.Serializable]
public class LevelCompleteProperties : WindowProperties
{
    public readonly int Score;
    public LevelCompleteProperties(int score) { Score = score; }
}

// 2. Open the Window and pass the payload
uiFrame.OpenWindow("LevelCompleteScreen", new LevelCompleteProperties(500));

// 3. Receive the Payload inside the Screen's Controller
public class LevelCompleteScreen : AWindowController<LevelCompleteProperties>
{
    protected override void OnPropertiesSet()
    {
        int finalScore = Properties.Score;
        scoreText.text = $"Score: {finalScore}";
    }
}
```

---

## 📡 3. Event & Signal System

To avoid hard inter-class dependencies, use a **Signal (Publish/Subscribe)** generic architecture. This template expects gameplay components to communicate via decoupled events.

### Example Standard Setup:
```csharp
using System;

// Define a structured event class
public static class GameSignals
{
    // C# Action approach
    public static Action<int> OnCoinCollected;
    public static Action OnLevelFailed;
}

// Firing a signal
public class Coin : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameSignals.OnCoinCollected?.Invoke(10);
        Destroy(gameObject);
    }
}

// Listening to a signal
public class ScoreManager : MonoBehaviour
{
    private void OnEnable() => GameSignals.OnCoinCollected += HandleCoinCollected;
    private void OnDisable() => GameSignals.OnCoinCollected -= HandleCoinCollected;

    private void HandleCoinCollected(int amount)
    {
        // Update Score logic here!
    }
}
```

> [!NOTE]
> **For Advanced AI users**: If implementing an advanced `SignalBus` class (like Zenject Signals or a custom PubSub), ensure it is instantiated exactly once during `GameBootstrapper` and registered as `ISignalService : IGameService` so scripts can request it globally.

---

## ✨ Included Core Modules
This project ships with wrappers for standard casual game dependencies:
- **GPGS (`IPlayService`)**: Separated internally into `GPGSLeaderboard` and `GPGSCloudSave` for clean score reporting and byte-based conflict resolution cloud saving.
- **Audio (`IAudioService`)**: Central volume routing and BGM/SFX playback.
- **Ads (`IAdService`)**: Interstitial, rewarded, and banner stub integration.
- **Addressables (`IAssetService`)**: Async load handlers.
- **Analytics (`IAnalyticsService`)**: Unified event tracking.

*Note: Ensure you download the respective Unity Packages (Google Mobile Ads, Google Play Games, Addressables) to run these services natively.*
