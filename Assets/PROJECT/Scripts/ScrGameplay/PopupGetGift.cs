using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class PopupGetGift : PanelBase
{
    [SerializeField] private ObjectGift objectGift;
    [SerializeField] private Transform parentSpawn;
    private List<ObjectGift> listObjectGift = new List<ObjectGift>();
    private List<TypeBooster> listType = new List<TypeBooster>();
    private UnityAction callback = null;
    private bool isShowPopRemoveAds = false;

    private Transform parentNumBooster;
    private Transform parentBombBooster;
    private Transform parentFindBooster;
    private bool isMoveGift = true;
    public void ShowPopup(List<Sprite> listSpr, List<int> listVal, List<TypeBooster> listType, UnityAction callback, bool isShowPopRemoveAds = false, bool isMoveGift = true)
    {
        var listStr = new List<string>();
        for (int i = 0; i < listVal.Count; i++)
            listStr.Add("+" + listVal[i]);
        ShowPopup(listSpr, listStr, listType, callback, isShowPopRemoveAds, isMoveGift);
    }
    public void ShowPopup(List<Sprite> listSpr, List<string> listStrVal, List<TypeBooster> listType, UnityAction callback, bool isShowPopRemoveAds = false, bool isMoveGift = true)
    {
        base.Show();
        this.isMoveGift = isMoveGift;
        if (ActionHelper.GetSceneCurrent() == TypeSceneCurrent.GameplayScene)
        {
            parentNumBooster = gameplayUIManager.boosterManager.fillByNum.transform;
            parentBombBooster = gameplayUIManager.boosterManager.fillByBomb.transform;
            parentFindBooster = gameplayUIManager.boosterManager.findBooster.transform;
        }

        SoundShowPopup();
        this.callback = callback;
        this.isShowPopRemoveAds = isShowPopRemoveAds;
        listObjectGift.Clear();
        ActionHelper.Clear(parentSpawn);

        this.listType = new List<TypeBooster>();
        for (int i = 0; i < listSpr.Count; i++)
        {
            var obj = Instantiate(objectGift, parentSpawn);
            if (i % 2 == 0 && listSpr.Count % 2 == 1 && listSpr.Count > 1 && i > 1)
                obj.ShowGift(DataAllShape.GetDataBooster(listType[i]).sprBooster, listStrVal[i], 240);
            else
                obj.ShowGift(DataAllShape.GetDataBooster(listType[i]).sprBooster, listStrVal[i]);

            listObjectGift.Add(obj);
            this.listType.Add(listType[i]);
        }

        if (ActionHelper.GetSceneCurrent() == TypeSceneCurrent.GameplayScene)
            StartCoroutine(IE_Move(listType, false));
        else
            Invoke(nameof(Hide), 1.5f);
    }
    private IEnumerator IE_Move(List<TypeBooster> listType, bool isDelay)
    {
        if (isDelay)
            yield return new WaitForSeconds(1.5f);
        if (isMoveGift)
        {
            for (int i = 0; i < listObjectGift.Count; i++)
            {
                var t = listObjectGift[i];
                t.txtVal.gameObject.SetActive(false);
                switch (listType[i])
                {
                    case TypeBooster.Find:
                        t.transform.DOMove(parentFindBooster.position, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            Destroy(t.gameObject);
                        });
                        break;
                    case TypeBooster.Bomb:
                        t.transform.DOMove(parentBombBooster.position, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            Destroy(t.gameObject);
                        });
                        break;
                    case TypeBooster.Number:
                        t.transform.DOMove(parentNumBooster.position, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            Destroy(t.gameObject);
                        });
                        break;
                }
            }
            yield return new WaitForSeconds(0.75f);
        }

        if (ActionHelper.GetSceneCurrent() == TypeSceneCurrent.GameplayScene)
        {
            if (isShowPopRemoveAds && VariableSystem.IsCanShowInter)
            {
                if (gameplayUIManager.progressPainting.CheckDoneGift_2())
                    gameplayUIManager.popupRemoveAds.Show();
            }
            gameplayUIManager.boosterManager.UpdateUI();
        }
        Hide();
    }
    public override void Hide()
    {
        callback?.Invoke();
        base.Hide();
    }
}
