using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class PanelNoInternet : PanelBase
{
    private IEnumerator IE_RELOAD = null;
    private UnityAction callback = null;
    private bool isSendRequest = false;
    public void ShowPanel(UnityAction callback, bool isSendRequest)
    {
        base.Show();
        this.isSendRequest = isSendRequest;
        this.callback = callback;
    }
    public void OnClickConnect()
    {
        SoundClickButton();
        canvasAllScene.objLoading.Show();

        if (IE_RELOAD != null)
            StopCoroutine(IE_RELOAD);

        IE_RELOAD = IE_Reload();
        StartCoroutine(IE_RELOAD);
    }
    private IEnumerator IE_Reload()
    {
        yield return new WaitForSeconds(0.5f);

        if (!isSendRequest)
        {
            canvasAllScene.objLoading.Hide();
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                RemoteConfig.instance.CheckRemoteConfigValues();
                callback?.Invoke();
                StopCoroutine(IE_RELOAD);
                base.Hide();
            }
            else
            {
                base.Hide();
                base.Show();
            }
        }
        else
        {
            canvasAllScene.CheckInternetSendRequest();
        }
    }
    public override void Show()
    {
        base.Show();
        SoundShowPopup();
    }
}
