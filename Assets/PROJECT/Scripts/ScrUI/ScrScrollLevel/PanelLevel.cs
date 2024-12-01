using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PanelLevel : PanelBase
{
    [SerializeField] private ScrollTopic scrollTopicSpawn;
    [SerializeField] private RectTransform parentSpawn;
    [SerializeField] private List<ScrollTopic> listScrollTopicSpawn = new List<ScrollTopic>();

    [SerializeField] private ElementTab elementTabSpawn;
    [SerializeField] private RectTransform parentTab;
    [SerializeField] private List<ElementTab> listElementTab = new List<ElementTab>();

    [HideInInspector] public ScrollTopic scrollTopicCurrent;
    [HideInInspector] public ElementTab elementTabCurrent;

    [HideInInspector] public int CountInProgress = 0;
    [HideInInspector] public int CountFinish = 0;

    private int indexTabCurrent = 0;
    public override void Show()
    {
        base.Show();
    }
    public void LoadData()
    {
        IE_LOAD_DATA = IE_LoadData();
        StartCoroutine(IE_LOAD_DATA);
    }
    private IEnumerator IE_LOAD_DATA = null;
    private IEnumerator IE_LoadData()
    {

        yield return new WaitForEndOfFrame();
        int count = 0;
        foreach (var keyPair in  InitDataGame.instance.GetDicListShapeInfo())
        {
            var scr = Instantiate(scrollTopicSpawn, parentSpawn);
            listScrollTopicSpawn.Add(scr);
            scr.topicInfo = new(keyPair.Key,keyPair.Value);
            scr.GetComponent<RectTransform>().anchoredPosition = new Vector2(count * 1080 + 540, scr.GetComponent<RectTransform>().anchoredPosition.y);
            count++;
            // scr.transform.position = new Vector2(0, -2.1f);
            scr.Hide();
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(LoadDataScrollTopic());
        listScrollTopicSpawn[0].Show();
        scrollTopicCurrent = listScrollTopicSpawn[0];

        foreach (var keyPair in InitDataGame.instance.GetDicListShapeInfo())
        {
            var scr = Instantiate(elementTabSpawn, parentTab);
            listElementTab.Add(scr);
            scr.typeTopic = keyPair.Key;
            yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i < listElementTab.Count; i++)
            listElementTab[i].LoadData(i, listScrollTopicSpawn[i]);

        elementTabCurrent = listElementTab[0];
        elementTabCurrent.SetColor(true);
        InitDataGame.instance.ClearDicShape();
    }
    private int count = 0;
    private IEnumerator LoadDataScrollTopic()
    {
        for (int i = 0; i < listScrollTopicSpawn.Count; i++)
        {
            bool isDone = false;
            listScrollTopicSpawn[i].LoadData(ref isDone);
            yield return new WaitUntil(() => isDone == true);
        }
    }
    public void LoadLevel(TypeTopic type, int idShape)
    {
        foreach (var scr in listScrollTopicSpawn)
        {
            if (scr.typeTopic == type)
            {
                scr.LoadLevel(idShape);
                return;
            }
        }
    }

    public void ShowTopic(TypeTopic type, int idShape)
    {
        foreach (var scr in listScrollTopicSpawn)
        {
            if (scr.typeTopic == type)
            {
                scr.Show();
                scr.transform.GetChild(0).gameObject.SetActive(true);
                scr.ShowLevel(idShape);
                return;
            }
        }
    }
    public void CheckOffTopic()
    {
        foreach (var scr in listScrollTopicSpawn)
            scr.CheckOffTopic();
    }
    public void RemoveScrollTopic(ScrollTopic scr)
    {
        if (listScrollTopicSpawn.Contains(scr))
            listScrollTopicSpawn.Remove(scr);
    }

    [Button]
    void SetScale()
    {
        foreach (var scroll in listScrollTopicSpawn)
            scroll.SetScale();
    }
    public void ActiveTopic()
    {
        return;
        //foreach (var scr in listScrollTopicSpawn)
        //{
        //    var check = scr.transform.position.y > 55 || scr.transform.position.y < (scr.isLoaded ? -44 : -116);
        //    scr.transform.GetChild(0).gameObject.SetActive(!check);
        //    scr.ActivePic();
        //    if (!check)
        //    {
        //        if (!scr.isLoaded)
        //            scr.LoadData();
        //    }
        //}
    }
    public void SetParentShape(Transform parentInProgress, Transform parentFinish)
    {
        CountInProgress = 0;
        CountFinish = 0;
        foreach (var ele in listScrollTopicSpawn)
        {
            ele.SetParentShape(parentInProgress, parentFinish, ref CountInProgress, ref CountFinish);
        }
    }
    public void ResetParentShape()
    {
        foreach (var ele in listScrollTopicSpawn)
            ele.ResetParentShape();
    }



    public void SetAnchorTab(int indexTab)
    {
        var count = listElementTab.Count - 2;

        var valMove = parentTab.sizeDelta.x / count;
        parentTab.DOAnchorPosX(indexTab * valMove * -1, 0.5f).SetEase(Ease.OutSine);
    }
    public void SetAnchorScroll(int indexTab)
    {
        var count = listScrollTopicSpawn.Count - 2;

        if (indexTabCurrent > indexTab)
        {
            for (int i = indexTab; i <= indexTabCurrent; i++)
            {
                listScrollTopicSpawn[i].Show();
            }
        }
        else if (indexTabCurrent < indexTab)
        {
            for (int i = indexTabCurrent; i <= indexTab; i++)
            {
                listScrollTopicSpawn[i].Show();
            }
        }
        else
        {
            return;
        }

        parentSpawn.DOAnchorPosX(indexTab * -1080, 0.5f).SetEase(Ease.OutSine).OnComplete(() =>
        {
            if (indexTabCurrent > indexTab)
            {
                for (int i = indexTab + 1; i <= indexTabCurrent; i++)
                {
                    listScrollTopicSpawn[i].Hide();
                }
            }
            else if (indexTabCurrent < indexTab)
            {
                for (int i = indexTabCurrent; i < indexTab; i++)
                {
                    listScrollTopicSpawn[i].Hide();
                }
            }
            indexTabCurrent = indexTab;
        });
    }
}
