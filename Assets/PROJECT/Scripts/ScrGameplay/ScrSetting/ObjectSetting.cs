using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectSetting : PanelBase
{
    [SerializeField] private Image imgFrame;

    [Space][SerializeField] private Sprite sprFrameOn;
    [SerializeField] private Sprite sprFrameOff;
    public void OnOffObject(bool isOn)
    {
        imgFrame.sprite = isOn ? sprFrameOn : sprFrameOff;
        imgFrame.SetNativeSize();
    }
}
