using GoogleMobileAds.Ump.Api;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HomeUIManager : MonoBehaviour
{
    public static HomeUIManager instance;

    [TabGroup("UI")][SerializeField] private Button btnRemoveAds;

    [Space][TabGroup("UI")][SerializeField] private Button btnLibrary;
    [TabGroup("UI")][SerializeField] private Button btnDailyQuest;
    [TabGroup("UI")][SerializeField] private Button btnDailyGift;
    [TabGroup("UI")][SerializeField] private Button btnDIY;
    [TabGroup("UI")][SerializeField] private Button btnMyWork;

    [Space][TabGroup("Panel")] public PopupSetting popupSetting;
    [TabGroup("Panel")] public PanelLevel panelLevel;
    [TabGroup("Panel")] public PanelMyWork panelMyWork;
    [TabGroup("Panel")] public PanelDIY panelDIY;
    [TabGroup("Panel")] public PanelDailyGift panelDailyGift;

    [TabGroup("Panel")] public PanelDailyQuest panelDailyQuest;

    public ManagerNativeAds managerNativeAds;

    private void Awake()
    {
        instance = this;


        if (ActionHelper.IsEditor() && KeepObject.instance.isHack)
        {
            VariableSystem.FindBooster = 10000;
            VariableSystem.FillByNumBooster = 10000;
            VariableSystem.FillByBomBooster = 10000;
        }
      //  managerNativeAds.Show();
        ConfigBtnRemoveAds();
        RemoteConfig.instance.CheckRemoteConfigValues();
    }
    private void Start()
    {
        panelLevel.LoadData();
        panelMyWork.LoadData();
        panelDailyGift.InitData();

        btnCurrent = btnLibrary;
        OnChoose(btnCurrent, true);
        popupSetting.Init();

        if (VariableSystem.IsResetRewardDailyQuest)
        {
            panelDailyQuest.ResetDataReward();
            VariableSystem.IsResetRewardDailyQuest = false;
        }
        else
        {
            panelDailyQuest.LoadData();
            panelDailyQuest.InitData();
        }
        if (KeepObject.instance.mode == TypeMode.Marketing)
            panelDailyQuest.SetDoneAllQuest();
    }
    public void ConfigBtnRemoveAds()
    {
        btnRemoveAds.gameObject.SetActive(!VariableSystem.RemoveAds);
        if (VariableSystem.RemoveAds)
            managerNativeAds.Hide();
    }
    public void RemoveAds()
    {
        SoundClickButton();
        ActionHelper.LogEvent(KeyLogFirebase.RemoveAdsClick);
        ActionHelper.BuyIAP(IAP_Product.RemoveAds, CallBack);
    }
    private void CallBack(bool isComplete)
    {
        if (!isComplete)
        {
            CanvasAllScene.instance.ShowNoti(I2.Loc.ScriptLocalization.Purchase_failed.ToUpper());
        }
        else
        {
            ActionHelper.LogEvent(KeyLogFirebase.RemoveAdsSuccess);
            VariableSystem.RemoveAds = true;
            CanvasAllScene.instance.ShowNoti(I2.Loc.ScriptLocalization.Purchase_success.ToUpper());
        }
        ConfigBtnRemoveAds();
    }

    #region Funct Onclick

    public void OnClickSetting()
    {
        SoundClickButton();
        managerNativeAds.Hide();
        popupSetting.Show();
    }

    private Button btnCurrent;
    public void OnClickLibrary()
    {
        SoundClickButton();
        ConfigBtnRemoveAds();
        ConfigBtnFunct(ref btnLibrary, panelLevel);
        managerNativeAds.Show();
    }
    public void OnClickDailyQuest()
    {
        SoundClickButton();
        ConfigBtnFunct(ref btnDailyQuest, panelDailyQuest);
        ConfigBtnRemoveAds();
    }
    public void OnClickDailyGift()
    {
        SoundClickButton();
        ConfigBtnFunct(ref btnDailyGift, panelDailyGift);
        ConfigBtnRemoveAds();
    }
    public void OnClickDIY()
    {
        SoundClickButton();
        ConfigBtnFunct(ref btnDIY, panelDIY);
    }

    public void OnClickMyWork()
    {
        SoundClickButton();
        ConfigBtnFunct(ref btnMyWork, panelMyWork);
        ConfigBtnRemoveAds();
    }
    private void ConfigBtnFunct(ref Button btn, PanelBase panel)
    {
        if (btnCurrent != null)
        {
            OnChoose(btnCurrent, false);
        }
        btnCurrent = btn;
        OnChoose(btnCurrent, true);

        managerNativeAds.Hide();
        panelLevel.Hide();
        panelDailyQuest.Hide();
        panelDIY.Hide();
        panelMyWork.Hide();
        panelDailyGift.Hide();

        panel.Show();
    }

    private void OnChoose(Button btn, bool isChoose)
    {
        btn.transform.GetChild(1).gameObject.SetActive(isChoose);
    }

    public void SoundClickButton()
    {
        SoundMusicManager.instance.SoundClickButton();
    }

    #endregion

    public void LoadLevel(ShapeInfo shape, Texture2D texture)
    {
        GameplayController.instance.LoadLevel(shape, texture);

        managerNativeAds.Hide();
        SoundMusicManager.instance.PlayMusic(StateGame.Ingame);

    }
}
