using deVoid.UIFramework;
using UnityEngine;

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
        }

        public void OpenScreen(string screenId)
        {
            Frame?.OpenWindow(screenId);
        }

        public void OpenScreen(System.Enum screenId)
        {
            OpenScreen(screenId.ToString());
        }

        public void OpenScreen<T>(string screenId, T properties) where T : WindowProperties
        {
            Frame?.OpenWindow(screenId, properties);
        }

        public void OpenScreen<T>(System.Enum screenId, T properties) where T : WindowProperties
        {
            OpenScreen<T>(screenId.ToString(), properties);
        }

        public void CloseCurrentScreen()
        {
            Frame?.CloseCurrentWindow();
        }

        public void ShowPopup(string panelId)
        {
            Frame?.ShowPanel(panelId);
        }

        public void ShowPopup(System.Enum panelId)
        {
            ShowPopup(panelId.ToString());
        }

        public void ShowPopup<T>(string panelId, T properties) where T : PanelProperties
        {
            Frame?.ShowPanel(panelId, properties);
        }

        public void ShowPopup<T>(System.Enum panelId, T properties) where T : PanelProperties
        {
            ShowPopup<T>(panelId.ToString(), properties);
        }

        public void HidePopup(string panelId)
        {
            Frame?.HidePanel(panelId);
        }

        public void HidePopup(System.Enum panelId)
        {
            HidePopup(panelId.ToString());
        }
    }
}
