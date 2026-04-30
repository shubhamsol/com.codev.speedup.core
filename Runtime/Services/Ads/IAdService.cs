using System;
using Speedup.Core;

namespace Speedup.Services.Ads
{
    public interface IAdService : IGameService
    {
        void ShowBanner();
        void HideBanner();
        void ShowInterstitial(Action onClosed = null);
        void ShowRewarded(Action<bool> onComplete);
    }
}
