using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum TypeElement
{
    None,
    Frame,
    BG
}
public class ElementChooseFrameBG : PanelBase
{
    [SerializeField] private TypeElement typeElement;
    [SerializeField] private Image imgFrame;
    [SerializeField] private Image imgSelect;
    [SerializeField] private Image imgAds;
    private FrameBGInfo frameBGInfo;
    public void LoadElement(FrameBGInfo frameBGInfo, TypeElement type)
    {
        this.frameBGInfo = frameBGInfo;
        imgFrame.sprite = frameBGInfo.spr;
        imgAds.gameObject.SetActive(!frameBGInfo.IsUnlock);
        typeElement = type;
        OnNotChoose(false);
    }
    public void InitData(ShapeInfo shape)
    {
        OnNotChoose(false);
        if (typeElement == TypeElement.Frame)
        {
            if (shape.IDFrame == frameBGInfo.id)
            {
                gameplayUIManager.panelChooseFrameBG.eleFrameCurrent = this;
                OnNotChoose(true);
            }
        }

        if (typeElement == TypeElement.BG)
        {
            if (shape.IDBackground == frameBGInfo.id)
            {
                gameplayUIManager.panelChooseFrameBG.eleBGCurrent = this;
                OnNotChoose(true);
            }
        }
    }

    public void OnClickChooseFrame()
    {
        if (!frameBGInfo.IsUnlock)
        {
            Debug.Log("SHOW REWARD");
            ActionHelper.ShowRewardAds("","Rw_" + typeElement.ToString() + "_" + frameBGInfo.id, Choose);
        }
        else
            Choose();
    }
    private void Choose(bool isComplete = true)
    {
        if (!isComplete) return;

        if (!frameBGInfo.IsUnlock)
        {
            imgAds.gameObject.SetActive(false);
            frameBGInfo.IsUnlock = true;
        }

        var panel = gameplayUIManager.panelChooseFrameBG;
        if (typeElement == TypeElement.Frame)
        {
            panel.eleFrameCurrent?.OnNotChoose(false);
            panel.eleFrameCurrent = this;
            panel.eleFrameCurrent?.OnNotChoose(true);

            if (gameplayController.typePlayMode == TypePlayMode.Normal)
                gameplayUIManager.levelLoader.shapeInfo.IDFrame = frameBGInfo.id;
            else
                DataDIY.SaveMetadata(DataDIY.metadataCurrent.textureID, DataDIY.metadataCurrent.idBackground, frameBGInfo.id);
            panel.idFrameChoose = frameBGInfo.id;
        }

        if (typeElement == TypeElement.BG)
        {
            panel.eleBGCurrent?.OnNotChoose(false);
            panel.eleBGCurrent = this;
            panel.eleBGCurrent?.OnNotChoose(true);

            if (gameplayController.typePlayMode == TypePlayMode.Normal)
                gameplayUIManager.levelLoader.shapeInfo.IDBackground = frameBGInfo.id;
            else
                DataDIY.SaveMetadata(DataDIY.metadataCurrent.textureID, frameBGInfo.id, DataDIY.metadataCurrent.idFrame);
        }
    }
    public void OnNotChoose(bool isChoose)
    {
        var gameplay = gameplayUIManager;
        imgSelect.gameObject.SetActive(isChoose);
        if (isChoose)
        {
            if (typeElement == TypeElement.Frame)
            {
                if (frameBGInfo.id == -1)
                {

                    gameplay.sprRenderFrame.gameObject.SetActive(false);
                }
                else
                {
                    gameplay.sprRenderFrame.gameObject.SetActive(true);
                    gameplay.sprRenderFrame.sprite = frameBGInfo.spr;
                    gameplay.sprRenderFrame.transform.localPosition = frameBGInfo.localPos;
                }
                gameplay.panelChooseFrameBG.idFrameChoose = frameBGInfo.id;
            }

            if (typeElement == TypeElement.BG)
            {

                if (frameBGInfo.id == -1)
                {
                    gameplay.sprRenderBG.gameObject.SetActive(false);
                }
                else
                {
                    gameplay.sprRenderBG.gameObject.SetActive(true);
                    gameplay.sprRenderBG.sprite = frameBGInfo.spr;
                }
            }

        }
    }
}

