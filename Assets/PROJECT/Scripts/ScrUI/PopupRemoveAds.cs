using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupRemoveAds : PanelBase
{
    private const string KeyShowPop = "IsShowPopRemoveAds";
    public override void Show()
    {
        if (PlayerPrefs.GetInt(KeyShowPop) == 0 && !VariableSystem.RemoveAds)
        {
            PlayerPrefs.SetInt(KeyShowPop, 1);
            base.Show();
        }
    }
    public void OnClickRemoveAds()
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
      
    }
    public void OnClickExit()
    {
        SoundClickButton();
        Hide();
    }
}
