using System;
using UnityEngine;
using GoogleMobileAds.Api;
using Speedup.Core;

namespace Speedup.Services.Ads
{
    public class AdMobService : IAdService
    {
        // --- Test Ad Unit IDs ---
#if UNITY_ANDROID
        private string _bannerAdUnitId = "ca-app-pub-3940256099942544/6300978111";
        private string _interstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";
        private string _rewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
        private string _bannerAdUnitId = "ca-app-pub-3940256099942544/2934735716";
        private string _interstitialAdUnitId = "ca-app-pub-3940256099942544/4411468910";
        private string _rewardedAdUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
        private string _bannerAdUnitId = "unused";
        private string _interstitialAdUnitId = "unused";
        private string _rewardedAdUnitId = "unused";
#endif

        private BannerView _bannerView;
        private InterstitialAd _interstitialAd;
        private RewardedAd _rewardedAd;

        public void Initialize()
        {
            MobileAds.Initialize(initStatus =>
            {
                Debug.Log("[AdMobService] SDK Initialized.");
            });
        }

        public void ShowBanner()
        {
            if (_bannerView == null)
            {
                CreateBannerView();
            }
            
            AdRequest request = new AdRequest();
            _bannerView.LoadAd(request);
            _bannerView.Show();
        }

        public void HideBanner()
        {
            if (_bannerView != null)
            {
                _bannerView.Hide();
                _bannerView.Destroy();
                _bannerView = null;
            }
        }

        private void CreateBannerView()
        {
            if (_bannerView != null)
            {
                _bannerView.Destroy();
            }

            // Create a banner at the bottom of the screen.
            _bannerView = new BannerView(_bannerAdUnitId, AdSize.Banner, AdPosition.Bottom);
        }

        public void ShowInterstitial(Action onClosed = null)
        {
            // Clean up the old ad before loading a new one
            if (_interstitialAd != null)
            {
                _interstitialAd.Destroy();
                _interstitialAd = null;
            }

            AdRequest request = new AdRequest();
            InterstitialAd.Load(_interstitialAdUnitId, request,
                (InterstitialAd ad, LoadAdError error) =>
                {
                    // if error is not null, the load request failed
                    if (error != null || ad == null)
                    {
                        Debug.LogError("[AdMobService] Interstitial failed to load: " + error);
                        onClosed?.Invoke(); // Invoke early so the game doesn't hang
                        return;
                    }

                    _interstitialAd = ad;

                    // Register for events
                    _interstitialAd.OnAdFullScreenContentClosed += () =>
                    {
                        onClosed?.Invoke();
                    };

                    _interstitialAd.OnAdFullScreenContentFailed += (AdError adError) =>
                    {
                        Debug.LogError("[AdMobService] Interstitial failed to show: " + adError);
                        onClosed?.Invoke();
                    };

                    _interstitialAd.Show();
                });
        }

        public void ShowRewarded(Action<bool> onComplete)
        {
            // Clean up the old ad before loading a new one
            if (_rewardedAd != null)
            {
                _rewardedAd.Destroy();
                _rewardedAd = null;
            }

            AdRequest request = new AdRequest();
            RewardedAd.Load(_rewardedAdUnitId, request,
                (RewardedAd ad, LoadAdError error) =>
                {
                    if (error != null || ad == null)
                    {
                        Debug.LogError("[AdMobService] Rewarded failed to load: " + error);
                        onComplete?.Invoke(false);
                        return;
                    }

                    _rewardedAd = ad;
                    
                    bool rewardEarned = false;

                    _rewardedAd.OnAdFullScreenContentClosed += () =>
                    {
                        onComplete?.Invoke(rewardEarned);
                    };

                    _rewardedAd.OnAdFullScreenContentFailed += (AdError adError) =>
                    {
                        Debug.LogError("[AdMobService] Rewarded failed to show: " + adError);
                        onComplete?.Invoke(false);
                    };

                    _rewardedAd.Show((Reward reward) =>
                    {
                        // User successfully watched the video
                        rewardEarned = true;
                    });
                });
        }
    }
}
