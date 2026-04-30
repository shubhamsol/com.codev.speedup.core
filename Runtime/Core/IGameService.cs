namespace Speedup.Core
{
    /// <summary>
    /// Base interface for all services registered in the ServiceLocator.
    /// </summary>
    public interface IGameService
    {
        /// <summary>
        /// Called when the service is registered or initialized.
        /// </summary>
        void Initialize();
    }
}
