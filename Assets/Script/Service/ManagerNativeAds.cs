using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManagerNativeAds : MonoBehaviour
{
    private NativeAd adNative;
    private bool nativeLoaded = false;
    public string idNative;
    public string idNative_2;

    [SerializeField] RawImage adIcon;
    [SerializeField] RawImage adImage;
    [SerializeField] Text adHeadline;
    [SerializeField] Text adCallToAction;
    [SerializeField] Text adAdvertiser;
    public bool nativeAdsLoaded;

    public GameObject objLoaded, objNotLoaded, objAll;
    private int retryAttempt_Native = 0;
    public void Show()
    {
        if (!VariableSystem.RemoveAds && !VariableSystem.RemoveAdsHack)
        {
            objAll.SetActive(true);
            ReloadNativeAds();
        }
        else
        {
            objAll.SetActive(false);
            objNotLoaded.SetActive(false);
        }
    }
    private void ReloadNativeAds()
    {
        if (!nativeAdsLoaded)
        {
            objNotLoaded.SetActive(true);
            Debug.Log("ReloadNativeAds");
            StartLoad();
        }
    }
    public void Hide()
    {
        objAll.SetActive(false);
    }
    public void StartLoad()
    {
        if (!VariableSystem.RemoveAds)
        {
            nativeAdsLoaded = false;
            RequestNativeAd();
        }
        else
        {
            objAll.SetActive(false);
        }
    }
    void Update()
    {
        if (nativeLoaded)
        {
            nativeLoaded = false;
            nativeAdsLoaded = true;
            Texture2D iconTexture = this.adNative.GetIconTexture();
            List<Texture2D> iconPanel = this.adNative.GetImageTextures();
            string headline = this.adNative.GetHeadlineText();
            string cta = this.adNative.GetCallToActionText();
            string advertiser = this.adNative.GetAdvertiserText();
            double star = this.adNative.GetStarRating();
            adIcon.texture = iconTexture;
            adHeadline.text = headline;
            adAdvertiser.text = advertiser;
            adCallToAction.text = cta;
            adImage.texture = iconPanel[0];
            //register gameobjects
            adNative.RegisterIconImageGameObject(adIcon.gameObject);
            adNative.RegisterHeadlineTextGameObject(adHeadline.gameObject);
            adNative.RegisterCallToActionGameObject(adCallToAction.gameObject);
            adNative.RegisterAdvertiserTextGameObject(adAdvertiser.gameObject);
            adNative.RegisterImageGameObjects(new List<GameObject> { adImage.gameObject });
            objLoaded.SetActive(true);
            //objAll.SetActive(true);
            objNotLoaded.SetActive(false);
            //GameServiceControl.myInstance.analystics.FireEvent("NATIVE_ADS");
        }
    }

    #region Native Ad Mehods ------------------------------------------------
    private bool isChangeID = false;
    public void RequestNativeAd()
    {
        isChangeID = !isChangeID;
        string id = isChangeID ? idNative : idNative_2;

        if (VariableSystem.IsUseIdTest)
        {
            if (ActionHelper.IsAndroid())
                id = "ca-app-pub-3940256099942544/2247696110";
            if (ActionHelper.IsIOS())
                id = "ca-app-pub-3940256099942544/3986624511";
            Debug.Log("Key test Native = " + id);
        }

        //AdLoader adLoader = new AdLoader.Builder(id)
        //.ForNativeAd()
        //.Build();
        //adLoader.OnNativeAdLoaded += this.HandleNativeAdLoaded;
        //adLoader.OnAdFailedToLoad += this.HandleNativeAdFailedToLoad;
        //adLoader.OnNativeAdImpression += this.AdsImpression;
        // adLoader.LoadAd(new AdRequest.Builder().Build());

        AdLoader adLoader = new AdLoader.Builder(id).ForNativeAd().Build();

        adLoader.OnNativeAdLoaded += this.HandleNativeAdLoaded;
        adLoader.OnAdFailedToLoad += this.HandleAdFailedToLoad;
     //   adLoader.LoadAd(new AdRequest.Builder().Build());
    }

    private void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.Log("Native ad failed to load: " + args.ToString());
        Invoke(nameof(ReloadNativeAds), ActionHelper.GetTimeDelayReloadAds(ref retryAttempt_Native));
    }
    private void HandleNativeAdLoaded(object sender, NativeAdEventArgs args)
    {
        Debug.Log("Native ad loaded.");
        this.adNative = args.nativeAd;
        args.nativeAd.OnPaidEvent += AdsImpression;
        nativeLoaded = true;
        retryAttempt_Native = 0;
    }

    private void AdsImpression(object sender, AdValueEventArgs args)
    {
        ActionHelper.TrackRevenue_Event(TypeAds.NativeAds, args.AdValue);
    }
  
    #endregion
}
