using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PanelLoadingStartGame : MonoBehaviour
{
    [SerializeField] private Image imgBar;
    [SerializeField] private Text txtLoading;
    [SerializeField] private Text txtPersen;

    [SerializeField] private bool isRealLoading = true;

    [HideIf("isRealLoading")][SerializeField] private AnimationCurve curve;
    [HideIf("isRealLoading")][SerializeField] private float timeLoading;
    [HideIf("isRealLoading")][SerializeField] private float timeShowAoa;
    [HideIf("isRealLoading")][SerializeField] private float timeShowCMP = 4;
    [HideIf("isRealLoading")][SerializeField] private bool isEditor = false;

    private AsyncOperation asyncOperation;
    private IEnumerator IE_TEXT_LOADING = null;

    private IEnumerator IE_LOADING = null;
    private IEnumerator IE_LOADING_ASYNC = null;

    private float countTime = 0;

    private bool isShowCMP = false;
    private bool isShowAOA = false;

    private bool isDone = false;
    private bool isCMP = false;
    private bool isAOA = false;
    private bool isContinueLoading = true;
    private void Awake()
    {
        if (VariableSystem.IsClearData)
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }
        Debug.Log(" path = " + Application.persistentDataPath);
    }
    private void Start()
    {
        countTime = 0;
        if (isEditor && ActionHelper.IsEditor())
        {
            timeShowAoa = 1;
            timeLoading = 2;
        }
        DataAllShape.SetHasEffect();

        asyncOperation = SceneManager.LoadSceneAsync(TypeSceneCurrent.HomeScene.ToString());
        asyncOperation.allowSceneActivation = false;
        LoadingProgress();

        //IE_TEXT_LOADING = IE_TextLoading();
        //StartCoroutine(IE_TEXT_LOADING);

        StartCoroutine(ActionHelper.IE_TextLoading(txtLoading, I2.Loc.ScriptLocalization.Loading, 0.2f));

        if (KeepObject.instance.mode == TypeMode.Marketing)
        {
            VariableSystem.RemoveAds = true;
            VariableSystem.RemoveAdsHack = true;
            VariableSystem.FindBooster = 999;
            VariableSystem.FillByNumBooster = 999;
            VariableSystem.FillByBomBooster = 999;
            DataAllShape.UnlockAll();
        }
    }

    public void LoadingProgress()
    {
        isContinueLoading = true;
        if (isRealLoading)
        {
            IE_LOADING_ASYNC = IE_LoadingASync();
            StartCoroutine(IE_LOADING_ASYNC);
        }
        else
        {
            IE_LOADING = IE_Loading();
            StartCoroutine(IE_LOADING);
        }
    }

    #region Loading Real

    private IEnumerator IE_LoadingASync()
    {
        while (!asyncOperation.isDone && !isShowCMP && !isShowAOA)
        {
            float progressValue = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            imgBar.fillAmount = progressValue;
            if (progressValue > 0.8f && !isCMP)
            {
                isCMP = true;
                isShowCMP = true;
                ActionHelper.ShowCMP(CallBackHideCMP);
            }
            if (progressValue > 0.9f && !isAOA)
            {
                isAOA = true;
                isShowAOA = true;
                CC_Interface.instance.IsLaunchAoa = true;
                AdsConfig.instance.CheckShowAoa(CallBackHideAOA);
                //    CallBackHideAOA();
            }
            yield return null;
        }
        if (isDone)
            LoadScene();
    }
    //IEnumerator IELoadTextProgress()
    //{
    //    txtProgress.text = "0%";
    //    while (progress < 100)
    //    {
    //        yield return new WaitForFixedUpdate();
    //        progress += speed * Time.deltaTime * 100f;
    //        var _percent = Mathf.RoundToInt(progress);
    //        _percent = Mathf.Clamp(_percent, 0, 100);
    //        //Debug.Log("========================> Progress= " + progress+"/ Percent= "+_percent);
    //        txtProgress.text = _percent.ToString() + "%";
    //    }
    //}
    private void CallBackHideCMP()
    {
        Debug.Log("CallBackHideCMP");
        isShowCMP = false;
        LoadingProgress();
    }
    private void CallBackHideAOA()
    {
        isDone = true;
        Debug.Log("CallBackHideAOA");
        isShowAOA = false;
        LoadingProgress();
    }


    #endregion

    #region Loading Fake

    private IEnumerator IE_Loading()
    {
        while (countTime <= timeLoading && isContinueLoading)
        {
            yield return new WaitForEndOfFrame();
            countTime += Time.deltaTime;

            float per = curve.Evaluate(countTime / timeLoading);

            imgBar.fillAmount = per;
            per *= 100;

            txtPersen.text = (int)per + "%";
            if (countTime >= timeShowCMP && !isShowCMP)
            {
                isShowCMP = true;
                isContinueLoading = false;
                ActionHelper.ShowCMP(LoadingProgress);
            }
            if (countTime >= timeShowAoa && !isShowAOA)
            {
                isShowAOA = true;
                LoadAoa();
            }
        }
        LoadScene();
    }
    private void LoadAoa()
    {
        CC_Interface.instance.IsLaunchAoa = true;
        AdsConfig.instance.CheckShowAoa();
    }
    private void LoadCollapseBanner()
    {
        //CC_BannerCollapse.instance.LoadAd();
    }

    private void LoadScene()
    {
        if (RemoteConfig.instance.allConfigData.BannerCollapInLoading)
            ActionHelper.ShowBannerCollapse(false, () =>
            {
                asyncOperation.allowSceneActivation = true;
            });
        else
            asyncOperation.allowSceneActivation = true;
    }
    #endregion

    IEnumerator IE_TextLoading()
    {
        int i = 0;
        while (true)
        {
            switch (i)
            {
                case 0:
                    txtLoading.text = I2.Loc.ScriptLocalization.Loading + ".";
                    break;
                case 1:
                    txtLoading.text = I2.Loc.ScriptLocalization.Loading + "..";
                    break;
                case 2:
                    txtLoading.text = I2.Loc.ScriptLocalization.Loading + "...";
                    i = -1;
                    break;
            }

            i++;
            yield return new WaitForSeconds(0.2f);
        }
    }
}