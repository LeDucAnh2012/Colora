using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PopupGift : PanelBase
{
    [SerializeField] private Text txtTitle;
    [SerializeField] private ObjectGift objectGift;
    [SerializeField] private Transform parentSpawn;

    private List<ObjectGift> listObjectGift = new List<ObjectGift>();
    private List<Sprite> listSprite = new List<Sprite>();
    private List<string> listStr = new List<string>();
    private List<TypeBooster> listTypeBooster = new List<TypeBooster>();

    private UnityAction callback = null;
    public void ShowPopup(List<Sprite> listSpr, List<string> listStrVal, List<TypeBooster> listType)
    {
        var count = listSpr.Count - listObjectGift.Count;

        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                var ojb = Instantiate(objectGift, parentSpawn);
                listObjectGift.Add(ojb);
            }
        }
        else
        {
            foreach (var obj in listObjectGift)
                obj.Hide();
        }

        for (int i = 0; i < listSpr.Count; i++)
        {
            listObjectGift[i].ShowGift(listSpr[i], listStrVal[i]);
        }
        base.Show();
        SoundShowPopup();
        //if (gameplayUIManager.stateGame == StateGame.Home)
        //    gameplayUIManager.SetConfetti();

        DelayCallback(() =>
        {
            Hide();
        }, 2f);
    }
    public void ShowPopup(List<Sprite> listSpr, List<int> listVal, List<TypeBooster> listType, UnityAction callback = null)
    {
        this.txtTitle.text = I2.Loc.ScriptLocalization.you_will_receive_a_gift_after_a_few_seconds.ToUpper();
        this.callback = callback;
        listSprite.Clear();
        listTypeBooster.Clear();

        listSprite = listSpr;
        listTypeBooster = listType;

        listStr = new List<string>();
        foreach (var obj in listVal)
            listStr.Add("+" + obj);
        ShowPopup(listSpr, listStr, listType);
    }
    public override void Hide()
    {
        if (ActionHelper.GetSceneCurrent() == TypeSceneCurrent.GameplayScene )
            ActionHelper.CheckShowInter((bool isShowCompleted) =>
            {
                canvasAllScene.popupGetGift.ShowPopup(listSprite, listStr, listTypeBooster, callback, isShowCompleted);
                base.Hide();
            }, false);
        else
        {
            canvasAllScene.popupGetGift.ShowPopup(listSprite, listStr, listTypeBooster, callback);
            base.Hide();
        }

    }
}
