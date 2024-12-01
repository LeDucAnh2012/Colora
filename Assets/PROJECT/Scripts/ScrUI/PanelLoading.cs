using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI.Extensions;

public class PanelLoading : PanelBase
{
    [SerializeField] private Image imgBar;
    [SerializeField] private Text txtLoading;
    [SerializeField] private Text txtPersen;

    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float timeLoading;

    public AsyncOperation asyncOperation;

    private float countTime = 0;
    private string nameScene;
    private bool isControlAllowActive = false;
    private bool isActiveScene = false;
    public void LoadingProgressReal(string nameScene, bool isControlAllowActive = false)
    {
        Show();
        Debug.Log("Loading Progress Real");
        isActiveScene = false;
        this.isControlAllowActive = isControlAllowActive;
        imgBar.fillAmount = 0;
        txtPersen.text = "0%";
        this.nameScene = nameScene;
        countTime = 0;

        StartCoroutine(ActionHelper.IE_TextLoading(txtLoading, I2.Loc.ScriptLocalization.Loading, 0.2f));
        asyncOperation = SceneManager.LoadSceneAsync(nameScene);
        if (IE_LOADING_REAL !=null)
            StopCoroutine(IE_LOADING_REAL);

        IE_LOADING_REAL = IE_LoadingReal();
        StartCoroutine(IE_LOADING_REAL);
    }

    private IEnumerator IE_LOADING_REAL = null;
    private IEnumerator IE_LoadingReal()
    {
        while (!asyncOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(asyncOperation.progress / 0.9f);

            imgBar.fillAmount = progressValue;
            txtPersen.text = ((int)(progressValue * 100)) + "%";
            yield return null;
        }

        if (!isControlAllowActive)
            EndLoading();

        //while (countTime <= timeLoading)
        //{
        //    yield return new WaitForEndOfFrame();
        //    countTime += Time.deltaTime;

        //    float per = curve.Evaluate(countTime / timeLoading);

        //    imgBar.fillAmount = per;
        //    per *= 100;

        //    txtLoading.text = (int)per + "%";
        //    if (countTime >= timeLoading)
        //    {
        //        ActiveScene();
        //    }
        //}
    }
    public void EndLoading()
    {
        if (IE_LOADING_REAL != null)
            StopCoroutine(IE_LOADING_REAL);
        Hide();
    }
    private void ActiveScene()
    {
        if (!isControlAllowActive)
        {
            Debug.Log("Hide In Loading");
            asyncOperation.allowSceneActivation = true;
            Hide();
        }
    }


    public void LoadingProgressFake()
    {
        Show();
        imgBar.fillAmount = 0;
        txtPersen.text = "0%";
        countTime = 0;
        StartCoroutine(IE_LoadingFake());
    }

    private IEnumerator IE_LoadingFake()
    {
        while (countTime <= timeLoading)
        {
            yield return new WaitForEndOfFrame();
            countTime += Time.deltaTime;

            float per = curve.Evaluate(countTime / timeLoading);

            imgBar.fillAmount = per;
            per *= 100;

            txtPersen.text = (int)per + "%";
            if (countTime >= timeLoading)
            {
                Hide();
            }
        }
    }
}
