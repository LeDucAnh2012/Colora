using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PopupWarring : PanelBase
{
    private UnityAction<bool> callback = null;
    public void ShowPopup(UnityAction<bool> callback = null)
    {
        this.callback = callback;
        base.Show();
        SoundShowPopup();
    }
    public void OnClickConfirm()
    {
        SoundClickButton();
        HidePopup(true);
    }
    public void OnClickCancel()
    {
        SoundClickButton();
        HidePopup(false);
    }
    public void HidePopup(bool isConfirm)
    {
        callback?.Invoke(isConfirm);
        base.Hide();
    }
}
