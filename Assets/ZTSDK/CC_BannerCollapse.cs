using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CC_BannerCollapse : MonoBehaviour
{
    public static CC_BannerCollapse instance;
    private BannerView _bannerView;
    private UnityAction callBackLoad = null;
    private string _adUnitId = "ca-app-pub-7702289911487047/6950240162";

    public bool isLoaded = false;
    public bool isShowLoading = false;

    private void Awake()
    {
        instance = this;

    }
    private void Start()
    {
        // LoadAd();
    }
    public void CreateBannerView()
    {
        Debug.Log("Creating banner view.");
        if (_bannerView != null)
        {
            DestroyAd();
        }

        if (VariableSystem.IsUseIdTest)
        {
            if (ActionHelper.IsAndroid())
                _adUnitId = "ca-app-pub-3940256099942544/2014213617";
            if (ActionHelper.IsIOS())
                _adUnitId = "ca-app-pub-3940256099942544/8388050270";
        }

        _bannerView = new BannerView(_adUnitId, AdSize.Banner, AdPosition.Bottom);
        ListenToAdEvents();
        Debug.Log("Banner view created.");
    }
    public void LoadAd(UnityAction callBack)
    {
        callBackLoad = callBack;
        if (VariableSystem.RemoveAds || VariableSystem.RemoveAdsHack)
        {
            callBackLoad?.Invoke();
            callBackLoad = null;
            return;
        }
        if (RemoteConfig.instance.allConfigData == null)
        {
            callBackLoad?.Invoke();
            callBackLoad = null;
            return;
        }

        if (isShowLoading)
            CanvasAllScene.instance.panelLoading.LoadingProgressFake();
        if (ActionHelper.IsEditor())
        {
            callBackLoad?.Invoke();
            callBackLoad = null;
            return;
        }
        if (_bannerView == null)
            CreateBannerView();

        var adRequest = new AdRequest();
        adRequest.Extras.Add("collapsible", "bottom");
        // adRequest.Extras.Add("collapsible_request_id", Guid.NewGuid().ToString());
        Debug.Log("Loading banner ad.");
        _bannerView.LoadAd(adRequest);

    }
    private void ShowAd(bool isShow)
    {
        ShowAd();
    }
    public void ShowAd()
    {
        Debug.Log("ShowAd");
        if (!isLoaded)
        {
            callBackLoad?.Invoke();
            callBackLoad = null;
            Debug.Log("isLoaded = false");
            //CC_Ads.instance.LoadBanner();
            return;
        }

        if (_bannerView != null)
        {
            if (CC_Interface.instance.IsShowingAd)
            {
                callBackLoad?.Invoke();
                callBackLoad = null;
                Debug.Log("call back inter");
                CC_Ads.instance.callbackInter = ShowAd;
            }
            else
            {
                Debug.Log("Showing banner view.");
                callBackLoad?.Invoke();
                callBackLoad = null;
                _bannerView.Show();
            }
        }
        else
        {
            callBackLoad?.Invoke();
            callBackLoad = null;
        }
    }
    public void HideAd()
    {
        if (_bannerView != null)
        {
            Debug.Log("Hiding banner view.");
            _bannerView.Hide();
        }
    }

    public void DestroyAd()
    {
        if (_bannerView != null)
        {
            Debug.Log("Destroying banner view.");
            _bannerView.Destroy();
            _bannerView = null;

            callBackLoad?.Invoke();
            callBackLoad = null;
        }
    }
    private void ListenToAdEvents()
    {
        // Raised when an ad is loaded into the banner view.
        _bannerView.OnBannerAdLoaded += () =>
        {
            CanvasAllScene.instance.panelLoading.Hide();
            Debug.Log("Banner view loaded an ad with response : " + _bannerView.GetResponseInfo());
            isLoaded = true;

            ShowAd();
        };
        // Raised when an ad fails to load into the banner view.
        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            CanvasAllScene.instance.panelLoading.Hide();
            callBackLoad?.Invoke();
            callBackLoad = null;
            Debug.Log("Banner view failed to load an ad with error : " + error);
            isLoaded = false;
        };
        // Raised when the ad is estimated to have earned money.
        _bannerView.OnAdPaid += (AdValue adValue) =>
        {
            ActionHelper.TrackRevenue_Event(TypeAds.BannerCollapse, adValue);
            Debug.Log(String.Format("Banner view paid {0} {1}.", adValue.Value, adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        _bannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Banner view recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        _bannerView.OnAdClicked += () =>
        {
            Debug.Log("Banner view was clicked.");
        };
        // Raised when an ad opened full screen content.
        _bannerView.OnAdFullScreenContentOpened += () =>
        {
            CanvasAllScene.instance.panelLoading.Hide();
            Debug.Log("Banner view full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        _bannerView.OnAdFullScreenContentClosed += () =>
        {
            CanvasAllScene.instance.panelLoading.Hide();
            Debug.Log("Banner view full screen content closed.");
            isLoaded = false;
            //if (CC_Ads.instance != null)
            //    CC_Ads.instance.LoadBannerNormal();
        };
    }
}
