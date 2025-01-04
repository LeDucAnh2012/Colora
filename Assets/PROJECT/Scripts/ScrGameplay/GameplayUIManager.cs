using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using UnityEngine;
using UnityEngine.UI;

public enum TypeBooster
{
    FillByRow,
    Number,
    Bomb,
    Find,
}


public class GameplayUIManager : MonoBehaviour
{
    public static GameplayUIManager instance;


    [TabGroup("UI")] public GameObject objTab;
    [TabGroup("UI")] public Button btnBack;
    [TabGroup("UI")] public Button btnOnOffUI;
    [TabGroup("UI")] public Text txtOnOffUI;
    [TabGroup("UI")] public Button btnOnOffColorCam;
    [TabGroup("UI")] public Text txtOnOffColorCam;
    [Space]
    [TabGroup("UI")][SerializeField] private GameObject gridButton;
    [TabGroup("UI")] public GameObject canvasGameplay;

    [TabGroup("Panel")] public PanelChooseFrameBG panelChooseFrameBG;
    [TabGroup("Panel")] public PanelChooseColor panelChooseColor;


    [Space][TabGroup("Panel")] public ProgressPainting progressPainting;
    [Space][TabGroup("Panel")] public PopupRemoveAds popupRemoveAds;
    [Space][TabGroup("Panel")] public PopupCompleteLevel popupCompleteLevel;

    [Space][TabGroup("Panel")][SerializeField] private PopupRate popupRate;

    [TabGroup("Panel")] public TutorialManager tutorialManager;

    [Space] public BoosterManager boosterManager;
    public SpriteRenderer sprRenderFrame;
    public SpriteRenderer sprRenderBG;
    [SerializeField] private ParticleSystem psConfetti;

    [Space] public LevelLoader levelLoader;
    public ZoomInZoomOut zoomInZoomOut;
    public ScreenshotHandler screenshotHandler;

    //public List<IEnumerator> listIEnum = new List<IEnumerator>();

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        panelChooseFrameBG.LoadData();

        if (!VariableSystem.FirstOpenGame)
        {
            tutorialManager.ShowTut();
            VariableSystem.FirstOpenGame = true;
        }

     
        LoadLevel();

        if (KeepObject.instance.mode == TypeMode.Marketing)
        {
            btnOnOffUI.gameObject.SetActive(true); 
            btnOnOffColorCam.gameObject.SetActive(true); 
            ConfigAppForMarkting();
        }
    }

    public void LoadNumColor(ShapeInfo shapeInfo)
    {
        panelChooseColor.SpawnElementNumColor(shapeInfo);
    }
    public void LoadNumColor(TextureMetadata textureMetadata)
    {
        panelChooseColor.SpawnElementNumColor(textureMetadata);
    }

    public void EventPointerDown()
    {
        SoundMusicManager.instance.SoundClickButton();
        levelLoader.FillByRow();
        zoomInZoomOut.isTouch = false;
    }
    public void EventPointerUp()
    {
        levelLoader.StopFillByRow();
        zoomInZoomOut.isTouch = true;
    }

    private IEnumerator IE_BackHome()
    {
        yield return new WaitForEndOfFrame();
        InitDataGame.instance.LoadData();
        yield return new WaitForEndOfFrame();
        levelLoader.SaveData();
        yield return new WaitForEndOfFrame();


        if (RemoteConfig.instance.allConfigData.BannerCollapInBackHome)
        {
            CanvasAllScene.instance.panelLoading.LoadingProgressReal(TypeSceneCurrent.HomeScene.ToString(), true);
            ActionHelper.ShowBannerCollapse(false, () =>
            {
                CanvasAllScene.instance.panelLoading.EndLoading();
            });
        }
        else
        {
            CanvasAllScene.instance.panelLoading.LoadingProgressReal(TypeSceneCurrent.HomeScene.ToString(), false);
        }

    }

    public void ReloadShape()
    {
        Debug.Log("ReloadShape");
        zoomInZoomOut.enabled = true;
        gridButton.SetActive(true);
        panelChooseFrameBG.Hide();

        sprRenderFrame.gameObject.SetActive(false);
        sprRenderBG.gameObject.SetActive(false);
    }
    public void OnClickBack()
    {
        ActionHelper.CheckShowInter(KeyLogFirebase.Colora_INT_BackHome_211224,(bool isShowCompleted) =>
        {
            StartCoroutine(IE_BackHome());
        });
    }

    public void SoundClickButton()
    {
        SoundMusicManager.instance.SoundClickButton();
    }

    public void ShowWin()
    {
        Debug.Log("Show win");
        var gameplay = GameplayController.instance;
        // log firebase
        ActionHelper.LogEvent(KeyLogFirebase.DonePic + levelLoader.shapeInfo.nameShape);

        zoomInZoomOut.enabled = false;
        btnBack.gameObject.SetActive(false);

        VariableSystem.CountShowRate--;
        if (GameplayController.instance.typePlayMode == TypePlayMode.Normal)
            levelLoader.shapeInfo.StateDone = StateDone.Done;
        else
            DataDIY.SetStateDone(StateDone.Done);
        levelLoader.SaveData();
    }
    public void ShapeDone()
    {
        Debug.Log("shape done");
        objTab.SetActive(false);
        boosterManager.StopShowBooster();
        levelLoader.SetPosScaleParentSpawn();
        panelChooseFrameBG.InitData();
        gridButton.gameObject.SetActive(false);

        if (RemoteConfig.instance.allConfigData.BannerCollapInFrameBG)
        {
            ActionHelper.ShowBannerCollapse(true, () =>
            {
                ShowPanelChooseFrame();
            });
        }
        else
            ShowPanelChooseFrame();
    }
    public void LoadLevel()
    {
        boosterManager.UpdateUI();
        SoundMusicManager.instance.PlayMusic(StateGame.Ingame);
        gridButton.gameObject.SetActive(true);
        btnBack.gameObject.SetActive(true);

        if (VariableSystem.CountShowRate == 0 && !VariableSystem.IsRate)
        {
            VariableSystem.CountShowRate = 3;
            popupRate.ShowPopup();
        }

        zoomInZoomOut.enabled = true;
        boosterManager.ResetCountTime();

        if (GameplayController.instance.typePlayMode == TypePlayMode.Normal)
            levelLoader.LoadLevel(GameplayController.instance.shapeInfo, GameplayController.instance.texture);
        else
            levelLoader.LoadLevel(GameplayController.instance.textureMetadata, GameplayController.instance.texture);
    }
    public void ShowPanelChooseFrame()
    {
        zoomInZoomOut.enabled = false;
        zoomInZoomOut.SetSizeAndPosCam(new Vector3(0, -14, -10));
        levelLoader.SetFrameBG();
        progressPainting.Hide();
        Debug.Log("Set color");
        Camera.main.backgroundColor = new Color32(202, 215,115,255);
        Debug.Log("ShowPanelChooseFrame");
        levelLoader.VisualDoneColor(() =>
        {
            panelChooseFrameBG.Show();
        });
    }
    public void SetConfetti()
    {
        psConfetti.Play();
        ActionHelper.SetVibration(150);
    }

    public void OnOffUI()
    {
        SoundClickButton();
        VariableSystem.OnOffUIGameplay = !VariableSystem.OnOffUIGameplay;
        ConfigAppForMarkting();
    }
    private void ConfigAppForMarkting()
    {
        var isOn = VariableSystem.OnOffUIGameplay;
        btnBack.gameObject.SetActive(isOn);
        objTab.gameObject.SetActive(isOn);

        txtOnOffColorCam.gameObject.SetActive(isOn);
        txtOnOffUI.gameObject.SetActive(isOn);
        btnOnOffUI.image.color = new Color(1, 1, 1, isOn ? 1 : 0);
        btnOnOffColorCam.image.color = new Color(1, 1, 1, isOn ? 1 : 0);
        progressPainting.gameObject.SetActive(isOn);
        gridButton.gameObject.SetActive(isOn);

        var isOn1 = VariableSystem.OnOffColorCam;
        zoomInZoomOut.mainCamera.backgroundColor = isOn1 ? new Color(0, 0, 1, 1) : Color.white;
    }
    public void OnOffColorCam()
    {
        SoundClickButton();
        VariableSystem.OnOffColorCam = !VariableSystem.OnOffColorCam;
        ConfigAppForMarkting();
    }
}
