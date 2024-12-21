using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelDailyGift : PanelBase
{
    [SerializeField] private List<ElementDailyGift> listElementDailyGift = new List<ElementDailyGift>();
    private int idEleReceiving = -1;
    public GameObject imgNoti;
    public override void Show()
    {
        base.Show();
        InitData();
    }
    public void InitData()
    {
        imgNoti.gameObject.SetActive(false);
        idEleReceiving = -1;

        for (int i = 0; i < listElementDailyGift.Count; i++)
        {
            var ele = listElementDailyGift[i];
            ele.Init();
            if (ele.id == VariableSystem.CountReceivedDaily)
            {
                ele.Received();
                if (!VariableSystem.IsCollectX2)
                {
                    idEleReceiving = ele.id;
                    ele.Receiving();
                    imgNoti.gameObject.SetActive(true);
                }

                if (!VariableSystem.IsCollect)
                {
                    idEleReceiving = ele.id;
                    ele.Receiving();
                    imgNoti.gameObject.SetActive(true);
                }
                else
                {
                    ele.Received();
                }
            }
            else if (ele.id > VariableSystem.CountReceivedDaily)
            {
                ele.NotReceived();
            }
            else
            {
                ele.Received();
            }
        }
    }
    public void Collect(int valMultip)
    {
        Debug.Log("Collect");
        if (idEleReceiving == -1) return;

        // show gift
        var listGiftInfo = listElementDailyGift[idEleReceiving].listGiftInfo;

        var listSprite = new List<Sprite>();
        var listInt = new List<int>();
        var listTypeGift = new List<TypeBooster>();
        Debug.Log("listGiftInfo = " + listGiftInfo.Count);

        foreach (var gift in listGiftInfo)
        {
            var val = gift.valGift * valMultip;

            listSprite.Add(gift.sprGift);
            listInt.Add(val);
            listTypeGift.Add(gift.typeBooster);

            switch (gift.typeBooster)
            {
                case TypeBooster.Find:
                    VariableSystem.FindBooster += val;
                    break;
                case TypeBooster.Bomb:
                    VariableSystem.FillByBomBooster += val;
                    break;
                case TypeBooster.Number:
                    VariableSystem.FillByNumBooster += val;
                    break;
            }
        }

        canvasAllScene.popupGetGift.ShowPopup(listSprite, listInt, listTypeGift, Callback);
    }
    private void Callback()
    {
        InitData();
      //  canvasAllScene.UpdateStoreCurrency();
    }
}
