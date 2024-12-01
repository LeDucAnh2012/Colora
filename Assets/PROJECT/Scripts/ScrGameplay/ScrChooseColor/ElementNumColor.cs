using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ElementNumColor : PanelBase
{
    [SerializeField] private Text txtNumColor;
    [SerializeField] private Image imgColor;
    [SerializeField] private Image imgChoose;
    [SerializeField] private Image imgProgress;
    [SerializeField] private Image imgTick;

    private Color color;
    private int numColor;

    public void LoadData(int numColor, Color color)
    {
        txtNumColor.text = (numColor + 1).ToString();
        this.numColor = numColor;
        this.color = color;
        imgColor.color = color;
    }
    public void OnClickChooseColor()
    {
        SoundClickButton();
        ChooseColor(false, true);
    }
    public void ChooseColor(bool isInit, bool isClick)
    {
        gameplayUIManager.levelLoader.colorFill = color;

        var panel = gameplayUIManager.panelChooseColor;
        panel.eleCurrent?.OnChoose(false);
        panel.eleCurrent = this;

        if (isClick)
            gameplayUIManager.boosterManager.SetOffBooster();

        if (!gameplayUIManager.boosterManager.IsOn)
        {
            panel.eleCurrent.OnChoose(true);
            gameplayUIManager.levelLoader.SetNumColorNew(numColor, isInit);
        }
    }
    public void SetNumColorNew()
    {
        gameplayUIManager.levelLoader.SetNumColorNew(numColor, false);
    }
    public void SetColorDefault()
    {
        gameplayUIManager.levelLoader.SetColorLerp(numColor);
    }
    public void OnChoose(bool isChoose)
    {
        imgChoose.gameObject.SetActive(isChoose);
    }
    public void SetProgressNumColor(int value, int maxValue)
    {
        imgProgress.fillAmount = 1 - ((float)value) / maxValue;
    }
    public void SetDoneNumColor(UnityAction<bool> callback)
    {
        txtNumColor.gameObject.transform.DOScale(0, 0.5f).SetEase(Ease.OutSine).OnComplete(() =>
        {
            imgTick.gameObject.SetActive(true);
            imgTick.gameObject.transform.DOScale(1, 0.5f).From(0).SetEase(Ease.OutBack).OnComplete(() =>
            {
                GetComponent<RectTransform>().DOSizeDelta(new Vector2(-50, -50), 0.5f).SetEase(Ease.OutSine).SetDelay(0.2f).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    callback?.Invoke(false);
                });
            });
        });


    }
}
