using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;

public class PopupRate : PanelBase
{
    [SerializeField] private List<GameObject> listStar;
    [SerializeField] private GameObject imgHand;
    [Button]
    public void ShowPopup()
    {
        base.Show();
      //  SoundShowPopup();
        if (IE_SHOW_STAR != null)
        {
            StopCoroutine(IE_SHOW_STAR);
            IE_SHOW_STAR = null;
        }
        imgHand.transform.DOLocalMove(Vector2.zero,0.5f).SetEase(Ease.InQuart).SetLoops(-1,LoopType.Yoyo);
        IE_SHOW_STAR = IE_ShowStar();
        StartCoroutine(IE_SHOW_STAR);
    }
    private IEnumerator IE_SHOW_STAR;
    private IEnumerator IE_ShowStar()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            GameObject g = listStar[^1];
            g.SetActive(!g.activeSelf);
        }
    }
    public void OnClickStar(int indexStar)
    {
        for (int i = 0; i <= indexStar; i++)
            listStar[i].SetActive(true);

        for (int i = indexStar + 1; i < listStar.Count; i++)
            listStar[i].SetActive(false);

        if (indexStar > 3)
        {
            VariableSystem.IsRate = true;
            RateInApp();
        }
        else
        {
            VariableSystem.CountShowRate = 3;
        }

        imgHand.SetActive(false);
        if (IE_SHOW_STAR != null)
        {
            StopCoroutine(IE_SHOW_STAR);
            IE_SHOW_STAR = null;
        }
        Invoke(nameof(Hide), 0.5f);
    }
    private void RateInApp()
    {
        string url = "https://play.google.com/store/apps/details?id=" + Application.identifier;
        Application.OpenURL(url);
        base.Hide();
    }
    public void OnClickExit()
    {
        SoundClickButton();
        VariableSystem.CountShowRate = 3;
        base.Hide();
    }
}
