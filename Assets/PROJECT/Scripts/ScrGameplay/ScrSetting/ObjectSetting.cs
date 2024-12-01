using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectSetting : PanelBase
{
    [SerializeField] private Image imgObj;
    [SerializeField] private Image imgFrame;

    [Space][SerializeField] private Sprite sprOn;
    [SerializeField] private Sprite sprOff;

    [Space][SerializeField] private Sprite sprFrameOn;
    [SerializeField] private Sprite sprFrameOff;
    public void OnOffObject(bool isOn)
    {
        imgFrame.sprite = isOn ? sprFrameOn : sprFrameOff;
        imgObj.sprite = isOn ? sprOn : sprOff;
    }
    /// <summary>
    /// 0: music
    /// 1: sound
    /// 2: vibration
    /// </summary>
    /// <param name="index"></param>
    public void OnClickObject(int index)
    {
        SoundClickButton();
        homeUIManager.popupSetting.ConfigSetting(index);
    }
}
