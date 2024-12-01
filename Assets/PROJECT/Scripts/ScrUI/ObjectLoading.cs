using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLoading : PanelBase
{
    private IEnumerator IE_AUTO_HIDE;
    public override void Show()
    {
        base.Show();
        if (IE_AUTO_HIDE != null)
        {
            StopCoroutine(IE_AUTO_HIDE);
            IE_AUTO_HIDE = null;
        }
        IE_AUTO_HIDE = IE_AutoHide();
        StartCoroutine(IE_AUTO_HIDE);
    }
    private IEnumerator IE_AutoHide()
    {
        yield return new WaitForSeconds(3);
        Hide();
    }
    public override void Hide()
    {
        if (IE_AUTO_HIDE != null)
        {
            StopCoroutine(IE_AUTO_HIDE);
            IE_AUTO_HIDE = null;
        }

        base.Hide();
    }
}
