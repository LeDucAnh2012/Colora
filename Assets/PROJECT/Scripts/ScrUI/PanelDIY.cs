using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PanelDIY : PanelBase
{
    [SerializeField] private GameObject panelDJY;
    [SerializeField] private GameObject btnRemoveAds;
    [SerializeField] private PanelTakePhoto panelTakePhoto;

  
    public override void Show()
    {
        base.Show();
        Debug.Log("SHOW DIY");
        ConfigBtnRemoveAds();

        panelDJY.SetActive(true);
        ConfigBtnRemoveAds();
    }
    #region OnClick
    public void OnClickImport()
    {
        SoundClickButton();
        SceneManager.LoadScene(TypeSceneCurrent.DIYScene.ToString());
    }
    public void OnClickBack()
    {
        SoundClickButton();
        panelDJY.SetActive(true);
        ConfigBtnRemoveAds();
    }
    private void ConfigBtnRemoveAds()
    {
        btnRemoveAds.SetActive(!VariableSystem.RemoveAds);
    }
    #endregion
}
