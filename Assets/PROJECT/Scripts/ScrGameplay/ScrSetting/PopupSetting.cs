using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupSetting : PanelBase
{
    [SerializeField] private Text txtVer;
    [SerializeField] private ObjectSetting objMusic;
    [SerializeField] private ObjectSetting objSound;
    [SerializeField] private ObjectSetting objVibration;

    public void Init()
    {
        txtVer.text = "VER " + Application.version;
        objMusic.OnOffObject(VariableSystem.Music == 1);
        objSound.OnOffObject(VariableSystem.Sound == 1);
        objVibration.OnOffObject(VariableSystem.Vibrate);
        SoundShowPopup();
    }
    public void ConfigSetting(int index)
    {
        switch (index)
        {
            case 0:
                VariableSystem.Music = Mathf.Abs(VariableSystem.Music - 1);
                SoundMusicManager.instance.PlayMusic(StateGame.Home);
                break;
            case 1:
                VariableSystem.Sound = Mathf.Abs(VariableSystem.Sound - 1);
                break;
            case 2:
                VariableSystem.Vibrate = !VariableSystem.Vibrate;
                break;
        }
        Init();
    }
    public void OnClickHide()
    {
        SoundClickButton();
        if (homeUIManager.panelLevel.gameObject.activeSelf)
            homeUIManager.managerNativeAds.Show();
        Hide();
    }
}
