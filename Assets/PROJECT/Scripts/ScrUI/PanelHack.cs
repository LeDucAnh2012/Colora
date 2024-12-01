using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PanelHack : PanelBase
{
    [SerializeField] private Button btnUseIdTest;
    [SerializeField] private Button btnRemoveAds;
    [SerializeField] private Button btnDebug;
    [SerializeField] private Button btnClearData;
    [SerializeField] private InputField inputAmountQuestDone;
    public void ShowPanel()
    {
        base.Show();

        btnUseIdTest.image.color = VariableSystem.IsUseIdTest ? Color.red : Color.gray;
        btnRemoveAds.image.color = VariableSystem.RemoveAdsHack ? Color.red : Color.gray;
        btnDebug.image.color = VariableSystem.IsDebug ? Color.red : Color.gray;
        btnClearData.image.color = VariableSystem.IsClearData ? Color.red : Color.gray;
    }
    public void OnClickFillAll()
    {
        SoundClickButton();
        gameplayUIManager.levelLoader.FillAll();
    }
  
    public void OnClickReload()
    {
        SoundClickButton();
        SceneManager.LoadScene(TypeSceneCurrent.GameplayScene.ToString());
    }
    public void OnClickUseIDTest()
    {
        SoundClickButton();
        VariableSystem.IsUseIdTest = !VariableSystem.IsUseIdTest;
        btnUseIdTest.image.color = VariableSystem.IsUseIdTest ? Color.red : Color.gray;
    }
    public void OnClickNextDay()
    {
        SoundClickButton();

        VariableSystem.DayOfYear = TimeManager.instance.GetDayOfYear();
        if (VariableSystem.DayOfYear <= TimeManager.instance.GetDayOfYear())
        {
            VariableSystem.DayOfYear = TimeManager.instance.GetDayOfYear() + 1;
            // reset new day

            VariableSystem.IsResetRewardDailyQuest = true;

            VariableSystem.IsCollect = false;
            VariableSystem.IsCollectX2 = false;

            if (VariableSystem.CountReceivedDaily >= 6)
                VariableSystem.CountReceivedDaily = 0;
            else
                VariableSystem.CountReceivedDaily++;

        }
        // reset new day
    }
    public void PlusBooster()
    {
        SoundClickButton();
        VariableSystem.FillByBomBooster += 5;
        VariableSystem.FillByNumBooster += 5;
        VariableSystem.FindBooster += 5;
        gameplayUIManager.boosterManager.UpdateUI();
    }
    public void RemoveAdsHack()
    {
        SoundClickButton();
        VariableSystem.RemoveAdsHack = !VariableSystem.RemoveAdsHack;
        btnRemoveAds.image.color = VariableSystem.RemoveAdsHack ? Color.red : Color.gray;
    }
    public void OnClickDebug()
    {
        SoundClickButton();
        VariableSystem.IsDebug = !VariableSystem.IsDebug;
        btnDebug.image.color = VariableSystem.IsDebug ? Color.red : Color.gray;
        KeepObjectDebug.instance.gameObject.SetActive(VariableSystem.IsDebug);
    }

    public void EventPointerDown()
    {
        if (gameplayUIManager != null)
            gameplayUIManager.EventPointerDown();
    }
    public void EventPointerUp()
    {
        if(gameplayUIManager!=null)
        gameplayUIManager.EventPointerUp();
    }
    public void OnClickSetDoneQuest()
    {
        SoundClickButton();
        int val = int.Parse(inputAmountQuestDone.text);
        var count = 0;

        var list = ActionHelper.ConfigListIntFromString(VariableSystem.ListTaskOn);
        for (int i = 0; i < InitDataGame.instance.listDataDailyQuest.Count - 1; i++)
        {
            if (list.Contains(InitDataGame.instance.listDataDailyQuest[i].id) && count < val)
            {
                InitDataGame.instance.listDataDailyQuest[i].SetDoneQuest();
                count++;
            }
        }
    }
    public void OnClickClearData()
    {
        SoundClickButton();
        VariableSystem.IsClearData = !VariableSystem.IsClearData;
        btnClearData.image.color = VariableSystem.IsClearData ? Color.red : Color.gray;
        // VariableSystem.IsClearData = true;
        // SceneManager.LoadScene(TypeSceneCurrent.BeginScene.ToString());
    }
}
