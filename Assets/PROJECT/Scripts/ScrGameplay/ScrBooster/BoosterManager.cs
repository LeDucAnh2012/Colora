using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BoosterManager : PanelBase
{
    [SerializeField] private Image iconZoom;
    [SerializeField] private Button btnBooster;
    [SerializeField] private Image iconBooster;
    [SerializeField] private Text txtBooster;

    [Space]
    [SerializeField] private Sprite sprZoomIn;
    [SerializeField] private Sprite sprZoomOut;

    public ElementBooster findBooster;
    public ElementBooster fillByNum;
    public ElementBooster fillByBomb;
    public Text txtSpawnPlus;

    [Space] public Sprite sprFind;
    public Sprite sprFillByNum;
    public Sprite sprFillByBom;

    public ElementBooster boosterCurrent = null;
    public bool IsOn = false;
    public bool isCountTimeShowBooster = false;
    public bool isStartCountTime = false;
    public bool isCountTimeOffBtn = false;

    private bool isZoomIn = false;
    public float timeShow = 0;
    public float timeOfExistence = 0;
    private int valBooster = 3;
    private TypeBooster typeBoosterWatchAds;
    private bool isClickWatch = false;
    private void Start()
    {
        ResetCountTime();
        UpdateUI();
        Init();
    }
    private void Update()
    {
        CountTimeShow();
        CountTimeOffBtn();
    }
    private void CountTimeOffBtn()
    {
        if (!isCountTimeOffBtn) return;
        timeOfExistence -= Time.deltaTime;
        if (timeOfExistence < 0)
        {
            isCountTimeOffBtn = false;
            timeOfExistence = RemoteConfig.instance.allConfigData.TimeOfExistence;

            btnBooster.transform.DOScale(0, 0.4f).From(1).SetEase(Ease.InBack).OnComplete(() =>
            {
                btnBooster.gameObject.SetActive(false);
            });
            ReCountTime();
        }
    }

    public void ResetCountTime()
    {
        timeOfExistence = RemoteConfig.instance.allConfigData.TimeOfExistence;
        timeShow = RemoteConfig.instance.allConfigData.TimeShowBuyBooster;
        isStartCountTime = false;
        isCountTimeShowBooster = false;
        isClickWatch = false;
        isCountTimeOffBtn = false;
        btnBooster.gameObject.SetActive(false);
    }
    public void StopShowBooster()
    {
        ResetCountTime();
    }
    public void SetStartCountTime()
    {
        if (isClickWatch) return;
        if (btnBooster.gameObject.activeSelf) return;
        isStartCountTime = true;
        isCountTimeShowBooster = true;
    }
    private void ReCountTime()
    {
        timeShow = RemoteConfig.instance.allConfigData.TimeCallBackShowBooster;
        timeOfExistence = RemoteConfig.instance.allConfigData.TimeOfExistence;
        isStartCountTime = true;
        isCountTimeShowBooster = true;
        isCountTimeOffBtn = false;
    }
    private void CountTimeShow()
    {
        if (!isStartCountTime) return;
        if (!isCountTimeShowBooster) return;
        timeShow -= Time.deltaTime;
        if (timeShow < 0)
        {
            isCountTimeOffBtn = true;
            isCountTimeShowBooster = false;
            ShowBooster();
        }
    }

    private void ShowBooster()
    {
        btnBooster.gameObject.SetActive(true);
        btnBooster.transform.DOScale(1, 0.4f).From(0).SetEase(Ease.OutBack).OnComplete(() =>
        {
            btnBooster.transform.DOScale(1.05f, 0.4f).From(1).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        });

        int idBooster = Random.Range(1, 4);
        typeBoosterWatchAds = (TypeBooster)idBooster;

        switch (typeBoosterWatchAds)
        {
            case TypeBooster.Number:
                iconBooster.sprite = sprFillByNum;
                break;
            case TypeBooster.Bomb:
                iconBooster.sprite = sprFillByBom;
                break;
            case TypeBooster.Find:
                iconBooster.sprite = sprFind;
                break;
        }
        iconBooster.SetNativeSize();
        txtBooster.text = "+" + valBooster;
    }
    public void OnClickBoosterAds()
    {
        SoundClickButton();

        isClickWatch = true;
        isCountTimeOffBtn = false;
        isCountTimeShowBooster = false;
        isStartCountTime = false;

        ActionHelper.ShowRewardAds(KeyLogFirebase.Colora_RW_BoosterInPlayGame_211224,"Rw_" + typeBoosterWatchAds.ToString() + "_Ingame", CallBack);
    }
    private void CallBack(bool isComplete)
    {
        btnBooster.gameObject.SetActive(false);
        ReCountTime();

        if (!isComplete) return;

        var listSpr = new List<Sprite>();
        var listVal = new List<int>();
        var listType = new List<TypeBooster>();

        listVal.Add(valBooster);
        listType.Add(typeBoosterWatchAds);

        switch (typeBoosterWatchAds)
        {
            case TypeBooster.Number:
                listSpr.Add(sprFillByNum);
                VariableSystem.FillByNumBooster += valBooster;
                break;
            case TypeBooster.Bomb:
                listSpr.Add(sprFillByBom);
                VariableSystem.FillByBomBooster += valBooster;
                break;
            case TypeBooster.Find:
                listSpr.Add(sprFind);
                VariableSystem.FindBooster += valBooster;
                break;
        }

        gameplayUIManager.zoomInZoomOut.enabled = false;
        canvasAllScene.popupGetGift.ShowPopup(listSpr, listVal, listType, () =>
        {
            gameplayUIManager.zoomInZoomOut.enabled = true;
        });
    }
    public void UpdateUI()
    {
        findBooster.UpdateUI();
        fillByNum.UpdateUI();
        fillByBomb.UpdateUI();
    }
    public void Init()
    {
        findBooster.Init();
        fillByNum.Init();
        fillByBomb.Init();
    }
    public void SetOffBooster()
    {
        if (fillByNum.isUse)
            fillByNum.SwitchBooster();
        if (fillByBomb.isUse)
            fillByBomb.SwitchBooster();
    }
    public void SetNumWhenChooseBooster(bool isChoose)
    {
        if (isChoose)
            gameplayUIManager.panelChooseColor.eleCurrent?.SetNumColorNew();
        else
            gameplayUIManager.panelChooseColor.eleCurrent?.SetColorDefault();

        gameplayUIManager.panelChooseColor.eleCurrent?.OnChoose(isChoose);
    }
    public void OnClickZoom()
    {
        if (CanvasAllScene.instance.countHack >= 2 && CanvasAllScene.instance.countHack < 4)
            CanvasAllScene.instance.countHack++;

        isZoomIn = !isZoomIn;

        if (isZoomIn)
        {
            iconZoom.sprite = sprZoomOut;
            gameplayUIManager.zoomInZoomOut.AutoZoomCam(5);
        }
        else
        {
            iconZoom.sprite = sprZoomIn;
            gameplayUIManager.zoomInZoomOut.AutoZoomCam(45);
        }

        if (KeepObject.instance.mode == TypeMode.Release)
            if (!RemoteConfig.instance.allConfigData.IsDebug)
                return;

        if (IE_DELAY != null)
        {
            StopCoroutine(IE_DELAY);
            IE_DELAY = null;
        }

        IE_DELAY = IE_Delay();
        StartCoroutine(IE_DELAY);

        CanvasAllScene.instance.countHack++;
        if (CanvasAllScene.instance.countHack >= 10)
        {
            CanvasAllScene.instance.countHack = 0;
            CanvasAllScene.instance.panelHack.ShowPanel();
        }
    }
    private IEnumerator IE_DELAY;
    private IEnumerator IE_Delay()
    {
        yield return new WaitForSeconds(1);
        CanvasAllScene.instance.countHack = 0;
    }
}
