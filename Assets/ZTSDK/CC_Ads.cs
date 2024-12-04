using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using GoogleMobileAds.Ump.Api;
using System.Linq;
using GoogleMobileAds.Api;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.UI;
using Unity.VisualScripting;
using Singular;
public enum TypeAds
{
    Banner,
    BannerCollapse,
    Inter,
    Reward,
    Aoa,
    NativeAds,
    NativeOverlay,
    IAP
}
public class CC_Ads : UnitySingleton<CC_Ads>
{

    private string _adUnitId_Banner = "";
    private string _adUnitId_Inter = "";
    private string _adUnitId_Reward = "";

    private BannerView _bannerView;
    private InterstitialAd _interstitialAd;
    private RewardedAd _rewardedAd;

    private int retryAttempt_Inter = 0;
    private int retryAttempt_Reward = 0;
    private int retryAttempt_Banner = 0;

    private bool isLoadBanner;
    private bool isLoadInter;
    private bool isLoadReward;
    private bool isRewarded;

    private Action<bool> callbackReward;
    private string whereReward;

    public Action<bool> callbackInter;
    private bool IsReloadAds = true;

    private string deviceIDTest2;
    private bool isLoadAds = false;
    private List<int> listLevelShowCMP = new List<int>();
    public bool isShowCMP = false;
    private bool isContinueLoading = false;
    public void Start()
    {
        RegisterKeyAds();
     
        //PlayerPrefs.DeleteAll();
        //   StartCMP();
        //deviceIDTest2 = SystemInfo.deviceUniqueIdentifier;
        InitAds();
    }
    private void RegisterKeyAds()
    {
        // banner
#if UNITY_ANDROID
        _adUnitId_Banner = "ca-app-pub-7702289911487047/4627329150";
#elif UNITY_IOS
        _adUnitId_Banner = "";
#else
        _adUnitId_Banner = "unused";
#endif
        if (VariableSystem.IsUseIdTest)
        {
#if UNITY_ANDROID
            _adUnitId_Banner = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IOS
            _adUnitId_Banner = "ca-app-pub-3940256099942544/2934735716";
#endif
        }

        // inter
#if UNITY_ANDROID
        _adUnitId_Inter = "ca-app-pub-7702289911487047/3314247486";
#elif UNITY_IOS
        _adUnitId_Inter = "";
#else
        _adUnitId_Inter = "unused";
#endif
        if (VariableSystem.IsUseIdTest)
        {
#if UNITY_ANDROID
            _adUnitId_Inter = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IOS
            _adUnitId_Inter = "ca-app-pub-3940256099942544/4411468910";
#endif
        }

        // reward
#if UNITY_ANDROID
        _adUnitId_Reward = " ca-app-pub-7702289911487047/9688084149";
#elif UNITY_IOS
        _adUnitId_Reward = "";
#else
        _adUnitId_Reward = "unused";
#endif
        if (VariableSystem.IsUseIdTest)
        {
#if UNITY_ANDROID
            _adUnitId_Reward = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IOS
            _adUnitId_Reward = "ca-app-pub-3940256099942544/1712485313";
#endif
        }
    }
    public void InitAds()
    {
        if (!isLoadAds)
        {
        Debug.Log("InitAds");
            isLoadAds = true;
            // Initialize the Google Mobile Ads SDK.
            MobileAds.Initialize((InitializationStatus initStatus) =>
            {
                // This callback is called once the MobileAds SDK is initialized
                Debug.Log("Load Reward");
                LoadReward();
                LoadBanner();
                LoadInterAds();
                ListenToAdEvents();

            });
        }
    }

    #region Banner

    /// <summary>
    /// Creates a 320x50 banner view at top of the screen.
    /// </summary>
    public void CreateBannerView()
    {
        Debug.Log("Creating banner view");

        // If we already have a banner, destroy the old one.
        if (_bannerView != null)
        {
            DestroyAd();
        }

        // Create a 320x50 banner at top of the screen
        _bannerView = new BannerView(_adUnitId_Banner, AdSize.Banner, AdPosition.Bottom);

    }
    /// <summary>
    /// Creates the banner view and loads a banner ad.
    /// </summary>
    private void LoadAd()
    {
        // create an instance of a banner view first.
        if (_bannerView == null)
        {
            CreateBannerView();
        }

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        Debug.Log("Loading banner ad.");
        _bannerView.LoadAd(adRequest);
    }
    /// <summary>
    /// listen to events the banner view may raise.
    /// </summary>
    private void ListenToAdEvents()
    {
        // Raised when an ad is loaded into the banner view.
        _bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner view loaded an ad with response : "
                + _bannerView.GetResponseInfo());
            CC_Interface.instance.HasBanner = true;
            isLoadBanner = false;
            retryAttempt_Banner = 0;

        };
        // Raised when an ad fails to load into the banner view.
        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner view failed to load an ad with error : "
                + error);
            isLoadBanner = false;
            Invoke(nameof(LoadBanner), ActionHelper.GetTimeDelayReloadAds(ref retryAttempt_Banner));
        };
        // Raised when the ad is estimated to have earned money.
        _bannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Banner view paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
            ActionHelper.TrackRevenue_Event(TypeAds.Banner, adValue);

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
            Debug.Log("Banner view full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        _bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Banner view full screen content closed.");
        };
    }

    /// <summary>
    /// Destroys the banner view.
    /// </summary>
    public void DestroyAd()
    {
        if (_bannerView != null)
        {
            Debug.Log("Destroying banner view.");
            _bannerView.Destroy();
            _bannerView = null;
        }
    }


    public void LoadBanner()
    {
        if (VariableSystem.RemoveAds || VariableSystem.RemoveAdsHack)
            return;

        if (    !TimeManager.instance.IsHasInternet || isLoadBanner || CC_Interface.instance.HasBanner)
            return;

        if (ActionHelper.GetSceneCurrent() == TypeSceneCurrent.BeginScene)
        {
            if (!RemoteConfig.instance.allConfigData.BannerCollapInLoading)
            {
                CanvasAllScene.instance.panelLoading.Hide();
                LoadBannerNormal();
                return;
            }
        }
        else
            LoadBannerNormal();
    }
    public void LoadBannerNormal()
    {
        isLoadBanner = true;
        Debug.Log("banner loaded");
        LoadAd();

    }
    #endregion



    #region Inter

    /// <summary>
    /// Loads the interstitial ad.
    /// </summary>
    public void LoadInterAds()
    {
        if (  !TimeManager.instance.IsHasInternet || isLoadInter || VariableSystem.RemoveAds)
            return;
        Debug.Log("inter loaded");
        isLoadInter = true;
        // Clean up the old ad before loading a new one.
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        InterstitialAd.Load(_adUnitId_Inter, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    isLoadInter = false;

                    Invoke(nameof(LoadInterAds), ActionHelper.GetTimeDelayReloadAds(ref retryAttempt_Inter));
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }


                ActionHelper.LogEvent(KeyLogFirebase.LoadInter);
                isLoadInter = false;
                retryAttempt_Inter = 0;

                Debug.Log("Interstitial ad loaded with response : " + ad.GetResponseInfo());

                _interstitialAd = ad;
                RegisterEventHandlers(_interstitialAd);
                RegisterReloadHandler(_interstitialAd);
            });
    }

    private void RegisterEventHandlers(InterstitialAd interstitialAd)
    {
        // Raised when the ad is estimated to have earned money.
        interstitialAd.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
            ActionHelper.TrackRevenue_Event(TypeAds.Inter, adValue);
        };
        // Raised when an impression is recorded for an ad.
        interstitialAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        interstitialAd.OnAdClicked += () =>
        {
            Debug.Log("Interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            ActionHelper.LogEvent(KeyLogFirebase.ShowInterSuccess);
            Debug.Log("Interstitial ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            LoadInterAds();
            callbackInter?.Invoke(true);
            callbackInter = null;
            CC_Interface.instance.IsShowingAd = false;
            ActionHelper.LogEvent(KeyLogFirebase.CloseInter);
            Debug.Log("Interstitial ad full screen content closed.");
        };
        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            CC_Interface.instance.IsShowingAd = false;
            callbackInter?.Invoke(false);
            callbackInter = null;

            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);
        };
    }

    private void RegisterReloadHandler(InterstitialAd interstitialAd)
    {
        // Raised when the ad closed full screen content.
        interstitialAd.OnAdFullScreenContentClosed += () =>
    {
        Debug.Log("Interstitial Ad full screen content closed.");

        // Reload the ad so that we can show another as soon as possible.
        LoadInterAds();
    };
        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);

            // Reload the ad so that we can show another as soon as possible.
            LoadInterAds();
        };
    }
    public void ShowInter(Action<bool> callback = null)
    {
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            Debug.Log("Showing interstitial ad.");
            callbackInter = callback;
            _interstitialAd.Show();
        }
        else
        {
            callback?.Invoke(false);
            LoadInterAds();
            Debug.Log("InterstitialNotReady");
        }
    }


    #endregion

    #region Reward

    /// <summary>
    /// Loads the rewarded ad.
    /// </summary>
    public void LoadReward()
    {
        if ( !TimeManager.instance.IsHasInternet || isLoadReward)
            return;
        Debug.Log("reward loaded");
        isLoadReward = true;

        // Clean up the old ad before loading a new one.
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        Debug.Log("Loading the rewarded ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedAd.Load(_adUnitId_Reward, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Invoke(nameof(LoadReward), ActionHelper.GetTimeDelayReloadAds(ref retryAttempt_Reward));
                    isLoadReward = false;
                    Debug.LogError("Rewarded ad failed to load an ad " + "with error : " + error);
                    return;
                }
                retryAttempt_Reward = 0;
                isLoadReward = false;
                Debug.Log("Rewarded ad loaded with response : " + ad.GetResponseInfo());

                _rewardedAd = ad;
                RegisterEventHandlers(_rewardedAd);
                RegisterReloadHandler(_rewardedAd);
            });
    }
    public void ShowRewardedAd(string where = "", Action<bool> callback = null)
    {
        const string rewardMsg =
            "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            isLoadReward = false;
            callbackReward = callback;
            whereReward = where;

            AdsConfig.instance.countTimeInterReward.time = 0;
            AdsConfig.instance.countTimeInterReward.isCountTime = true;

            CC_Interface.instance.IsShowingAd = true;
            isRewarded = false;

            ActionHelper.LogEvent(KeyLogFirebase.AdsRewardCompleted + where);

            _rewardedAd.Show((Reward reward) =>
            {
                // TODO: Reward the user.
                ActionHelper.LogEvent(whereReward);
                InitDataGame.instance.listDataDailyQuest[5].CountFinishQuest++;
                callbackReward?.Invoke(true);
                callbackReward = null;
                Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));

            });
        }
        else
        {
            isLoadReward = false;
            if (ActionHelper.IsEditor())
            {
                AdsConfig.instance.countTimeInterReward.time = 0;
                AdsConfig.instance.countTimeInterReward.isCountTime = true;
            }

            CanvasAllScene.instance.ShowNoti(I2.Loc.ScriptLocalization.Ads_is_not_ready);
            callback?.Invoke(false);
            LoadReward();
        }
    }
    private void RegisterEventHandlers(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
            ActionHelper.TrackRevenue_Event(TypeAds.Reward, adValue);
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            isRewarded = true;


            StartCoroutine(IE_ResetStateAds());
            Debug.Log("Rewarded ad full screen content closed.");
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {

            CC_Interface.instance.IsShowingAd = false;

            callbackReward?.Invoke(false);
            callbackReward = null;
            Debug.LogError("Rewarded ad failed to open full screen content " + "with error : " + error);
        };
    }
    private void RegisterReloadHandler(RewardedAd ad)
    {
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded Ad full screen content closed.");

            // Reload the ad so that we can show another as soon as possible.
            LoadReward();
            StartCoroutine(IE_ResetStateAds());
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);

            // Reload the ad so that we can show another as soon as possible.
            LoadReward();
        };
    }
    private IEnumerator IE_ResetStateAds()
    {
        yield return new WaitForSeconds(1f);
        CC_Interface.instance.IsShowingAd = false;
    }
    #endregion

    #region ImpressionSuccess callback handler
    void ImpressionSuccessEvent(AdValue adValue)
    {

        //{
        //    Debug.Log("ImpressionSuccessEvent");

            SingularSDK.Revenue("USD", adValue.Value / 1000000);
        //    AdjustAdRevenue adjustAdRevenue = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceIronSource);
        //    adjustAdRevenue.setRevenue((double)impressionData.revenue, "USD");
        //    optional fields
        //    adjustAdRevenue.setAdRevenueNetwork(impressionData.adNetwork);
        //    adjustAdRevenue.setAdRevenueUnit(impressionData.adUnit);
        //    adjustAdRevenue.setAdRevenuePlacement(impressionData.placement);
        //    track Adjust ad revenue
        //    Adjust.trackAdRevenue(adjustAdRevenue);



        //    Tracking firebase
        //    Firebase.Analytics.Parameter[] AdParameters = {
        //     new Firebase.Analytics.Parameter("ad_platform", "ironSource"),
        //      new Firebase.Analytics.Parameter("ad_source", impressionData.adNetwork),
        //      new Firebase.Analytics.Parameter("ad_unit_name", impressionData.adUnit),
        //    new Firebase.Analytics.Parameter("ad_format", impressionData.instanceName),
        //      new Firebase.Analytics.Parameter("currency","USD"),
        //    new Firebase.Analytics.Parameter("value", (double)impressionData.revenue)
        //};
        //    Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", AdParameters);
        //}
    }
    #endregion

    #region CMP

    //    void StartCMP()
    //    {
    //        if (isCMPConsent())
    //        {
    //            MaxSdk.SetHasUserConsent(true);
    //            InitAds();
    //        }
    //        else
    //        {
    //            MaxSdk.SetHasUserConsent(false);
    //            //ResetCMP();
    //        }
    //    }
    //    void OnConsentInfoUpdated(FormError consentError)
    //    {
    //        if (consentError != null)
    //        {
    //            // Handle the error.
    //            Debug.Log(consentError);
    //            ContinueLoading();
    //            return;
    //        }
    //        // If the error is null, the consent information state was updated.
    //        // You are now ready to check if a form is available.
    //        ConsentForm.LoadAndShowConsentFormIfRequired((FormError formError) =>
    //        {
    //            if (formError != null)
    //            {
    //                // Consent gathering failed.
    //                Debug.Log("consent = " + consentError);
    //                ContinueLoading();
    //                return;
    //            }
    //            // Consent has been gathered.
    //            if (ConsentInformation.CanRequestAds())
    //            {
    //                if (isCMPConsent())
    //                {
    //                    //Request an ad.
    //                    Debug.Log("Bool Check String:" + isCMPConsent());
    //                    Debug.LogError("CMP: START 03");
    //                    MaxSdk.SetHasUserConsent(true);
    //                    ContinueLoading();
    //                }
    //                else
    //                {
    //                    Debug.Log("???");
    //                    MaxSdk.SetHasUserConsent(false);
    //                    ContinueLoading();
    //                }

    //            }
    //            else
    //            {
    //                Debug.Log("=====");
    //                MaxSdk.SetHasUserConsent(false);
    //                ContinueLoading();
    //            }
    //        });
    //    }
    //    private void ContinueLoading()
    //    {
    //        //Time.timeScale = 1;
    //        isContinueLoading = true;
    //        CanvasAllScene.instance.panelLoading.Hide();
    //        if (ActionHelper.GetSceneCurrent() == TypeSceneCurrent.BeginScene)
    //        {
    //            isShowCMP = false;
    //            callbackHideCMP?.Invoke();
    //        }
    //        InitAds();
    //    }
    //    UnityAction callbackHideCMP = null;
    //    public void LevelShowCMP(UnityAction callback = null)
    //    {
    //        if (!isCMPConsent())
    //        {
    //            isContinueLoading = false;
    //            if (ActionHelper.GetSceneCurrent() != TypeSceneCurrent.BeginScene)
    //                CanvasAllScene.instance.panelLoading.Show();
    //            isShowCMP = true;
    //            callbackHideCMP = callback;
    //            ResetCMP();
    //        }
    //        else
    //        {
    //            CanvasAllScene.instance.panelLoading.Hide();
    //            isShowCMP = false;
    //            callback?.Invoke();
    //        }
    //    }
    //    private List<int> ConvertStringToInt(string value)
    //    {
    //        List<int> vs = new List<int>();
    //        string[] bs = value.Split(',');
    //        for (int i = 0; i < bs.Length; i++)
    //        {
    //            vs.Add(int.Parse(bs[i].Trim()));
    //        }

    //        return vs;
    //    }

    //    /// <summary>
    //    /// If user not accept consent, reset consent and show
    //    /// </summary>
    //    public void ResetCMP()
    //    {
    //        Debug.Log("reset cmp");
    //        ConsentInformation.Reset();
    //        var debugSettings = new ConsentDebugSettings
    //        {
    //            // Geography appears as in EEA for debug devices.
    //            DebugGeography = DebugGeography.EEA,
    //            TestDeviceHashedIds = new List<string>
    //        {
    //            deviceIDTest2
    //        }
    //        };
    //        ConsentRequestParameters request = new ConsentRequestParameters
    //        {
    //            TagForUnderAgeOfConsent = false,
    //            ConsentDebugSettings = debugSettings,
    //        };
    //        ConsentInformation.Update(request, OnConsentInfoUpdated);
    //    }

    //    /// <summary>
    //    ///Get ID consent on Android device
    //    /// </summary>
    //    public string GetID()
    //    {
    //        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    //        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    //        AndroidJavaClass preferenceManagerClass = new AndroidJavaClass("android.preference.PreferenceManager");
    //        AndroidJavaObject sharedPreferences = preferenceManagerClass.CallStatic<AndroidJavaObject>("getDefaultSharedPreferences", currentActivity);
    //        return sharedPreferences.Call<string>("getString", "IABTCF_AddtlConsent", "");
    //    }

    //    ///// <summary>
    //    ///// Check if user accepted consent
    //    ///// </summary>
    //    //bool isCMPConsent()
    //    //{
    //    //    if (Application.platform == RuntimePlatform.Android)
    //    //    {
    //    //        string CMPString = GetID();
    //    //        Debug.Log("CMP String = " + CMPString);
    //    //        if (CMPString.Length >= 4 && CMPString.Contains("2878"))
    //    //        {
    //    //            Debug.Log("vao true");
    //    //            return true;
    //    //        }
    //    //        else
    //    //        {
    //    //            Debug.Log("vao false");
    //    //            return false;
    //    //        }
    //    //    }
    //    //    else if (Application.platform == RuntimePlatform.IPhonePlayer)
    //    //    {

    //    //        string CMPString = PlayerPrefs.GetString("IABTCF_AddtlConsent", "");
    //    //        Debug.LogError("CMPString: " + CMPString);
    //    //        if (CMPString.Length >= 4 && CMPString.Contains("2878"))
    //    //        {
    //    //            return true;
    //    //        }
    //    //        else
    //    //        {
    //    //            return false;
    //    //        }
    //    //    }
    //    //    else
    //    //    {
    //    //        return true;
    //    //    }
    //    //}

    //    bool isCMPConsent()
    //    {
    //        if (ActionHelper.IsEditor()) return true;
    //        //Debug.LogError("isCMPConsent CanShowAds: " + CanShowAds() + " ----- CanShowPersonalizedAds: " + CanShowPersonalizedAds());
    //        if (CanShowAds() || CanShowPersonalizedAds())
    //        {
    //            return true;
    //        }
    //        else
    //        {
    //            return false;
    //        }
    //    }


    //    private const string PURPOSE_CONSENT_KEY = "IABTCF_PurposeConsents";
    //    private const string VENDOR_CONSENT_KEY = "IABTCF_VendorConsents";
    //    private const string VENDOR_LI_KEY = "IABTCF_VendorLegitimateInterests";
    //    private const string PURPOSE_LI_KEY = "IABTCF_PurposeLegitimateInterests";
    //    // Check if ads can be shown
    //    bool CanShowAds()
    //    {
    //        string purposeConsent = "";
    //        string vendorConsent = "";
    //        string vendorLI = "";
    //        string purposeLI = "";
    //#if UNITY_IPHONE
    //            purposeConsent = PlayerPrefs.GetString(PURPOSE_CONSENT_KEY, "");
    //            vendorConsent = PlayerPrefs.GetString(VENDOR_CONSENT_KEY, "");
    //            vendorLI = PlayerPrefs.GetString(VENDOR_LI_KEY, "");
    //            purposeLI = PlayerPrefs.GetString(PURPOSE_LI_KEY, "");
    //#elif UNITY_ANDROID
    //        purposeConsent = GT.Utils.PlayerPrefsNative.GetString(PURPOSE_CONSENT_KEY, "");
    //        vendorConsent = GT.Utils.PlayerPrefsNative.GetString(VENDOR_CONSENT_KEY, "");
    //        vendorLI = GT.Utils.PlayerPrefsNative.GetString(VENDOR_LI_KEY, "");
    //        purposeLI = GT.Utils.PlayerPrefsNative.GetString(PURPOSE_LI_KEY, "");
    //#endif

    //        int googleId = 755;
    //        bool hasGoogleVendorConsent = HasAttribute(vendorConsent, googleId);
    //        bool hasGoogleVendorLI = HasAttribute(vendorLI, googleId);

    //        // Minimum required for at least non-personalized ads
    //        return HasConsentFor(new List<int> { 1 }, purposeConsent, hasGoogleVendorConsent)
    //            && HasConsentOrLegitimateInterestFor(new List<int> { 2, 7, 9, 10 }, purposeConsent, purposeLI, hasGoogleVendorConsent, hasGoogleVendorLI);
    //    }

    //    // Check if personalized ads can be shown
    //    bool CanShowPersonalizedAds()
    //    {
    //        string purposeConsent = "";
    //        string vendorConsent = "";
    //        string vendorLI = "";
    //        string purposeLI = "";

    //#if UNITY_IPHONE
    //            purposeConsent = PlayerPrefs.GetString(PURPOSE_CONSENT_KEY, "");
    //            vendorConsent = PlayerPrefs.GetString(VENDOR_CONSENT_KEY, "");
    //            vendorLI = PlayerPrefs.GetString(VENDOR_LI_KEY, "");
    //            purposeLI = PlayerPrefs.GetString(PURPOSE_LI_KEY, "");
    //#elif UNITY_ANDROID
    //        purposeConsent = PlayerPrefs.GetString(PURPOSE_CONSENT_KEY, "");
    //        vendorConsent = PlayerPrefs.GetString(VENDOR_CONSENT_KEY, "");
    //        vendorLI = PlayerPrefs.GetString(VENDOR_LI_KEY, "");
    //        purposeLI = PlayerPrefs.GetString(PURPOSE_LI_KEY, "");
    //#endif
    //        int googleId = 755;
    //        bool hasGoogleVendorConsent = HasAttribute(vendorConsent, googleId);
    //        bool hasGoogleVendorLI = HasAttribute(vendorLI, googleId);

    //        return HasConsentFor(new List<int> { 1, 3, 4 }, purposeConsent, hasGoogleVendorConsent)
    //            && HasConsentOrLegitimateInterestFor(new List<int> { 2, 7, 9, 10 }, purposeConsent, purposeLI, hasGoogleVendorConsent, hasGoogleVendorLI);
    //    }

    //    // Check if a binary string has a "1" at position "index" (1-based)
    //    bool HasAttribute(string input, int index)
    //    {
    //        return input.Length >= index && input[index - 1] == '1';
    //    }

    //    // Check if consent is given for a list of purposes
    //    bool HasConsentFor(List<int> purposes, string purposeConsent, bool hasVendorConsent)
    //    {
    //        return purposes.All(p => HasAttribute(purposeConsent, p)) && hasVendorConsent;
    //    }

    //    // Check if a vendor either has consent or legitimate interest for a list of purposes
    //    bool HasConsentOrLegitimateInterestFor(List<int> purposes, string purposeConsent, string purposeLI, bool hasVendorConsent, bool hasVendorLI)
    //    {
    //        return purposes.All(p =>
    //            (HasAttribute(purposeLI, p) && hasVendorLI) ||
    //            (HasAttribute(purposeConsent, p) && hasVendorConsent)
    //        );
    //    }


    #endregion
}
