using deVoid.UIFramework;
using Speedup.Core;

namespace Speedup.Services.UI
{
    /// <summary>
    /// Contract for the UI Manager, abstracting the deVoid UI Framework
    /// into the global ServiceLocator architecture.
    /// </summary>
    public interface IUIService : IGameService
    {
        /// <summary>
        /// Direct access to the raw deVoid UIFrame if complex native operations are needed.
        /// </summary>
        UIFrame Frame { get; }

        void OpenScreen(string screenId);
        void OpenScreen<T>(string screenId, T properties) where T : WindowProperties;
        void CloseCurrentScreen();

        void ShowPopup(string panelId);
        void ShowPopup<T>(string panelId, T properties) where T : PanelProperties;
        void HidePopup(string panelId);
    }
}
