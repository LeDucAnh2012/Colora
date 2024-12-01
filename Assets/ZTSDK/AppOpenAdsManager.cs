using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

public class AppOpenAdsManager : MonoBehaviour
{
    public static AppOpenAdsManager instance;

#if UNITY_ANDROID
    [SerializeField] private string _adUnitId = "ca-app-pub-7702289911487047/8371133042";
#elif UNITY_IOS
    [SerializeField] private string _adUnitId = "ca-app-pub-3940256099942544/5575463023";
#else
     [SerializeField] private string _adUnitId = "unused";
#endif

    private readonly TimeSpan APPOPEN_TIMEOUT = TimeSpan.FromHours(4);
    private DateTime appOpenExpireTime;
    private AppOpenAd appOpenAd;
    private int retryAttemp_AOA = 0;
    private UnityAction callback = null;
    private void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            LoadAppOpenAd();
        });
    }

  
    #region AOA

    public bool IsAdAvailable
    {
        get
        {
            return appOpenAd != null
                  && appOpenAd.CanShowAd()
                  && DateTime.Now < appOpenExpireTime;
        }
    }
    /// <summary>
    /// Loads the app open ad.
    /// </summary>
    public void LoadAppOpenAd()
    {
        // Clean up the old ad before loading a new one.
        if (appOpenAd != null)
        {
            appOpenAd.Destroy();
            appOpenAd = null;
        }

        // Create our request used to load the ad.
        var adRequest = new AdRequest();

        if (VariableSystem.IsUseIdTest)
        {
            if (ActionHelper.IsAndroid())
                _adUnitId = "ca-app-pub-3940256099942544/9257395921";
            if (ActionHelper.IsIOS())
                _adUnitId = "ca-app-pub-3940256099942544/5575463023";
        }
        // send the request to load the ad.
        AppOpenAd.Load(_adUnitId, adRequest,
            (AppOpenAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Invoke(nameof(LoadAppOpenAd), ActionHelper.GetTimeDelayReloadAds(ref retryAttemp_AOA));
                    return;
                }

                retryAttemp_AOA = 0;
                appOpenExpireTime = DateTime.Now + APPOPEN_TIMEOUT;
                appOpenAd = ad;

                RegisterEventHandlers(ad);
            });
    }


    /// <summary>
    /// Shows the app open ad.
    /// </summary>
    public void ShowAppOpenAd(UnityAction callback)
    {
        StartCoroutine(IE_WaitShowAOA(callback));
    }
    public IEnumerator IE_WaitShowAOA(UnityAction callback)
    {
        yield return new WaitForEndOfFrame();
        if (!CC_Interface.instance.IsShowingAd)
        {
            this.callback = callback;
            CC_Interface.instance.IsShowingAd = true;
            ActionHelper.LogEvent(KeyLogFirebase.ShowAOASuccess);
            appOpenAd?.Show();
        }
    }

    private void RegisterEventHandlers(AppOpenAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("App open ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
            ActionHelper.TrackRevenue_Event(TypeAds.Aoa, adValue);
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("App open ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("App open ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("App open ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            CC_Interface.instance.IsShowingAd = false;
            callback?.Invoke();
            LoadAppOpenAd();
            Debug.Log("App open ad full screen content closed.");
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            CC_Interface.instance.IsShowingAd = false;
            callback?.Invoke();
            LoadAppOpenAd();
            Debug.LogError("App open ad failed to open full screen content " +
                           "with error : " + error);
        };
    }
    #endregion

  
}
