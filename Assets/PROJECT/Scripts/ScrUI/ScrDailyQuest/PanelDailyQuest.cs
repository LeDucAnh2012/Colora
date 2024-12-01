using DG.Tweening;
using I2.Loc;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PanelDailyQuest : PanelBase
{
    [SerializeField] private List<ElementDailyQuest> listElementDailyQuest = new List<ElementDailyQuest>();
    [SerializeField] private ObjectGift objectGift;
    [SerializeField] private Image imgGift;
    [SerializeField] private Text txtTitleGift;
    [SerializeField] private GameObject panelGift;
    [SerializeField] private GameObject objNoti;

    public bool isGet = false;
    public void LoadData()
    {
        foreach (var data in InitDataGame.instance.listDataDailyQuest)
            data.InitRewardQuest();

        // random task
        var list = new List<int>() { 0, 1, 2, 3, 4 };
        //if (VariableSystem.ListTaskOn.Equals("null"))
        //{
        //    VariableSystem.ListTaskOn = "";
        //    var lst = new List<int>() { 0, 1, 2, 3, 4 };
        //    while (list.Count < 3)
        //    {
        //        var rd = Random.Range(0, lst.Count);
        //        list.Add(lst[rd]);
        //        VariableSystem.ListTaskOn += lst[rd] + ",";
        //        lst.Remove(lst[rd]);
        //    }
        //}
        //else
        //    list = ActionHelper.ConfigListIntFromString(VariableSystem.ListTaskOn);

        for (int i = 0; i < listElementDailyQuest.Count - 2; i++)
        {
            if (!list.Contains(listElementDailyQuest[i].id))
                listElementDailyQuest[i].Hide();
        }
    }
    public void InitData()
    {
        var listDataDailyQuest = InitDataGame.instance.listDataDailyQuest;
        isGet = false;
        int count = 0;
        for (int i = 0; i < listElementDailyQuest.Count - 1; i++)
        {
            if (listElementDailyQuest[i].gameObject.activeSelf)
            {
                if (listDataDailyQuest[i].CountFinishQuest >= listDataDailyQuest[i].AmountQuest)
                    count++;
            }
        }
        //if (count >= listDataDailyQuest[6].AmountQuest)
        listDataDailyQuest[6].CountFinishQuest = count;

        objNoti.SetActive(false);

        for (int i = 0; i < listElementDailyQuest.Count; i++)
        {
            if (listElementDailyQuest[i].gameObject.activeSelf)
                listElementDailyQuest[i].InitData(listDataDailyQuest[i], ref isGet);

            if (!listElementDailyQuest[i].isGetting && listElementDailyQuest[i].data.IsGetQuest)
            {
                Debug.Log("????");
                listElementDailyQuest[i].transform.SetAsLastSibling();
            }
        }

        count = 0;

        for (int i = 0; i < listElementDailyQuest.Count; i++)
        {
            if (listElementDailyQuest[i].data.IsGetQuest && listElementDailyQuest[i].gameObject.activeSelf)
            {
                count++;
            }
        }

        if (count >= 5)
            objNoti.SetActive(false);
        else
            objNoti.SetActive(isGet);
    }
    public void ResetDataReward()
    {
        VariableSystem.ListTaskOn = "null";
        foreach (var data in InitDataGame.instance.listDataDailyQuest)
        {
            data.ResetReward();
            VariableSystem.ResetLogTask(data.id);
        }

        LoadData();
        InitData();
    }
    DataDailyQuest dataCurrent;
    public void ShowGift(DataDailyQuest data, bool isGet = false)
    {
        dataCurrent = data;
        panelGift.SetActive(true);
        panelGift.transform.GetChild(0).DOScale(1, 0.6f).From(0).SetEase(Ease.OutBack);
        txtTitleGift.text = I2.Loc.ScriptLocalization.Your_gifts.ToUpper();
        if (data.id == 6)
        {
            imgGift.gameObject.SetActive(true);
            imgGift.sprite = data.sprGiftReward;
            imgGift.SetNativeSize();
            var rec = imgGift.rectTransform.sizeDelta;
            var val = 282 / rec.y;
            imgGift.rectTransform.sizeDelta *= val;

            objectGift.Hide();
            if (isGet)
            {
                txtTitleGift.text = I2.Loc.ScriptLocalization.completed_the_task.ToUpper();
                isLoadLevel = true;
                data.IsGetQuest = true;
                Debug.Log("is get quest");
                InitData();
            }
        }
        else
        {
            imgGift.gameObject.SetActive(false);
            objectGift.ShowGift(data.sprGiftReward, "+" + data.amountReward);
            // show booster
        }
    }
    public bool isLoadLevel = false;
    public void OnClickExitPopupGift()
    {
        SoundClickButton();
        Debug.Log("isLoadLevel = " + isLoadLevel);
        if (isLoadLevel)
        {
            homeUIManager.panelLevel.Show();

            isLoadLevel = false;
            var idShape = 0;
            if (dataCurrent != null)
                idShape = int.Parse(dataCurrent.GetRewardQuest());

            var typeTopic = TypeTopic.Animal;
            var str = dataCurrent.GetTypeTopicQuest_6();
            System.Enum.TryParse(str, out typeTopic);

            DataAllShape.GetShapeInfo(idShape).IsUnlock = true;
            homeUIManager.panelLevel.ShowTopic(typeTopic, idShape);
            homeUIManager.panelLevel.LoadLevel(typeTopic, idShape);
            Hide();
        }
        else
            ActionHelper.CheckShowInter();

        panelGift.SetActive(false);
    }
    public void SetDoneAllQuest()
    {
        for (int i = 0; i < InitDataGame.instance.listDataDailyQuest.Count - 1; i++)
        {
            if (listElementDailyQuest[i].gameObject.activeSelf)
            {
                InitDataGame.instance.listDataDailyQuest[i].SetDoneQuest();
            }
        }
        InitData();
    }
}
