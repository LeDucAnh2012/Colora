using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PanelChooseColor : PanelBase
{
    [SerializeField] private ElementNumColor eleSpawn;
    [SerializeField] private Transform parentSpawn;
    [SerializeField] private List<ElementNumColor> listEleNumColor = new List<ElementNumColor>();

 [HideInInspector]   public ElementNumColor eleCurrent;

    [SerializeField] private Image imgFrameActive;
    [SerializeField] private Image icon;
    [SerializeField] private Image iconAds;

    [Space][SerializeField] private Sprite sprIconOff;
    [SerializeField] private Sprite sprIconOn;

    private ShapeInfo shapeInfo;
    private TextureMetadata textureMetadata;
    private void OnEnable()
    {
        isClick = false;
    }
 
    public void SpawnElementNumColor(ShapeInfo shape)
    {

        shapeInfo = shape;
        for (int i = 0; i < shape.listColor.Count; i++)
        {
            var e = Instantiate(eleSpawn, parentSpawn);
            e.LoadData(i, shape.listColor[i]);
            listEleNumColor.Add(e);
            if (i == 0)
            {
                eleCurrent = e;
                e.ChooseColor(true, false);
            }
        }

        iconAds.gameObject.SetActive(!shapeInfo.IsUnlockEffect);
        ConfigBtnEffect(shapeInfo.IsOnEffect);
        parentSpawn.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }
    public void SpawnElementNumColor(TextureMetadata textureMetadata)
    {
        //  shapeInfo = shape;
        this.textureMetadata = textureMetadata;
        for (int i = 0; i < textureMetadata.listColor.Count; i++)
        {
            var e = Instantiate(eleSpawn, parentSpawn);
            e.LoadData(i, textureMetadata.listColor[i]);
            listEleNumColor.Add(e);
            if (i == 0)
            {
                eleCurrent = e;
                e.ChooseColor(true, false);
            }
        }
        ConfigBtnEffect(textureMetadata.isOnEffect);
        parentSpawn.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }
    public void OffButtonNumColor(int num, bool isInit)
    {
        if (num >= listEleNumColor.Count) return;

        var el = listEleNumColor[num];

        if (!isInit)
            el.SetDoneNumColor(CheckDoneNumColor);
        else
        {
            el.gameObject.SetActive(false);
            CheckDoneNumColor(el);
        }


    }
    private void CheckDoneNumColor(bool isInit)
    {
        for (int i = 0; i < listEleNumColor.Count; i++)
        {
            if (listEleNumColor[i].gameObject.activeSelf)
            {
                listEleNumColor[i].ChooseColor(isInit, false);
                return;
            }
        }

        if (!isInit)
        {
            Debug.Log("CheckDoneNumColor");
            InitDataGame.instance.listDataDailyQuest[0].CountFinishQuest++;

            if (gameplayController.typePlayMode == TypePlayMode.Normal)
            {
                if (!initDataGame.listDataDailyQuest[3].listTypeTopic.Contains(shapeInfo.typeTopic))
                {
                    initDataGame.listDataDailyQuest[3].CountFinishQuest++;
                    initDataGame.listDataDailyQuest[3].SetTypeTopic(shapeInfo.typeTopic);
                }
                if ((TypeTopic)initDataGame.listDataDailyQuest[4].AmountQuest_2 == shapeInfo.typeTopic)
                {
                    initDataGame.listDataDailyQuest[4].CountFinishQuest++;
                }
            }
            GameplayUIManager.instance.ShowWin();
        }
    }
    public void SetProgressNumColor(int num, int value, int maxValue)
    {
        if (num >= listEleNumColor.Count) return;
        listEleNumColor[num].SetProgressNumColor(value, maxValue);
    }
    private bool isClick = false;
    public void OnClickShowEffect()
    {
        if (isClick) return;
        isClick = true;

        if (gameplayController.typePlayMode == TypePlayMode.Normal)
        {
            if (!shapeInfo.IsUnlockEffect)
                ActionHelper.ShowRewardAds(KeyLogFirebase.Colora_RW_ShowPicEffect_211224,"Rw_UnlockEffect_" + shapeInfo.nameShape, OnEffect);
            else
                OnEffect(true);
        }
        else
        {
            ConfigUiEffect(ref textureMetadata.isOnEffect);
            DataDIY.SetIsOnEffect(textureMetadata.isOnEffect);
        }
    }
    private void OnEffect(bool isComplere)
    {
        if (!isComplere) return;

        iconAds.gameObject.SetActive(false);
        ActionHelper.LogEvent(KeyLogFirebase.OnOffEffect + (shapeInfo.IsOnEffect ? "On" : "Off") + "_" + shapeInfo.nameShape);
        shapeInfo.IsUnlockEffect = true;

        shapeInfo.IsOnEffect = !shapeInfo.IsOnEffect;
        ConfigBtnEffect(shapeInfo.IsOnEffect);
    }
    private void ConfigUiEffect(ref bool isOnEffect)
    {
        isOnEffect = !isOnEffect;
        ConfigBtnEffect(isOnEffect);
    }
    private void ConfigBtnEffect(bool isOnEffect)
    {
        var check = isOnEffect;

        imgFrameActive.gameObject.SetActive(check);
        icon.sprite = check ? sprIconOn : sprIconOff;
        icon.rectTransform.DOAnchorPosX(icon.rectTransform.anchoredPosition.x * -1, 0.5f).SetEase(Ease.OutSine).OnComplete(() =>
        {
            isClick = false;
        });
        gameplayUIManager.levelLoader.ShowEffect(isOnEffect);
    }
}
