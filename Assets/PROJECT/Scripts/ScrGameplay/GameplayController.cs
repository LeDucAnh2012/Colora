using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum StateGame
{
    Home,
    Ingame,
    Done
}
public enum StateDone
{
    NotDone = -1,
    InProgress = 0,
    Done = 1
}
public enum TypePlayMode
{
    Normal,
    DIY
}
public class GameplayController : MonoBehaviour
{
    public static GameplayController instance;
    private static bool isReady = false;

    public TextureMetadata textureMetadata;
    public ShapeInfo shapeInfo;
    public Texture2D texture;
    public TypePlayMode typePlayMode;
    public bool isStartGame = true;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            isReady = true;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (!isReady)
                Destroy(gameObject);
        }
    }
    public void LoadLevel(ShapeInfo shapeInfo, Texture2D texture)
    {
     
        this.shapeInfo = shapeInfo;
        this.texture = texture;
        ShowLoad();
    }
    public void LoadLevel(TextureMetadata textureMetadata, Texture2D texture)
    {
        this.textureMetadata = textureMetadata;
        this.texture = texture;
        ShowLoad();
    }
    private void ShowLoad()
    {
        if (RemoteConfig.instance.allConfigData.BannerCollapInGameplay )
        {
            if (DataAllShape.GetCountPicDone() != 0)
            {
                CanvasAllScene.instance.panelLoading.LoadingProgressReal(TypeSceneCurrent.GameplayScene.ToString(), true);
                ActionHelper.ShowBannerCollapse(false, () =>
                {
                    CanvasAllScene.instance.panelLoading.EndLoading();
                });
                return;
            }
            else
            {
                CanvasAllScene.instance.panelLoading.LoadingProgressReal(TypeSceneCurrent.GameplayScene.ToString(), false);
            }
        }
        else
            CanvasAllScene.instance.panelLoading.LoadingProgressReal(TypeSceneCurrent.GameplayScene.ToString(), false);
    }
}