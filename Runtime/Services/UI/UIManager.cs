using UnityEngine;
using deVoid.UIFramework;

namespace Speedup.Services.UI
{
    /// <summary>
    /// A robust ServiceLocator wrapper around the deVoid UI Frame.
    /// Handles instantiating the UI dynamically from settings or wrapping an existing one.
    /// </summary>
    public class UIManager : IUIService
    {
        public UIFrame Frame { get; private set; }

        private readonly UISettings _settings;

        /// <summary>
        /// Constructor accepting the ScriptableObject UI settings to auto-instantiate the UI Canvas.
        /// </summary>
        public UIManager(UISettings settings)
        {
            _settings = settings;
        }

        public void Initialize()
        {
            if (_settings != null)
            {
                Frame = _settings.CreateUIInstance();
                Object.DontDestroyOnLoad(Frame.gameObject);
                Debug.Log("[UIManager] Dynamically instantiated UIFrame from UISettings.");
            }
            else
            {
                // Fallback: Try to find an existing UIFrame in the scene
                Frame = Object.FindFirstObjectByType<UIFrame>();
                if (Frame != null)
                {
                    Debug.Log("[UIManager] Found existing UIFrame in the scene.");
                }
                else
                {
                    Debug.LogError("[UIManager] CRITICAL: No UISettings provided and no UIFrame found in scene!");
                }
            }
            OpenScreen("HomeScreen");
        }

        public void OpenScreen(string screenId)
        {
            Frame?.OpenWindow(screenId);
        }

        public void OpenScreen<T>(string screenId, T properties) where T : WindowProperties
        {
            Frame?.OpenWindow(screenId, properties);
        }

        public void CloseCurrentScreen()
        {
            Frame?.CloseCurrentWindow();
        }

        public void ShowPopup(string panelId)
        {
            Frame?.ShowPanel(panelId);
        }

        public void ShowPopup<T>(string panelId, T properties) where T : PanelProperties
        {
            Frame?.ShowPanel(panelId, properties);
        }

        public void HidePopup(string panelId)
        {
            Frame?.HidePanel(panelId);
        }
    }
}
