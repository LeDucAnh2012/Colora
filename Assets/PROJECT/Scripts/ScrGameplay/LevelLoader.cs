using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using VTLTools;
using UnityEngine.Events;
using Unity.VisualScripting;
using System;
using UnityEditor;
using UnityEngine.UIElements;

public class DataElementShape
{
    public int id;
    public List<ElementShape> listShape = new List<ElementShape>();
    public Color colorShape;
    public int countShapeHasColor;
    public int CountTotalShape => listShape.Count;

    public DataElementShape() { }
    public DataElementShape(int id, ElementShape e)
    {
        this.id = id;
        listShape.Add(e);
    }
}

public class LevelLoader : MonoBehaviour
{
    public int numCurrent = 1;

    [SerializeField] private GameplayUIManager gameplayUIManager;

    [Space]
    [SerializeField] private ParticleSystem psPaintColor;
    [SerializeField] private ParticleSystem psMask;
    [SerializeField] private SpriteMask sprMaskEffect;

    [Space]
    [SerializeField] private Transform parentSpawn;
    [SerializeField] private ElementShape elementShapeSpawn;
    [SerializeField] private List<ElementShape> listEleShape = new List<ElementShape>();

    [Space]
    [SerializeField] private float time = 2;
    [SerializeField] private Vector2 blockSize = new Vector2(1f, 1f); // Kích thước của mỗi khối
    [SerializeField] private Vector2 spacing = new Vector2(-0.65f, -0.65f); // Khoảng cách giữa các khối

    [HideInInspector] public Dictionary<int, DataElementShape> dicListDataShape = new Dictionary<int, DataElementShape>();
    [HideInInspector] public List<List<ElementShape>> listDataElement = new List<List<ElementShape>>();

    [HideInInspector] public DataSave dataSave;
    [HideInInspector] public ShapeInfo shapeInfo;
    [HideInInspector] public DataPaintingProgress dataPainting;
    [HideInInspector] public TextureMetadata textureMetadata;

    [HideInInspector] public float posTop = 0f;
    [HideInInspector] public float posBottom = 0f;
    [HideInInspector] public float posRight = 0f;
    [HideInInspector] public float posLeft = 0f;

    private long timeVibration = 150;
    private int speed = 20;

    private int totalShape = 0;
    private int countShapeHasColor = 0;

    private int width = 0;
    private int height = 0;
    private bool isVibration = true;

    private Texture2D texture;
    private void Init()
    {
        if (RemoteConfig.instance.allConfigData.IsDebug)
            timeVibration = RemoteConfig.instance.allConfigData.TimeVibration;

        numCurrent = 1;
        if (VariableSystem.IndexShowAds < RemoteConfig.instance.allConfigData.IndexShowInter && !VariableSystem.IsCanShowInter)
        {
            VariableSystem.IndexShowAds++;
            if (VariableSystem.IndexShowAds >= RemoteConfig.instance.allConfigData.IndexShowInter)
                VariableSystem.IsCanShowInter = true;
        }
    }
    [Button]
    void CheckColor()
    {
        if (GameplayController.instance.typePlayMode == TypePlayMode.Normal)
        {

            for (int i = 0; i < textureMetadata.listColor.Count; i++)
            {
                if (textureMetadata.listColor[i] == textureMetadata.listNoColor[i])
                {
                    Debug.Log("same = " + i);
                }
            }
        }
        else
        {
            for (int i = 0; i < shapeInfo.listColor.Count; i++)
            {
                if (shapeInfo.listColor[i] == shapeInfo.listNoColor[i])
                {
                    Debug.Log("same = " + i);
                }
            }
        }
    }
    public void SetScaleParent()
    {

    }
    public void LoadLevel(TextureMetadata textureMetadata, Texture2D texture2D)
    {
        Init();



        //    ShowEffect(shape.IsOnEffect && shape.IsUnlockEffect);

        this.texture = texture2D;
        this.textureMetadata = textureMetadata;
        dataSave = textureMetadata.dataSave;
        sprMaskEffect.sprite = ActionHelper.Texture2DToSprite(texture2D);
        dataPainting = textureMetadata.dataPaintingProgress;

        width = texture2D.width;// collum
        height = texture2D.height; // row

        arrIsHasColor = new bool[height, width];
        Debug.Log("width = " + width);
        Debug.Log("height = " + height);
        var count = 0;
        dataPainting = textureMetadata.dataPaintingProgress;
        if (dataPainting == null)
            dataPainting = new DataPaintingProgress();
      //  parentSpawn.transform.localRotation = Quaternion.Euler(0, 0, textureMetadata.typeAlbum == TypeAlbum.Create ? -90 : 0);
        float pixelSize = blockSize.x;
        float spacing = this.spacing.x;

        // Tính toán tổng kích thước của hình ảnh
        float totalWidth = width * (pixelSize + spacing);
        float totalHeight = height * (pixelSize + spacing);

        // Tính toán vị trí trung tâm
        float centerX = totalWidth / 2f - (pixelSize + spacing) / 2f;
        float centerY = totalHeight / 2f - (pixelSize + spacing) / 2f;
        var type = GameplayController.instance.typePlayMode;
        var textureColor = DataDIY.LoadTextureByID(textureMetadata.textureID, true);

        Color[] arrColor = textureColor.GetPixels();
        Color[] arrNoColor = texture2D.GetPixels();

        for (int i = 0; i < height; i++)
        {
            var listEle = new List<ElementShape>();

            for (int j = 0; j < width; j++)
            {
                var num = textureMetadata.listMap[count];
                var numNocolor = textureMetadata.listMapNoColor[count];
                if (num != -1)
                {
                    totalShape++;
                    var e = ObjectPool.Spawn(elementShapeSpawn, parentSpawn);
                    // var e = Instantiate(elementShapeSpawn, parentSpawn);
                    listEleShape.Add(e);
                    e.colorShape = textureMetadata.listColor[num];
                    e.colorGray = textureMetadata.listNoColor[numNocolor];

                    if (dicListDataShape.ContainsKey(num))
                    {
                        dicListDataShape[num].id = num;
                        dicListDataShape[num].listShape.Add(e);
                    }
                    else
                    {
                        dicListDataShape.Add(num, new DataElementShape(num, e));
                    }
                    float posX = (j * (pixelSize + spacing)) - centerX;
                    float posY = (i * (pixelSize + spacing)) - centerY;

                    var pos = new Vector3(posX, posY, 0);
                    e.SetPos(pos, blockSize);
                    pos = e.transform.position;

                    if (pos.x > posRight)
                        posRight = pos.x;

                    if (pos.x < posLeft)
                        posLeft = pos.x;

                    if (pos.y > posTop)
                        posTop = pos.y;

                    if (pos.y < posBottom)
                        posBottom = pos.y;

                    e.LoadShape(num, i, j, type, textureMetadata.typeAlbum);

                    if (textureMetadata.dataSave.listDataCoor[i].listCoor[j].isHasColor)
                    //     if (arrColor[count] == arrNoColor[count])
                    {
                        e.SetColor(e.colorShape, isDone: true, isSaveData: false, isInit: true);
                        //   arrIsHasColor[i, j] = true;
                    }
                    else
                        e.SetColorDefault(e.colorGray, Color.black, false);

                    listEle.Add(e);
                }
                else
                    listEle.Add(null);

                count++;

            }
            listDataElement.Add(listEle);
        }

        gameplayUIManager.LoadNumColor(textureMetadata);

        for (int i = 0; i < textureMetadata.listColor.Count; i++)
        {
            if (dicListDataShape[i].countShapeHasColor == dicListDataShape[i].CountTotalShape)
                gameplayUIManager.panelChooseColor.OffButtonNumColor(i, true);
            gameplayUIManager.panelChooseColor.SetProgressNumColor(i, dicListDataShape[i].countShapeHasColor, dicListDataShape[i].CountTotalShape);
        }
        shapeInfo = new ShapeInfo();
        shapeInfo.CountCompleteGift = textureMetadata.countGift;
        gameplayUIManager.progressPainting.Init((float)countShapeHasColor / totalShape, shapeInfo);

        if ((StateDone)textureMetadata.stateDone == StateDone.Done)
        {
            try
            {
                gameplayUIManager.ShapeDone();
            }
            catch
            {
                ReloadShape();
            }
            return;
        }
    }
    public void LoadLevel(ShapeInfo shape, Texture2D texture)
    {
        Init();
        Debug.Log("Id Shape = " + shape.indexShape);
        Debug.Log(" shape.listColor.Count = " + shape.listColor.Count);
        ShowEffect(shape.IsOnEffect && shape.IsUnlockEffect);

        this.texture = texture;
        shapeInfo = shape;

        sprMaskEffect.sprite = ActionHelper.Texture2DToSprite(shapeInfo.textureGray);
        dataSave = shapeInfo.DataSave;
        dataPainting = shapeInfo.DataPainting;

        width = shapeInfo.width;// collum
        height = shapeInfo.height; // row
        arrIsHasColor = new bool[height, width];
        var count = 0;

        if (dataPainting == null)
            dataPainting = new DataPaintingProgress();

        float pixelSize = blockSize.x;
        float spacing = this.spacing.x;

        // Tính toán tổng kích thước của hình ảnh
        float totalWidth = width * (pixelSize + spacing);
        float totalHeight = height * (pixelSize + spacing);

        // Tính toán vị trí trung tâm
        float centerX = totalWidth / 2f - (pixelSize + spacing) / 2f;
        float centerY = totalHeight / 2f - (pixelSize + spacing) / 2f;
        var type = GameplayController.instance.typePlayMode;

        for (int i = 0; i < height; i++)
        {
            var listEle = new List<ElementShape>();

            for (int j = 0; j < width; j++)
            {
                var num = shapeInfo.listMap[count];
                var numNocolor = shapeInfo.listMapNoColor[count];
                if (num != -1)
                {
                    totalShape++;
                    var e = ObjectPool.Spawn(elementShapeSpawn, parentSpawn);
                    // var e = Instantiate(elementShapeSpawn, parentSpawn);
                    listEleShape.Add(e);
                    e.colorShape = shapeInfo.listColor[num];
                    e.colorGray = shapeInfo.listNoColor[numNocolor];

                    if (dicListDataShape.ContainsKey(num))
                    {
                        dicListDataShape[num].id = num;
                        dicListDataShape[num].listShape.Add(e);
                    }
                    else
                    {
                        dicListDataShape.Add(num, new DataElementShape(num, e));
                    }
                    float posX = (j * (pixelSize + spacing)) - centerX;
                    float posY = (i * (pixelSize + spacing)) - centerY;

                    var pos = new Vector3(posX, posY, 0);
                    e.SetPos(pos, blockSize);
                    pos = e.transform.position;

                    if (pos.x > posRight)
                        posRight = pos.x;

                    if (pos.x < posLeft)
                        posLeft = pos.x;

                    if (pos.y > posTop)
                        posTop = pos.y;

                    if (pos.y < posBottom)
                        posBottom = pos.y;

                    e.LoadShape(num, i, j, type);

                    if (dataSave.listDataCoor[i].listCoor[j].isHasColor)
                        e.SetColor(e.colorShape, isDone: true, isSaveData: false, isInit: true);
                    else
                        e.SetColorDefault(e.colorGray, Color.black, false);

                    listEle.Add(e);
                }
                else
                    listEle.Add(null);

                count++;

            }
            listDataElement.Add(listEle);
        }

        gameplayUIManager.LoadNumColor(shapeInfo);

        for (int i = 0; i < shapeInfo.listColor.Count; i++)
        {
            if (dicListDataShape[i].countShapeHasColor == dicListDataShape[i].CountTotalShape)
                gameplayUIManager.panelChooseColor.OffButtonNumColor(i, true);
            gameplayUIManager.panelChooseColor.SetProgressNumColor(i, dicListDataShape[i].countShapeHasColor, dicListDataShape[i].CountTotalShape);
        }

        gameplayUIManager.progressPainting.Init((float)countShapeHasColor / totalShape, shapeInfo);

        Debug.Log("shape.StateDone = " + shape.StateDone);
        if (shape.StateDone == StateDone.Done)
        {
            try
            {
                Debug.Log("ShapeDone");
                gameplayUIManager.ShapeDone();
            }
            catch
            {
                ReloadShape();
            }
            return;
        }
    }
    private void ReloadShape()
    {
        gameplayUIManager.ReloadShape();
        shapeInfo.DeleteDataSave();

        texture = new Texture2D(shapeInfo.width, shapeInfo.height);

        int c = 0;
        dataSave = shapeInfo.DataSave;

        if (dataSave == null)
        {
            dataSave = new DataSave();
            var listDataCoor = new List<DataCoordinates>();
            for (int y = 0; y < texture.height; y++)
            {
                var data = new DataCoordinates();

                for (int x = 0; x < texture.width; x++)
                {
                    var cor = new Coordinates();
                    //     cor.num = -1;
                    cor.isHasColor = false;

                    var num = shapeInfo.listMap[c];
                    if (num != -1)
                    {
                        //      cor.num = num;
                        // cor.color = shapeInfo.listColor[num];
                    }

                    c++;


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

        shapeInfo.DataSave = dataSave;
        shapeInfo.DataTexture = ActionHelper.TextureToString(texture);

        LoadLevel(shapeInfo, texture);
    }
    public void SetPosScaleParentSpawn()
    {
        if (shapeInfo.scaleInHome != 0)
        {
            float valScale = shapeInfo.scaleInHome / 3.28571428571f;
            parentSpawn.localScale = Vector2.one * valScale;
            sprMaskEffect.transform.localScale = Vector2.one * 30 * valScale;
        }
        if(GameplayController.instance.typePlayMode == TypePlayMode.DIY)
        {
            parentSpawn.transform.localScale = Vector2.one * ( 2.9f - 0.035f * (texture.width - 40));
            sprMaskEffect.transform.localScale = Vector2.one * 85;
        }
    }

    #region Funct Booster

    public float speedFillByRow = 0.05f;
    public float timeFillByRow = 2;
    public Color colorFill;
    public Color colorGray;

    private IEnumerator IE_FILL_BY_ROW = null;

    public void FillByRow()
    {
        StopFillByRow();
        IE_FILL_BY_ROW = IE_FillByRow();
        StartCoroutine(IE_FILL_BY_ROW);
    }
    public void StopFillByRow()
    {
        if (IE_FILL_BY_ROW != null)
        {
            StopCoroutine(IE_FILL_BY_ROW);
            IE_FILL_BY_ROW = null;
        }
    }
    private IEnumerator IE_FillByRow()
    {
        float countTime = 0;
        foreach (var e in listEleShape)
        {
            if (!e.isDoneColor)
            {
                e.SetColor(e.colorShape, isDone: true, isSaveData: true, isInit: false);
                yield return new WaitForSeconds(ActionHelper.IsEditor() ? 0.01f : speedFillByRow);
                countTime += speedFillByRow;
            }
            if (countTime >= timeFillByRow)
                break;
        }
    }
    public void FillByNumber()
    {
        for (int i = 0; i < dicListDataShape[numCurrent].listShape.Count; i++)
        {
            var e = dicListDataShape[numCurrent].listShape[i];

            if (!e.isDoneColor)
                e.SetColor(colorFill, isDone: true, isSaveData: false, isInit: false);
        }
        VariableSystem.FillByNumBooster--;
        gameplayUIManager.boosterManager.fillByNum.UpdateUI();
        //  SaveData();
    }
    public void FillByNumber(int num)
    {
        for (int i = 0; i < dicListDataShape[num].listShape.Count; i++)
        {
            var e = dicListDataShape[num].listShape[i];

            if (!e.isDoneColor)
                e.SetColor(e.colorShape, isDone: true, isSaveData: false, isInit: false);
        }
        VariableSystem.FillByNumBooster--;
        InitDataGame.instance.listDataDailyQuest[2].CountFinishQuest++;
        gameplayUIManager.boosterManager.fillByNum.UpdateUI();
        //  SaveData();
    }
    public void FillByBomb(ElementShape e, bool isSaveData)
    {
        if (!e.isDoneColor)
            e.SetColor(e.colorShape, true, isSaveData, false);
    }
    public void FillAll()
    {
        foreach (var e in listEleShape)
        {
            if (!e.isDoneColor)
            {
                e.SetColor(e.colorShape, isDone: true, isSaveData: false, isInit: false);
            }
        }
        //    GameplayUIManager.instance.ShowWin();
    }
    public void Find()
    {
        var pos = Vector3.zero;

        foreach (var shape in dicListDataShape[numCurrent].listShape)
            if (!shape.isDoneColor)
            {
                pos = shape.transform.position;
                break;
            }
        pos.z = -10;
        GameplayUIManager.instance.zoomInZoomOut.AutoZoomCam(5, pos);
        VariableSystem.FindBooster--;
        InitDataGame.instance.listDataDailyQuest[2].CountFinishQuest++;
        gameplayUIManager.boosterManager.findBooster.UpdateUI();
    }
    #endregion
    public bool CheckDoneColor(int num)
    {
        foreach (var e in dicListDataShape.Keys)
            if (e == num)
            {
                foreach (var shape in dicListDataShape[e].listShape)
                    if (!shape.isDoneColor)
                        return true;
            }

        return false;
    }

    public void SetColorByNum(ElementShape ele, bool isSaveData, bool isSound = false)
    {
        if (ele.numColor == numCurrent)
            ele.SetColor(colorFill, isDone: true, isSaveData, isInit: false, isSound);
        //else
        //{
        //    var color = colorFill;
        //    color.a = 0.3f;

        //    ele.SetColor(color, false, false,false);
        //}
    }
    public void SetNumColorNew(int num, bool isInit)
    {
        foreach (var e in dicListDataShape[numCurrent].listShape)
            e.SetColorDefault(e.colorLerp);

        numCurrent = num;
        foreach (var e in dicListDataShape[numCurrent].listShape)
            e.SetColor(colorGray, isDone: false, isSaveData: false, isInit);
    }
    public void SetColorLerp(int num)
    {
        foreach (var e in dicListDataShape[numCurrent].listShape)
            e.SetColorDefault(e.colorLerp);
    }


    public void SpawnEffectPaintColor(Vector2 pos, Color color)
    {
        var ps = Instantiate(psPaintColor, pos, Quaternion.identity);
        ps.startColor = color;
        ps.Play();
        Destroy(ps, 1f);
    }
    public void SetDoneColor(int idDone, bool isSaveData, bool isInit, int row, int collum)
    {
        dicListDataShape[idDone].countShapeHasColor++;

        //  if (GameplayController.instance.typePlayMode == TypePlayMode.Normal)
        {
            //    dataSave.listDataCoor[row].listCoor[collum].num = idDone;
            dataSave.listDataCoor[row].listCoor[collum].isHasColor = true;
        }

        arrIsHasColor[row, collum] = true;

        if (!isInit)
        {
            // paint pixel
            InitDataGame.instance.listDataDailyQuest[1].CountFinishQuest++;

            dataPainting.listVecCoordinates.Add(new Vector2(row, collum));
            if (GameplayController.instance.typePlayMode == TypePlayMode.Normal)
            {
                if (shapeInfo.StateDone == StateDone.NotDone)
                {
                    shapeInfo.StateDone = StateDone.InProgress;
                    // log firebase
                    ActionHelper.LogEvent(KeyLogFirebase.PlayPic + shapeInfo.nameShape);
                }
            }
        }

        if (isSaveData)
        {
            //  SaveData();
        }


        countShapeHasColor++;
        if (!isInit)
        {
            if (GameplayController.instance.typePlayMode == TypePlayMode.Normal)
                gameplayUIManager.progressPainting.SetProgress((float)countShapeHasColor / totalShape, shapeInfo);
            else
                gameplayUIManager.progressPainting.SetProgress((float)countShapeHasColor / totalShape, textureMetadata);
        }

        if (CheckFillFullNumber(idDone))
        {
            if (!isInit)
                ActionHelper.SetVibration(150);
            gameplayUIManager.panelChooseColor.OffButtonNumColor(idDone, false);
        }

        gameplayUIManager.panelChooseColor.SetProgressNumColor(idDone, dicListDataShape[idDone].countShapeHasColor, dicListDataShape[idDone].CountTotalShape);
    }
    bool[,] arrIsHasColor = new bool[1, 2];
    public void SaveData()
    {
        if (GameplayController.instance.typePlayMode == TypePlayMode.DIY)
        {
            Color[] arrColor = DataDIY.LoadTextureByID(textureMetadata.textureID, true).GetPixels();
            int count = 0;
            for (int i = 0; i < texture.height; i++)
                for (int j = 0; j < texture.width; j++)
                {
                    // if (arrIsHasColor[i, j])
                    if (dataSave.listDataCoor[i].listCoor[j].isHasColor)
                        texture.SetPixel(j, i, arrColor[count]);
                    count++;
                }

            texture.Apply();

            DataDIY.SaveEditedTexture(texture, textureMetadata.textureID);
            DataDIY.SaveMetadata(textureMetadata.textureID, (StateDone)textureMetadata.stateDone, textureMetadata.countGift, dataPainting, dataSave);
        }
        else
        {
            shapeInfo.DataSave = dataSave;
            shapeInfo.DataPainting = dataPainting;
            Color[] arrColor = shapeInfo.textureDefault.GetPixels();
            int count = 0;
            if (shapeInfo.IDFrame != -1)
                ActionHelper.LogEvent(KeyLogFirebase.UseFrame + shapeInfo.IDFrame);

            if (shapeInfo.IDBackground != -1)
                ActionHelper.LogEvent(KeyLogFirebase.UseBackground + shapeInfo.IDBackground);

            for (int i = 0; i < texture.height; i++)
                for (int j = 0; j < texture.width; j++)
                {
                    if (dataSave.listDataCoor[i].listCoor[j].isHasColor)
                        texture.SetPixel(j, i, arrColor[count]);
                    count++;
                }
            texture.Apply();
            shapeInfo.DataTexture = ActionHelper.TextureToString(texture);


        }
    }

    private bool CheckFillFullNumber(int idDone)
    {
        return dicListDataShape[idDone].countShapeHasColor >= dicListDataShape[idDone].CountTotalShape;
    }
    public void ShowEffect(bool isOn)
    {
        psMask.gameObject.SetActive(isOn);
    }
    public void SetFrameBG()
    {
        var checkFrame = shapeInfo.IDFrame != -1;
        gameplayUIManager.sprRenderFrame.gameObject.SetActive(checkFrame);
        if (checkFrame)
        {
            var frameBGInfo = DataFrameBG.GetFrameBGInfo(TypeElement.Frame, shapeInfo.IDFrame);
            gameplayUIManager.sprRenderFrame.sprite = frameBGInfo.spr;
            gameplayUIManager.sprRenderFrame.transform.localPosition = frameBGInfo.localPos;

        }

        var checkBG = shapeInfo.IDBackground != -1;
        gameplayUIManager.sprRenderBG.gameObject.SetActive(checkBG);
        if (checkBG)
            gameplayUIManager.sprRenderBG.sprite = DataFrameBG.GetFrameBGInfo(TypeElement.BG, shapeInfo.IDBackground).spr;
    }

    public void VisualDoneColor(UnityAction callback = null)
    {
        foreach (var el in listEleShape)
            el.SetNone();
        var val = totalShape / 100;
        if (totalShape < 3000)
            val /= 2;

        //var val = (totalShape / (50 * time) - 20 / time);
        //var val = (totalShape / (totalShape/85.76f * time) - totalShape/ 214.4f / time);
        //if (val <= 1)
        //    speed = 1;
        //else
        //    speed = (int)val;
        speed = val;
        IE_SHOW_PANEL = ActionHelper.StartAction(() => { callback?.Invoke(); }, time);

        // gameplayUIManager.listIEnum.Add(IE_SHOW_PANEL);
        StartCoroutine(IE_SHOW_PANEL);

        for (int i = 0; i < speed; i++)
        {
            var IE = IE_Visual(i, speed);
            //   gameplayUIManager.listIEnum.Add(IE);
            StartCoroutine(IE);
        }
    }
    private IEnumerator IE_SHOW_PANEL = null;
    private IEnumerator IE_Visual(int indexBegin, int step)
    {


        for (int i = indexBegin; i < dataPainting.listVecCoordinates.Count; i += step)
        {
            var vec = dataPainting.listVecCoordinates[i];

            if ((int)vec.x >= listDataElement.Count)
            {
                StopAllCoroutines();
                ReloadShape();
                break;
            }

            if ((int)vec.y >= listDataElement[(int)vec.x].Count)
            {
                StopAllCoroutines();
                ReloadShape();
                break;
            }

            if (listDataElement[(int)vec.x][(int)vec.y] != null)
            {
                listDataElement[(int)vec.x][(int)vec.y].SetColorDone();

                yield return null;
            }
        }
    }
    public void SetVibration()
    {
        if (!isVibration) return;
        isVibration = false;
        ActionHelper.SetVibration(timeVibration);
        StartCoroutine(ActionHelper.StartAction(() =>
        {
            isVibration = true;
        }, (float)timeVibration / 1000));
    }
    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
            SaveData();
    }
}
