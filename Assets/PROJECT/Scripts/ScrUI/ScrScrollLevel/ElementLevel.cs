using AssetKits.ParticleImage;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ElementLevel : PanelBase
{
    public int idShape;
    public int indexSibling;
    [SerializeField] private RawImage rawImage;
    [SerializeField] private ParticleImage objEffect;
    [SerializeField] private Image imgAds;

    [SerializeField] private Image imgFrame;
    [SerializeField] private Image imgBG;
    [SerializeField] private Sprite sprFrameDefault;

    [ShowInInspector] public Dictionary<int, DataElementShape> dicListDataShape = new Dictionary<int, DataElementShape>();
    [ShowInInspector] public DataSave dataSave;

    public ShapeInfo shapeInfo;
    public bool isLoaded = false;

    private bool isNew = false;
    private Texture2D texture;
    private TextureMetadata textureMetadata;
    private TypePlayMode playMode;
    public void SetScale()
    {
        shapeInfo.scaleInHome = rawImage.transform.localScale.x;
    }
    public void LoadData(ShapeInfo shapeInfo)
    {
        if(shapeInfo.indexShape == 34)
        {
            Debug.Log("name Shape = " + shapeInfo.nameShape);
            Debug.Log("Id Shape = " + shapeInfo.indexShape);
            Debug.Log(" shape.listColor.Count = " + shapeInfo.listColor.Count);
        }
        this.playMode = TypePlayMode.Normal;
        isLoaded = true;
        this.shapeInfo = shapeInfo;
        idShape = shapeInfo.indexShape;

        if (shapeInfo.scaleInHome != 0)
            rawImage.transform.localScale = Vector3.one * shapeInfo.scaleInHome;

        InitData();
    }
    public void LoadData(TextureMetadata textureMetadata, Texture2D texture2D)
    {
        playMode = TypePlayMode.DIY;
        rawImage.texture = texture2D;
        texture = texture2D;
        dataSave = textureMetadata.dataSave;
        this.textureMetadata = textureMetadata;
        idShape = textureMetadata.textureID;
        rawImage.SetNativeSize();
      //  rawImage.transform.localRotation = Quaternion.Euler(0, 0, textureMetadata.typeAlbum == TypeAlbum.Create ? -90 : 0);
        rawImage.transform.localScale = Vector3.one * 3;
        rawImage.rectTransform.sizeDelta = Vector2.one * rawImage.texture.width * 1.75f;
        Debug.Log("size delta = " + rawImage.rectTransform.sizeDelta);
        Debug.Log("width = " + rawImage.texture.width);
        ConfigFrameBG(textureMetadata.idFrame, textureMetadata.idBackground);

        int count = 0;

        if (dataSave.listDataCoor.Count == 0)
        {
            isNew = true;
            dataSave = new DataSave();
            var listDataCoor = new List<DataCoordinates>();
            for (int y = 0; y < texture.height; y++)
            {
                var data = new DataCoordinates();

                for (int x = 0; x < texture.width; x++)
                {
                    var cor = new Coordinates();
                    //   cor.num = -1;
                    cor.isHasColor = false;

                    var num = textureMetadata.listMap[count];
                    if (num != -1)
                    {
                        //       cor.num = num;
                        //      cor.color = textureMetadata.listColor[num];
                    }

                    count++;


                    data.listCoor.Add(cor);
                }
                listDataCoor.Add(data);
            }
            dataSave.listDataCoor = listDataCoor;
        }

        ConfigEffect(textureMetadata.isOnEffect);
    }
    public void InitData()
    {
        ConfigFrameBG(shapeInfo.IDFrame, shapeInfo.IDBackground);
        LoadTexture();
    }
    private void ConfigFrameBG(int idFrame, int idBG)
    {
        var frameBGInfo = DataFrameBG.GetFrameBGInfo(TypeElement.Frame, idFrame);
        imgFrame.rectTransform.anchoredPosition = frameBGInfo.recPos;

        if (idFrame != -1)
        {
            imgFrame.sprite = frameBGInfo.spr;
            imgFrame.transform.localScale = Vector2.one * 0.6f;
        }
        else
        {
            imgFrame.transform.localScale = Vector2.one;

            if (idBG != -1)
            {
                imgFrame.transform.SetSiblingIndex(0);
            }
            else
            {
                imgFrame.transform.SetSiblingIndex(1);
                imgFrame.sprite = sprFrameDefault;
            }
        }

        imgFrame.SetNativeSize();

        if (idBG == -1)
        {
            imgBG.gameObject.SetActive(false);
            imgBG.transform.localScale = Vector2.one;
        }
        else
        {
            imgBG.gameObject.SetActive(true);
            imgBG.sprite = DataFrameBG.GetFrameBGInfo(TypeElement.BG, idBG).spr;
            imgBG.transform.localScale = Vector2.one * 0.62f;
        }
        imgBG.SetNativeSize();
    }

    private void LoadTexture()
    {
        texture = new Texture2D(shapeInfo.width, shapeInfo.height);

        int count = 0;
        dataSave = shapeInfo.DataSave;

        if (dataSave == null)
        {
            isNew = true;
            dataSave = new DataSave();
            var listDataCoor = new List<DataCoordinates>();
            for (int y = 0; y < texture.height; y++)
            {
                var data = new DataCoordinates();

                for (int x = 0; x < texture.width; x++)
                {
                    var cor = new Coordinates();
                    //  cor.num = -1;
                    cor.isHasColor = false;

                    var num = shapeInfo.listMap[count];
                    if (num != -1)
                    {
                        //    cor.num = num;
                        //   cor.color = shapeInfo.listColor[num];
                    }

                    count++;


                    data.listCoor.Add(cor);
                }
                listDataCoor.Add(data);
            }
            dataSave.listDataCoor = listDataCoor;
        }

        if (shapeInfo.DataTexture.Equals("null"))
        {
            var s = ActionHelper.TextureToString(shapeInfo.textureGray);
            texture = ActionHelper.StringToTexture(s);
        }
        else
            texture = ActionHelper.StringToTexture(shapeInfo.DataTexture);

        rawImage.texture = texture;
        rawImage.rectTransform.sizeDelta = new Vector2(shapeInfo.width, shapeInfo.height);
        rawImage.SetNativeSize();
        rawImage.texture.filterMode = FilterMode.Point;

        ConfigEffect(shapeInfo.IsOnEffect);
        ConfigLevelDailyQuest();
    }
    private void ConfigEffect(bool isOn)
    {
        objEffect.gameObject.SetActive(isOn);
        if (shapeInfo.IsOnEffect)
        {
            objEffect.rectWidth = rawImage.rectTransform.sizeDelta.x;
            objEffect.rectHeight = rawImage.rectTransform.sizeDelta.y;
        }
    }
    public void ConfigLevelDailyQuest()
    {
        if (shapeInfo.typeUnlock == TypeUnlock.DailyQuest && !shapeInfo.IsUnlock)
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);
    }
    public void OnClickChoose()
    {
        if (playMode == TypePlayMode.DIY)
        {
            StartCoroutine(IE_LoadLevel());
        }
        else
        {

            if (shapeInfo.typeUnlock == TypeUnlock.DailyQuest)
            {
                gameObject.SetActive(true);
                shapeInfo.IsUnlock = true;
            }
            ActionHelper.CheckShowInter((bool isShowCompleted) =>
            {
                LoadLV();
            });
        }
    }
    private void LoadLV(bool isComplete = true)
    {
        if (!isComplete) return;
        shapeInfo.IsUnlock = true;
        StartCoroutine(IE_LoadLevel());
    }
    private IEnumerator IE_LoadLevel()
    {
        //canvasAllScene.panelLoading.LoadingProgressFake();
        yield return new WaitForEndOfFrame();

        var gameplay = gameplayController;

        if (isNew)
        {
            if (playMode == TypePlayMode.Normal)
            {
                isNew = false;
                shapeInfo.DataSave = dataSave;
                shapeInfo.DataTexture = ActionHelper.TextureToString(texture);

            }
            else
            {
                textureMetadata.dataSave = dataSave;
            }
        }

        yield return new WaitForEndOfFrame();

        if (playMode == TypePlayMode.Normal)
        {
            gameplay.typePlayMode = TypePlayMode.Normal;
            gameplay.LoadLevel(shapeInfo, texture);
        }
        else
        {
            gameplay.typePlayMode = TypePlayMode.DIY;
            VariableSystem.LevelDIY = idShape;
            DataDIY.SetMetaDataCurrent(idShape);
            gameplay.LoadLevel(textureMetadata, texture);
        }
    }

    public void SetScaleInContinue()
    {
        if (shapeInfo.scaleInHome != 0)
            rawImage.transform.localScale = Vector3.one * shapeInfo.scaleInHome * 3 / 4;
    }
}
