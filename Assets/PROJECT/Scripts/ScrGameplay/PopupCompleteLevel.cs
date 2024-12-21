using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PopupCompleteLevel : PanelBase
{
    private bool isWatchAds = false;
    private List<Sprite> listSprite = new List<Sprite>();
    private List<int> listInt = new List<int>();
    private List<string> listStr = new List<string>();
    private List<TypeBooster> listTypeBooster = new List<TypeBooster>();
    private UnityAction callback = null;
    public void ShowPopup(List<Sprite> listSpr, List<int> listVal, List<TypeBooster> listType, UnityAction callback = null)
    {
        Debug.Log("List spr = " + listSpr.Count);
        SoundMusicManager.instance.SoundCompletedLevel();
        listSprite.Clear();
        listTypeBooster.Clear();
        listInt.Clear();

        listSprite = listSpr;
        listTypeBooster = listType;
        listInt = listVal;
        this.callback = callback;

        isWatchAds = false;
        base.Show();

        //  txtTitle.text = strTitle;
    }
    public void OnClickClaimAds()
    {
        Debug.Log("OnClickClaimAds");
        SoundClickButton();
        ActionHelper.ShowRewardAds(KeyLogFirebase.Colora_RW_X2RewardEndGame_211224,"Rw_CompleteLevel_", Callback);

    }
    private void Callback(bool isComplete)
    {
        if (!isComplete) return;
        VariableSystem.FillByNumBooster += 4;
        Continue(6);
        base.Hide();
    }

    public void OnClickNoThanks()
    {
        SoundClickButton();
        ActionHelper.CheckShowInter(KeyLogFirebase.Colora_INT_NoThank_GiftPopup_211224, (bool isShowCompleted) =>
        {
            Continue(2);
            base.Hide();
        });
    }
    public void Continue(int valPlus)
    {
        listStr.Clear();
        for (int i = 0; i < listInt.Count; i++)
            listStr.Add("+" + valPlus);
        Debug.Log("listSprite = " + listSprite.Count);
        canvasAllScene.popupGetGift.ShowPopup(listSprite, listStr, listTypeBooster, () =>
        {
            gameplayUIManager.ShapeDone();
            gameplayUIManager.boosterManager.UpdateUI();
        });
    }
}
