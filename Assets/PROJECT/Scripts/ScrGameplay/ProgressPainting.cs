using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

public class ProgressPainting : PanelBase
{
    [SerializeField] private Image imgProgress;
    [SerializeField] private Image imgGift1;
    [SerializeField] private Image imgGift2;
    [SerializeField] private Image imgGift3;
    [Space]

    [SerializeField] private Sprite sprGift1_On;
    [SerializeField] private Sprite sprGift2_On;
    [SerializeField] private Sprite sprGift3_On;

    [Space]

    [SerializeField] private Sprite sprGift1_Off;
    [SerializeField] private Sprite sprGift2_Off;
    [SerializeField] private Sprite sprGift3_Off;

    [Space]
    [SerializeField] private RectTransform rectEffect;
    [SerializeField] private ParticleSystem psSpawn;

    private ShapeInfo shapeInfo;
    private TextureMetadata textureMetadata;
    private bool isGetting = false;
    private bool isGiftContinue = false;
    private float valueSlider = 0;
    private int indexContinue = -1;
    public void Init(float val, ShapeInfo shapeInfo)
    {
        this.shapeInfo = shapeInfo;
        Init(val, shapeInfo.CountCompleteGift);
    }
    public void Init(float val, TextureMetadata textureMetadata)
    {
        this.textureMetadata = textureMetadata;
        Init(val, textureMetadata.countGift);
    }
    private void Init(float val, int CountCompleteGift)
    {
        imgProgress.fillAmount = val;

        if (val < 0.33f)
        {
            if (CountCompleteGift == 0)
                imgGift1.sprite = sprGift1_Off;
            else
                imgGift1.sprite = sprGift1_On;

            imgGift2.sprite = sprGift2_Off;
            imgGift3.sprite = sprGift3_Off;
        }
        else
        if (val < 0.66f)
        {
            imgGift1.sprite = sprGift1_On;

            if (CountCompleteGift == 1)
                imgGift2.sprite = sprGift2_Off;
            else
                imgGift2.sprite = sprGift2_On;

            imgGift3.sprite = sprGift3_Off;
        }
        else
        if (val < 1)
        {
            imgGift1.sprite = sprGift1_On;
            imgGift2.sprite = sprGift2_On;
            imgGift3.sprite = sprGift3_Off;
        }
        else
        {
            imgGift1.sprite = sprGift1_On;
            imgGift2.sprite = sprGift2_On;
            imgGift3.sprite = sprGift3_On;
        }
    }


    public void SetProgress(float val, ShapeInfo shapeInfo)
    {
        this.shapeInfo = shapeInfo;
        SetProgress(val, shapeInfo.CountCompleteGift);
    }
    public void SetProgress(float val, TextureMetadata textureMetadata)
    {
        this.textureMetadata = textureMetadata;
        SetProgress(val, textureMetadata.countGift);
    }
    private void SetProgress(float val, int CountCompleteGift)
    {
        imgProgress.fillAmount = val;
        rectEffect.anchoredPosition = new Vector2(val * imgProgress.rectTransform.sizeDelta.x, 0);

        var ps = Instantiate(psSpawn, rectEffect.transform);
        ps.Play();
        Destroy(ps, 1f);

        if (val >= 1 && CountCompleteGift == 2)
        {
            // show visual
            OpenBox(3);
        }
        else
        if (val >= 0.66f && CountCompleteGift == 1)
        {
            // show visual
            if (isGetting)
            {
                isGiftContinue = true;
                valueSlider = val;
                indexContinue = CountCompleteGift;
                return;
            }
            OpenBox(2);
        }
        else
        if (val >= 0.33f && CountCompleteGift == 0)
        {
            // show visual
            isGetting = true;
            OpenBox(1);
        }
        else
        {
            if (val >= 0.28f && val < 0.33f && CountCompleteGift == 0)
                imgGift1.sprite = sprGift1_Off;
            else if (val >= 0.6f && val < 0.66f && CountCompleteGift == 1)
                imgGift2.sprite = sprGift2_Off;
            else if (val >= 0.934f && val < 1 && CountCompleteGift == 2)
                imgGift3.sprite = sprGift3_Off;
        }
    }
    private void OpenBox(int idBox)
    {
        gameplayUIManager.zoomInZoomOut.enabled = false;
        gameplayUIManager.btnBack.interactable = false;
        if (gameplayController.typePlayMode == TypePlayMode.Normal)
            shapeInfo.CountCompleteGift = idBox;
        else
            DataDIY.SetCountGift(VariableSystem.LevelDIY, idBox);

        VisualOpenBox(idBox);
    }

    private void VisualOpenBox(int idBox)
    {
        // show panel box gift

        var listSpr = new List<Sprite>();
        var listVal = new List<int>();
        var listType = new List<TypeBooster>();

        var booster = gameplayUIManager.boosterManager;
        switch (idBox)
        {
            case 1:
                listSpr.Add(booster.sprFind);
                listSpr.Add(booster.sprFillByBom);

                listVal.Add(1);
                listVal.Add(1);

                listType.Add(TypeBooster.Find);
                listType.Add(TypeBooster.Bomb);

                VariableSystem.FindBooster++;
                VariableSystem.FillByBomBooster++;
                break;
            case 2:
                listSpr.Add(booster.sprFind);
                listSpr.Add(booster.sprFillByBom);

                listVal.Add(1);
                listVal.Add(1);

                listType.Add(TypeBooster.Find);
                listType.Add(TypeBooster.Bomb);

                VariableSystem.FindBooster++;
                VariableSystem.FillByBomBooster++;
                break;
            case 3:
                listSpr.Add(booster.sprFillByNum);
                listVal.Add(2);

                listType.Add(TypeBooster.Number);
                listType.Add(TypeBooster.Number);

                VariableSystem.FillByNumBooster += 2;
                StartCoroutine(gameplayUIManager.zoomInZoomOut.IE_AutoZoomCam(Vector2.zero, 35, () =>
                {
                    gameplayUIManager.popupCompleteLevel.ShowPopup(listSpr, listVal, listType, () =>
                    {
                        gameplayUIManager.btnBack.interactable = true;
                    });
                }));
                return;
        }

        canvasAllScene.popupGetGift.ShowPopup(listSpr, listVal, listType, () =>
        {
            gameplayUIManager.zoomInZoomOut.enabled = true;
            gameplayUIManager.btnBack.interactable = true;
            isGetting = false;
            if (isGiftContinue)
            {
                isGiftContinue = false;
                SetProgress(valueSlider, indexContinue);
            }
        }, true);


        ActionHelper.SetVibration(150);
    }
    public bool CheckDoneGift_2()
    {
        return imgProgress.fillAmount >= 0.5f;
    }
}
