using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PanelChooseFrameBG : PanelBase
{

    [TabGroup("UI")][SerializeField] private GameObject objFrameBG;
    [TabGroup("UI")][SerializeField] private GameObject objDone;

    [Space]
    [TabGroup("UI")][SerializeField] private Button btnFrame;
    [TabGroup("UI")][SerializeField] private Button btnBG;
    [TabGroup("UI")][SerializeField] private Button btnback;
    [TabGroup("UI")][SerializeField] private Text txtSaved;

    [Space][TabGroup("BtnEffect")][SerializeField] private Image imgFrameActive;
    [TabGroup("BtnEffect")][SerializeField] private Image icon;
    [TabGroup("BtnEffect")][SerializeField] private Image iconAds;

    [Space][TabGroup("BtnEffect")][SerializeField] private Sprite sprIconOff;
    [TabGroup("BtnEffect")][SerializeField] private Sprite sprIconOn;

    [Space][SerializeField] private Sprite sprSelect;
    [SerializeField] private Sprite sprNoSelect;

    [Space]
    [SerializeField] private GameObject scrollFrame;
    [SerializeField] private GameObject scrollBG;

    [Space]
    [SerializeField] private Transform parentBG;
    [SerializeField] private ElementChooseFrameBG eleBG;
    [SerializeField] private List<ElementChooseFrameBG> listEleBG = new List<ElementChooseFrameBG>();

    [Space][SerializeField] private Transform parentFrame;
    [SerializeField] private ElementChooseFrameBG eleFrame;
    [SerializeField] private List<ElementChooseFrameBG> listEleFrame = new List<ElementChooseFrameBG>();

    [Space][SerializeField] public Transform parentLevel;
    [SerializeField] private ElementLevel eleLevel;
    public List<ElementLevel> listLevelContinue = new List<ElementLevel>();

    public ElementChooseFrameBG eleFrameCurrent;
    public ElementChooseFrameBG eleBGCurrent;
    public int idFrameChoose = -1;

    private ShapeInfo shapeInfo;
    private bool isClick = false;

    public override void Show()
    {
        base.Show();

        gameplayUIManager.objTab.SetActive(false);
        isClick = false;

        if (gameplayController.typePlayMode == TypePlayMode.Normal)
        {
            shapeInfo = gameplayUIManager.levelLoader.shapeInfo;
            iconAds.gameObject.SetActive(!shapeInfo.IsUnlockEffect);
            ConfigBtnEffect(shapeInfo.IsOnEffect);
        }
        else
        {
            ConfigBtnEffect(gameplayUIManager.levelLoader.textureMetadata.isOnEffect);
        }

        gameplayUIManager.SetConfetti();
        OnClickDone();
    }
    public void LoadData()
    {
        foreach (var frame in DataFrameBG.GetListBG())
        {
            var el = Instantiate(eleBG, parentBG);
            el.LoadElement(frame, TypeElement.BG);
            listEleBG.Add(el);
        }
        foreach (var frame in DataFrameBG.GetListFrame())
        {
            var el = Instantiate(eleFrame, parentFrame);
            el.LoadElement(frame, TypeElement.Frame);
            listEleFrame.Add(el);
        }

    }
    public void InitData()
    {
        var shape = gameplayUIManager.levelLoader.shapeInfo;

        foreach (var frame in listEleBG)
            frame.InitData(shape);
        foreach (var frame in listEleFrame)
            frame.InitData(shape);
    }

    public void OnClickDone()
    {
//        SoundClickButton();

        ActionHelper.ShowCMP();
        objFrameBG.SetActive(false);
        objDone.SetActive(true);

        ActionHelper.Clear(parentLevel);
        listLevelContinue.Clear();

        int count = 0;

        var listShapeNotDone = DataAllShape.GetListShapeNotDone();

        if (listShapeNotDone.Count <= 10)
            count = listShapeNotDone.Count;
        else
            count = 10;

        var list = new List<ShapeInfo>();


        while (count > 0)
        {
            int rd = Random.Range(0, listShapeNotDone.Count);
            // int rd = count - 1;// Random.Range(0, list.Count);

            list.Add(listShapeNotDone[rd]);
            listShapeNotDone.RemoveAt(rd);
            count--;
        }

        foreach (var element in list)
        {
            var el = Instantiate(eleLevel, parentLevel);
            el.LoadData(element);
            el.SetScaleInContinue();
            listLevelContinue.Add(el);
        }
    }


    public void OnClickFrame()
    {
        SoundClickButton();
        OnOffFrameBG(TypeElement.Frame);
    }
    public void OnClickBG()
    {
        SoundClickButton();
        OnOffFrameBG(TypeElement.BG);
    }
    private void OnOffFrameBG(TypeElement type)
    {
        scrollFrame.SetActive(false);
        scrollBG.SetActive(false);

        switch (type)
        {
            case TypeElement.Frame:
                scrollFrame.SetActive(true);
                btnFrame.image.sprite = sprSelect;
                btnBG.image.sprite = sprNoSelect;
                break;
            case TypeElement.BG:
                scrollBG.SetActive(true);
                btnFrame.image.sprite = sprNoSelect;
                btnBG.image.sprite = sprSelect;
                break;
        }
    }
    public void OnClickReset()
    {
        SoundClickButton();
        gameplayUIManager.levelLoader.VisualDoneColor();
    }


    public void OnClickDownload()
    {
        SoundClickButton();

        var pos = DataFrameBG.GetFrameBGInfo(TypeElement.Frame, idFrameChoose).localPos;
        gameplayUIManager.screenshotHandler.camShot.transform.position = new Vector3(pos.x, pos.y, -10);
        gameplayUIManager.screenshotHandler.camShot.orthographicSize = DataFrameBG.GetFrameBGInfo(TypeElement.Frame, idFrameChoose).sizeCamShot;
        Debug.Log("id Frame = " + idFrameChoose);
        Debug.Log("pos cam = " + gameplayUIManager.screenshotHandler.camShot.transform.position);
        Debug.Log("size cam = " + DataFrameBG.GetFrameBGInfo(TypeElement.Frame, idFrameChoose).sizeCamShot);

        gameplayUIManager.screenshotHandler.TakeScreenshot(() =>
        {
            txtSaved.gameObject.SetActive(true);
        });
    }
    public void OnClickBack()
    {
        gameplayUIManager.OnClickBack();
        Hide();
    }


    public void OnClickShowEffect()
    {
        if (isClick) return;
        isClick = true;

        if (gameplayController.typePlayMode == TypePlayMode.Normal)
        {
            if (!shapeInfo.IsUnlockEffect)
                ActionHelper.ShowRewardAds("Rw_UnlockEffect_" + shapeInfo.nameShape, OnEffect);
            else
                OnEffect(true);
        }
        else
        {
            ConfigUiEffect(ref gameplayUIManager.levelLoader.textureMetadata.isOnEffect);
            DataDIY.SetIsOnEffect(gameplayUIManager.levelLoader.textureMetadata.isOnEffect);
        }
    }
    private void ConfigUiEffect(ref bool isOnEffect)
    {
        isOnEffect = !isOnEffect;
        ConfigBtnEffect(isOnEffect);
    }
    private void OnEffect(bool isComplere)
    {
        if (!isComplere) return;

        iconAds.gameObject.SetActive(false);
        shapeInfo.IsUnlockEffect = true;
        shapeInfo.IsOnEffect = !shapeInfo.IsOnEffect;
        var check = shapeInfo.IsOnEffect;

        ConfigBtnEffect(check);
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

    public void ActiveLevelContinue()
    {
        foreach (var shape in listLevelContinue)
        {
            var check = shape.transform.position.x > 50 || shape.transform.position.x < -50;
            shape.transform.GetChild(0).gameObject.SetActive(!check);
        }
    }
}
