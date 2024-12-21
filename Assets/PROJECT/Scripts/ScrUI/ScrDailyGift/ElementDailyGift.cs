using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
[System.Serializable]
public struct GiftInfo
{
    public int valGift;
    public Sprite sprGift;
    public TypeBooster typeBooster;
}
public class ElementDailyGift : PanelBase
{
    public int id;
    [SerializeField] private Text txtDay;
    [SerializeField] private Image imgNoti;
    [SerializeField] private GameObject objGifted;
    [SerializeField] private Button btnClaim;
    [SerializeField] private Button btnClaimX2;

    public List<GiftInfo> listGiftInfo = new List<GiftInfo>();
    public void OnClickClaim()
    {
        SoundClickButton();
        VariableSystem.IsCollect = true;
        VariableSystem.IsCollectX2 = true;
        homeUIManager.panelDailyGift.Collect(1);
    }
    public void OnClickClaimX2()
    {
        SoundClickButton();
        VariableSystem.IsCollectX2 = true;
        VariableSystem.IsCollect = true;
        homeUIManager.panelDailyGift.Collect(2);
    }
    public void Init()
    {
        txtDay.text = I2.Loc.ScriptLocalization.Day.ToUpper() + " " + (id + 1);
    }
    public void NotReceived()
    {
        imgNoti.gameObject.SetActive(false);
        objGifted.gameObject.SetActive(false);
        btnClaim.gameObject.SetActive(false);
        btnClaimX2.gameObject.SetActive(false);
    }
    public void Receiving()
    {if (id == 1)
            Debug.Log("receiving");
        objGifted.gameObject.SetActive(false);
        imgNoti.gameObject.SetActive(true);
        btnClaim.gameObject.SetActive(true);
        btnClaimX2.gameObject.SetActive(true);
    }
    public void Received()
    {
        imgNoti.gameObject.SetActive(false);
        objGifted.gameObject.SetActive(true);
        btnClaim.gameObject.SetActive(false);
        btnClaimX2.gameObject.SetActive(false);
    }

}
