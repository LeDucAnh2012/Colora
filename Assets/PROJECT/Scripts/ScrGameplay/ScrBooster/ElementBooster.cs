using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ElementBooster : PanelBase
{
    [SerializeField] private TypeBooster typeBooster;
    [SerializeField] private Text txtCount;
    [SerializeField] private Image imgFrameCount;
    [SerializeField] private Image imgAds;
    [SerializeField] private Button btnBooster;
    [SerializeField] private Image iconBooster;
    //[SerializeField] private Image iconBoosterEnd;
    [SerializeField] private Image imgFrameUse;
    [SerializeField] private BoosterManager boosterManager;

    public bool isUse = false;
    public void Init()
    {
        isUse = false;
        TurnOn();
    }
    public void UpdateUI()
    {
        switch (typeBooster)
        {
            case TypeBooster.Bomb:
                if (VariableSystem.FillByBomBooster <= 0)
                {
                    OnOffTextAds(false);
                }
                else
                {
                    OnOffTextAds(true);
                    txtCount.text = VariableSystem.FillByBomBooster.ToString();
                }
                break;

            case TypeBooster.Find:

                if (VariableSystem.FindBooster <= 0)
                {
                    OnOffTextAds(false);
                }
                else
                {
                    OnOffTextAds(true);
                    txtCount.text = VariableSystem.FindBooster.ToString();
                }

                break;

            case TypeBooster.Number:

                if (VariableSystem.FillByNumBooster <= 0)
                {
                    OnOffTextAds(false);
                }
                else
                {
                    OnOffTextAds(true);
                    txtCount.text = VariableSystem.FillByNumBooster.ToString();
                }

                break;
        }
    }
    private void OnOffTextAds(bool isOn)
    {
        txtCount.gameObject.SetActive(isOn);
        imgFrameCount.gameObject.SetActive(isOn);
        imgAds.gameObject.SetActive(!isOn);

        //    iconBoosterEnd.gameObject.SetActive(!isOn);
        iconBooster.gameObject.SetActive(true);

        if (!isOn)
        {
            isUse = false;
            boosterManager.SetNumWhenChooseBooster(true);
            TurnOn();
        }
    }
    public void OnClickBooster()
    {
        SoundClickButton();
        SwitchBooster();
    }
    public void SwitchBooster()
    {
        if (typeBooster != TypeBooster.Find)
        {
            if (boosterManager.boosterCurrent != null)
                if (boosterManager.boosterCurrent != this)
                    boosterManager.boosterCurrent.isUse = false;

            boosterManager.boosterCurrent?.TurnOn();
            boosterManager.boosterCurrent = this;
        }

        switch (typeBooster)
        {
            case TypeBooster.Bomb:

                if (VariableSystem.FillByBomBooster <= 0)
                {
                    ActionHelper.ShowRewardAds("Rw_" + typeBooster.ToString(), CallBackAdsBooster);
                    return;
                }
                isUse = !isUse;
                boosterManager.SetNumWhenChooseBooster(!isUse);
                TurnOn();

                break;
            case TypeBooster.Find:
                if (VariableSystem.FindBooster <= 0)
                {
                    ActionHelper.ShowRewardAds("Rw_" + typeBooster.ToString(), CallBackAdsBooster);
                    return;
                }
                SoundMusicManager.instance.SoundUseBooster();

                if (gameplayUIManager.boosterManager.fillByNum.isUse)
                    gameplayUIManager.boosterManager.fillByNum.SwitchBooster();

                if (gameplayUIManager.boosterManager.fillByBomb.isUse)
                    gameplayUIManager.boosterManager.fillByBomb.SwitchBooster();

                gameplayUIManager.levelLoader.Find();
                //if (VariableSystem.FindBooster <= 0)
                //    iconBoosterEnd.gameObject.SetActive(true);
                // log firebase
                ActionHelper.LogEvent(KeyLogFirebase.UseBooster + "_" + typeBooster.ToString());

                break;
            case TypeBooster.Number:
                if (VariableSystem.FillByNumBooster <= 0)
                {
                    ActionHelper.ShowRewardAds("Rw_" + typeBooster.ToString(), CallBackAdsBooster);
                    return;
                }
                isUse = !isUse;
                boosterManager.SetNumWhenChooseBooster(!isUse);
                TurnOn();

                break;
        }

    }
    private void CallBackAdsBooster(bool isComplete)
    {
        if (!isComplete) return;

        switch (typeBooster)
        {
            case TypeBooster.Bomb:

                VariableSystem.FillByBomBooster++;
                break;

            case TypeBooster.Find:
                VariableSystem.FindBooster++;
                //iconBoosterEnd.gameObject.SetActive(false);
                break;

            case TypeBooster.Number:
                VariableSystem.FillByNumBooster++;
                break;
        }
        UpdateUI();
        VisualPlusBooster();
        TurnOn();
    }

    private void VisualPlusBooster()
    {
        var text = Instantiate(boosterManager.txtSpawnPlus, btnBooster.transform);
        text.rectTransform.anchoredPosition = new Vector2(0, 95);
        text.text = "+1";
        text.rectTransform.DOAnchorPosY(text.rectTransform.anchoredPosition.y + 200, 3).SetEase(Ease.Linear).OnComplete(() =>
        {
            Destroy(text.gameObject);
        });
        text.DOFade(0, 2.9f).SetEase(Ease.Linear);
    }

    private void TurnOn()
    {
        if (typeBooster != TypeBooster.Find)
        {
            boosterManager.IsOn = isUse;
            imgFrameUse.gameObject.SetActive(isUse);
           // iconBoosterEnd.gameObject.SetActive(!isUse);
        }
        if (isUse)
            iconBooster.transform.DOLocalRotate(new Vector3(0, 0, 30), 0.5f).From(Vector3.zero).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        else
        {
            iconBooster.transform.DOKill();
            iconBooster.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
    public void EventPointerDown()
    {
        gameplayUIManager.zoomInZoomOut.isTouch = false;
        gameplayUIManager.zoomInZoomOut.isPaintColor = false;
    }
    public void EventPointerUp()
    {
        gameplayUIManager.zoomInZoomOut.isTouch = true;
    }
}
