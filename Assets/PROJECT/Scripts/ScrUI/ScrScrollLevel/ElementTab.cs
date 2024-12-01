using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementTab : PanelBase
{
    public int indexTab;
    public TypeTopic typeTopic;
    [SerializeField] private Text txtTopic;
    [SerializeField] private Image imgBgSelect;
    [SerializeField] private Image imgFrameSelect;
    [SerializeField] private Outline outLine;
    [SerializeField] private Shadow[] shadows;

    [Space]
    [SerializeField] private Color colorChoose;
    [SerializeField] private Color colorNone;
    [Space]
    [SerializeField] private Color colorTextChoose;
    [SerializeField] private Color colorTextNone;
    [SerializeField] private ScrollTopic scrollTopic;
    public void LoadData(int indexTab,ScrollTopic scrollTopic)
    {
        this.indexTab = indexTab;
        this.scrollTopic = scrollTopic;
        txtTopic.text = typeTopic.ToString();
        SetColor(false);
    }
    public void OnClickTab()
    {
        //if (!homeUIManager.panelLevel.isClickElementTab) return;

        SoundClickButton();
        ActiveTab(ref homeUIManager.panelLevel.scrollTopicCurrent, ref homeUIManager.panelLevel.elementTabCurrent);
        homeUIManager.panelLevel.SetAnchorTab(indexTab);
        homeUIManager.panelLevel.SetAnchorScroll(indexTab);
    }

    public void ActiveTab(ref ScrollTopic scrollTopicCurrent, ref ElementTab elementTabCurrent)
    {
        elementTabCurrent.SetColor(false);
        scrollTopicCurrent.Hide();

        elementTabCurrent = this;
        scrollTopicCurrent = elementTabCurrent.scrollTopic;
        elementTabCurrent.SetColor(true);
        scrollTopicCurrent.Show();
    //    scrollTopicCurrent.transform.position = new Vector2(0,-2.1f);
    }

    public void SetColor(bool isActive)
    {
        imgBgSelect.gameObject.SetActive(isActive);
        imgFrameSelect.gameObject.SetActive(isActive);
        var color = isActive ? colorChoose : colorNone;
        var colorText = isActive ? colorTextChoose : colorTextNone;
        foreach (Shadow shadow in shadows)
            shadow.effectColor = color;
        outLine.effectColor = color;
        txtTopic.color = colorText;
        txtTopic.transform.localScale = Vector2.one * (isActive ? 1.1f : 1f);
    }
}
