using Google.Play.Review;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CC_Interface : MonoBehaviour
{
    public static CC_Interface instance;

    public bool HasBanner { get; set; }
    public bool IsShowingAd { get; set; }
    public bool IsLaunchAoa = false;
    private void Awake()
    {
        instance = this;
        IsShowingAd = false;
    }
    private void Start()
    {
        AppStateEventNotifier.AppStateChanged += OnAppStateChanged;

#if UNITY_ANDROID
        _coroutine = StartCoroutine(InitReview());
#endif

    }
    #region AOA
    private void OnAppStateChanged(AppState state)
    {
        if (state == AppState.Foreground)
        {
            if (IsLaunchAoa && !ActionHelper.IsEditor())
                AdsConfig.instance.CheckShowAoa(KeyLogFirebase.Colora_AOA_Resume_211224);
        }
    }
    public void ShowAppOpenAd(string idAds,UnityAction callback)
    {
        if (IsShowingAd)
        {
            Debug.Log("AOA return IsShowingAd " + IsShowingAd);
            callback?.Invoke();
            return;
        }
        Debug.Log("AOA return IsAdAvailable " + AppOpenAdsManager.instance.IsAdAvailable);
        if (AppOpenAdsManager.instance.IsAdAvailable)
            AppOpenAdsManager.instance.ShowAppOpenAd(idAds,callback);

    }
    #endregion

    #region Reward
    public void ShowReward(string idAds,string where = "", Action<bool> callback = null)
    {
        if (!TimeManager.instance.IsHasInternet)
        {
            // show popup no internet
            Debug.Log("ko co internet");
            callback?.Invoke(false);
            CanvasAllScene.instance.ShowNoti(I2.Loc.ScriptLocalization.No_Internet);
        }
        else
        {
            // show loading
            StartCoroutine(WaitForRewardVideo(idAds));
        }

        IEnumerator WaitForRewardVideo(string idAds)
        {
            CanvasAllScene.instance.objLoading.Show();

            Debug.Log("loading in 1s");
            yield return new WaitForSecondsRealtime(1f);
            CanvasAllScene.instance.objLoading.Hide();
            CC_Ads.instance.ShowRewardedAd(idAds,where,  callback);
        }
    }
    #endregion

    #region Inter
    public void ShowInter(string idAds, Action<bool> callback = null)
    {
        // callback?.Invoke();
        if (CC_Ads.instance == null)
        {
            Debug.Log("cc_asd null");
            callback?.Invoke(false);
        }
        else
            CC_Ads.instance.ShowInter(idAds,callback);
    }
    #endregion

    #region Banner
    public void ShowBanner()
    {
        CC_Ads.instance.LoadBanner();
    }
    public void DestroyBanner()
    {
        CC_Ads.instance.DestroyAd();
    }
    #endregion

    #region IAP
    private Action<bool> callback = null;
    public void BuyIAP(IAP_Product iAP_Product, Action<bool> callback = null)
    {
        this.callback = callback;
        CC_IAP.instance.BuyIAP(iAP_Product);
    }
    public void RestorePurchase(IAP_Product iAP_Product)
    {
        switch (iAP_Product)
        {
            case IAP_Product.RemoveAds:
                Debug.Log("Restore RemoveAds");
                ActionHelper.LogEvent(KeyLogFirebase.RemoveAdsSuccess);
                VariableSystem.RemoveAds = true;
                DestroyBanner();

                break;
            //case IAP_Product.Princess_Pack:
            //    Debug.Log("Restore Princess_Pack");
            //    ActionHelper.LogEvent(KeyLogFirebase.BuyPackSuccess + iAP_Product);
            //    DataShop.GetListShop()[0].isUnLock = true;
            //    DataShop.UnLock(0);

            //    break;
            //case IAP_Product.Fairy_Pack:
            //    Debug.Log("Restore Fairy_Pack");
            //    ActionHelper.LogEvent(KeyLogFirebase.BuyPackSuccess + iAP_Product);
            //    DataShop.GetListShop()[1].isUnLock = true;
            //    DataShop.UnLock(1);

            //    break;
        }
    }
    public void PurchaseCallback(bool isSuccess)
    {
        
        CanvasAllScene.instance.panelLoading.Hide();
        string str = isSuccess ? I2.Loc.ScriptLocalization.Purchase_success : I2.Loc.ScriptLocalization.Purchase_failed;

        CanvasAllScene.instance.ShowNoti(str);

        callback?.Invoke(isSuccess);
    }
    #endregion

    #region Rate

#if UNITY_ANDROID
    private ReviewManager _reviewManager;
    private PlayReviewInfo _playReviewInfo;
    private Coroutine _coroutine;
#endif

    public void RateAndReview()
    {
#if UNITY_IOS
        Device.RequestStoreReview();
#elif UNITY_ANDROID
        StartCoroutine(LaunchReview());
#endif
    }

#if UNITY_ANDROID
    private IEnumerator InitReview(bool force = false)
    {
        if (_reviewManager == null) _reviewManager = new ReviewManager();

        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            if (force) DirectlyOpen();
            yield break;
        }

        _playReviewInfo = requestFlowOperation.GetResult();
    }

    public IEnumerator LaunchReview()
    {
        Debug.Log("LaunchReview");
        if (_playReviewInfo == null)
        {
            if (_coroutine != null) StopCoroutine(_coroutine);
            yield return StartCoroutine(InitReview(true));
        }

        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        _playReviewInfo = null;
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            DirectlyOpen();
            yield break;
        }
    }
#endif

    private void DirectlyOpen()
    {
        Debug.Log("DirectlyOpen");
        Application.OpenURL($"https://play.google.com/store/apps/details?id={Application.identifier}");
    }
}
#endregion

