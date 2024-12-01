using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasAllScene : MonoBehaviour
{
    public static CanvasAllScene instance;
    private static bool isReady = false;

    public Canvas myCanvas;

    [SerializeField] private Text txtNoti;
    public ObjectLoading objLoading;

    public PanelHack panelHack;

    [Space] public PopupGetGift popupGetGift;
    public PanelLoading panelLoading;
    public PanelNoInternet panelNoInternet;

    public int countHack = 0;

    private bool isCheckInternet = true;
    private bool IsSendWeb = true;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            isReady = true;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (isReady)
                Destroy(gameObject);
        }
    }
  
    private void Update()
    {
        CheckInternet();
    }

    #region [Check Internet]
    private void ShowPopupNoInternet(bool isSendRequest)
    {
        isCheckInternet = false;
        objLoading.Hide();
        panelNoInternet.ShowPanel(() =>
        {
            isCheckInternet = true;
            IsSendWeb = true;
        }, isSendRequest);
    }
    private void CheckInternet()
    {
        if (isCheckInternet)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                ShowPopupNoInternet(false);
            }
        }
    }
    public void CheckInternetSendRequest()
    {
        return;
        StartCoroutine(CheckInternetConnection(isConnected =>
        {
            if (isConnected)
            {
                Debug.Log("Internet Available!");
                isCheckInternet = true;
                IsSendWeb = true;
                objLoading.Hide();
            }
            else
            {
                Debug.Log("Internet Not Available");
                ShowPopupNoInternet(true);
            }
        }));
    }

    IEnumerator CheckInternetConnection(Action<bool> action)
    {
        WWW www = new WWW("http://google.com");
        yield return www;
        if (www.error != null)
        {
            Debug.Log("Error");
            action(false);
        }
        else
        {
            Debug.Log("Success");
            action(true);
        }
    }
    #endregion
    public void ShowNoti(string str)
    {
        txtNoti.gameObject.SetActive(true);
        txtNoti.text = str;
    }
   

}
