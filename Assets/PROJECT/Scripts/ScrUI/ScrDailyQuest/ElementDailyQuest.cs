using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementDailyQuest : PanelBase
{
    public int id;
    [SerializeField] private Image iconQuest;
    [SerializeField] private Text txtNameQuest;
    [SerializeField] private Text txtProgress;
    [SerializeField] private Image imgProgress;
    [SerializeField] private Image imgComplete;
    [SerializeField] private Image imgGift;
    [SerializeField] private Image imgAds;
    [SerializeField] private Text txtClaim;
    [SerializeField] private Text txtValBooster;
    [SerializeField] private GameObject objNoti;

    [SerializeField] private Sprite sprClaimAds;
    [SerializeField] private Sprite sprClaim;

    public DataDailyQuest data;
    public bool isGetting = false;
    public void InitData(DataDailyQuest data, ref bool isGet)
    {
        this.data = data;

        string str = I2.Loc.LocalizationManager.GetTermTranslation(data.nameQuest);
        iconQuest.transform.localScale = Vector2.one;
        if (data.id != 6)
        {
            if (data == null)
            {
                Debug.Log("data null");
            }
            else
            {
                str = str.Replace("xx", data.AmountQuest.ToString());

                //  str = str.Replace("xx", data.AmountQuest.ToString());
            }
            txtValBooster.gameObject.SetActive(true);
            var spr = DataAllShape.GetDataBooster(data.listTypeBooster[0]).sprBooster;
            txtValBooster.text = "+" + data.amountReward;
            iconQuest.sprite = spr;

            txtProgress.gameObject.SetActive(true);
            txtProgress.text = data.CountFinishQuest + "/" + data.AmountQuest;
        }
        else
        {
            txtProgress.gameObject.SetActive(true);
            txtProgress.text = data.CountFinishQuest + "/" + data.AmountQuest;

            txtValBooster.gameObject.SetActive(false);
            iconQuest.transform.localScale = Vector2.one * 3;
            iconQuest.sprite = data.sprGiftReward;

            if (DataAllShape.CheckUnlockAllShapeDailyQuest())
            {
                iconQuest.transform.localScale = Vector2.one;
                txtValBooster.gameObject.SetActive(true);
                txtValBooster.text = "+" + data.amountReward;
            }
        }

        if (data.id == 4)
        {
            var type = (TypeTopic)data.AmountQuest_2;
            str = str.Replace("yy", type.ToString());
        }

        if (data.id == 2)
        {
            TypeBooster _typeBooster = TypeBooster.Number;
            string _type = data.GetRewardQuest();
            System.Enum.TryParse(_type, out _typeBooster);

            _type = ActionHelper.ClassifyTypeBooster(_typeBooster);
            try
            {
                str = str.Replace("booster", _type);
            }
            catch
            {
                Debug.Log("catch" + data.nameQuest);
            }
        }

        iconQuest.SetNativeSize();
        txtNameQuest.text = str.ToUpper();

        imgProgress.fillAmount = (float)data.CountFinishQuest / data.AmountQuest;


        isGetting = false;
        objNoti.SetActive(false);
        imgGift.sprite = sprClaimAds;

        txtClaim.rectTransform.anchoredPosition = new Vector2(29.4f, 0);
        imgAds.gameObject.SetActive(true);
        imgComplete.gameObject.SetActive(false);

        if (data.CountFinishQuest >= data.AmountQuest)
        {
            if (data.IsGetQuest)
            {
                GetComponent<Image>().raycastTarget = false;
                imgGift.sprite = sprClaim;
                imgComplete.gameObject.SetActive(true);
                imgAds.gameObject.SetActive(false);
            }
            else
            {
                // rung box
                txtClaim.rectTransform.anchoredPosition = new Vector2(0, 0);
                imgAds.gameObject.SetActive(false);
                VariableSystem.SetLogTask(id);
                imgGift.sprite = sprClaim;
                objNoti.SetActive(true);
                isGet = true;
                isGetting = true;
                imgComplete.gameObject.SetActive(false);
            }
        }
        else
        {
            if (data.IsGetQuest)
            {
                GetComponent<Image>().raycastTarget = false;
                imgGift.sprite = sprClaim;
                imgComplete.gameObject.SetActive(true);
                imgAds.gameObject.SetActive(false);
            }
        }
    }

    public void OnClickShowGift()
    {
        if (isGetting)
            if (!data.IsGetQuest)
            {
                OnClickGetGift();
                return;
            }

        SoundClickButton();
        homeUIManager.panelDailyQuest.ShowGift(data);

    }
    public void OnClickGetGift()
    {
        if (data.IsGetQuest) return;
        if (!isGetting)
        {
            ActionHelper.ShowRewardAds("Rw_DoneQuest_" + id, Callback);
            return;
        }

        SoundClickButton();
        GetGift();

    }
    private void GetGift()
    {
        ActionHelper.LogEvent(KeyLogFirebase.RewardTask + id);
        imgGift.raycastTarget = false;
        isGetting = false;

        if (data.id != 6)
            ShowGift();
        else
        {
            if (DataAllShape.CheckUnlockAllShapeDailyQuest())
                ShowGift();
            else
                homeUIManager.panelDailyQuest.ShowGift(data, true);
        }
    }
    private void ShowGift()
    {
        var valReward = data.amountReward;

        var listSpr = new List<Sprite>();
        var listVal = new List<int>();
        var listType = new List<TypeBooster>();

        listSpr.Add(DataAllShape.GetDataBooster(data.listTypeBooster[0]).sprBooster);
        listVal.Add(valReward);
        listType.Add(data.listTypeBooster[0]);

        switch (data.listTypeBooster[0])
        {
            case TypeBooster.Number:
                VariableSystem.FillByNumBooster += valReward;
                break;
            case TypeBooster.Find:
                VariableSystem.FindBooster += valReward;
                break;
            case TypeBooster.Bomb:
                VariableSystem.FillByBomBooster += valReward;
                break;
        }

        data.IsGetQuest = true;
        canvasAllScene.popupGetGift.ShowPopup(listSpr, listVal, listType, () =>
        {
            homeUIManager.panelDailyQuest.InitData();
        },false,false);
    }
    private void Callback(bool isComplete)
    {
        if (!isComplete) return;

        isGetting = true;
        data.IsGetQuest = true;
        data.CountFinishQuest = data.AmountQuest;
        GetGift();
    }
}
