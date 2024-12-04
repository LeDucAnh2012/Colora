using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupSetting : PanelBase
{
    [SerializeField] private Text txtVer;

    [SerializeField] private ObjectSetting iconSound;
    [SerializeField] private ObjectSetting iconMusic;
    [SerializeField] private ObjectSetting iconVibrate;

    [Space]
    [SerializeField] private Button btnSound;
    [SerializeField] private Button btnMusic;
    [SerializeField] private Button btnVibration;

    [Space]
    [SerializeField] private GameObject objSound;
    [SerializeField] private GameObject objMusic;
    [SerializeField] private GameObject objVibration;


    [Space]
    [SerializeField] private Sprite sprOn;
    [SerializeField] private Sprite sprOff;

    private bool isChange = true;
    public void Init()
    {
        txtVer.text = "Version " + Application.version;
        SetMusicSound(VariableSystem.Sound, btnSound, objSound);
        iconSound.OnOffObject(VariableSystem.Sound);

        SetMusicSound(VariableSystem.Music, btnMusic, objMusic);
        iconMusic.OnOffObject(VariableSystem.Music);

        SetMusicSound(VariableSystem.Vibrate, btnVibration,objVibration);
        iconVibrate.OnOffObject(VariableSystem.Vibrate);
        SoundShowPopup();
    }
    public void OnClickSound()
    {
        if (!isChange) return;

        SoundClickButton();
        VariableSystem.Sound = !VariableSystem.Sound;
        SetMusicSound(VariableSystem.Sound, btnSound, objSound);
        iconSound.OnOffObject(VariableSystem.Sound);
    }
    public void OnClickMusic()
    {
        if (!isChange) return;

        SoundClickButton();
        VariableSystem.Music = !VariableSystem.Music;
        SoundMusicManager.instance.PlayMusic(ActionHelper.GetSceneCurrent() == TypeSceneCurrent.GameplayScene ? StateGame.Ingame:StateGame.Home);
        SetMusicSound(VariableSystem.Music, btnMusic, objMusic);
        iconMusic.OnOffObject(VariableSystem.Music);
    }
    public void OnClickVibration()
    {
        if (!isChange) return;

        SoundClickButton();
        VariableSystem.Vibrate = !VariableSystem.Vibrate;
        SetMusicSound(VariableSystem.Vibrate, btnVibration,objVibration);
        iconVibrate.OnOffObject(VariableSystem.Vibrate);
    }
    public void OnClickHide()
    {
        SoundClickButton();
        if (homeUIManager.panelLevel.gameObject.activeSelf)
            homeUIManager.managerNativeAds.Show();
        Hide();
    }

    private void SetMusicSound(bool isState, Button btn, GameObject objNode)
    {
        Vector3 _vt;
        float valueEnd;

        _vt = objNode.transform.localPosition;
        valueEnd = isState ? Mathf.Abs(_vt.x) : (Mathf.Abs(_vt.x) * -1);

        objNode.transform.DOLocalMoveX(valueEnd, 0.15f).SetEase(Ease.OutCirc).OnComplete(() =>
        {
            btn.image.sprite = isState ? sprOn : sprOff;
            btn.image.SetNativeSize();
            isChange = true;
        });
    }
}
