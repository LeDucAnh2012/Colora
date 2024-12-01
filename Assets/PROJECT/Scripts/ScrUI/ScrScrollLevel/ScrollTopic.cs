using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScrollTopic : PanelBase
{
    [SerializeField] private ElementLevel elementLevelSpawn;
    [SerializeField] private Transform parentSpawn;
    [SerializeField] private List<ElementLevel> listElementLevelSpawn = new List<ElementLevel>();

    [HideInInspector] public TypeTopic typeTopic;
    [HideInInspector] public TopicInfo topicInfo;
    [HideInInspector] public bool isLoaded = false;

    UnityAction<bool> callback;
    public override void Show()
    {
        //if (!isLoaded)
        //{
        //    canvasAllScene.objLoading.SetActive(true);
        //    LoadData();
        //}
        base.Show();
    }
    //public void LoadData(UnityAction<bool> callback)
    //{
    //    this.callback = callback;
    //    LoadData();
    //}
    public void LoadData(ref bool isDone)
    {
        isLoaded = true;
        typeTopic = topicInfo.typeTopic;

        var size = GetComponent<RectTransform>().sizeDelta;

        var count = 0;
        foreach (var shape in topicInfo.listShapeInfo)
        {
            var ele = Instantiate(elementLevelSpawn, parentSpawn.transform);
            ele.shapeInfo = shape;

            listElementLevelSpawn.Add(ele);
            ele.LoadData(shape);

            ele.indexSibling = count;
            count++;
        }

        if (listElementLevelSpawn.Count == 0)
        {
            homeUIManager.panelLevel.RemoveScrollTopic(this);
            Destroy(gameObject);
        }

        CheckOffTopic();
        canvasAllScene.objLoading.Hide();
        isDone = true;
    }
    [Button]
    private void ShowPos()
    {
        Debug.Log("pos = " + transform.position);
    }
    public void CheckOffTopic()
    {
        bool isOff = true;
        foreach (var el in listElementLevelSpawn)
            if (el.gameObject.activeSelf)
            {
                isOff = false;
                break;
            }

        if (isOff)
            gameObject.SetActive(false);
    }

    public void LoadLevel(int id)
    {
        foreach (var ele in listElementLevelSpawn)
        {
            if (ele.idShape == id)
            {
                ele.OnClickChoose();
                return;
            }
        }
    }
    public void ShowLevel(int id)
    {
        foreach (var ele in listElementLevelSpawn)
        {
            if (ele.idShape == id)
            {
                ele.Show();
                return;
            }
        }
    }
    public void SetScale()
    {
        foreach (var shape in listElementLevelSpawn)
            shape.SetScale();
    }
    public void ActivePic()
    {
        if (listElementLevelSpawn.Count < 7)
        {
            foreach (var shape in listElementLevelSpawn)
                if (!shape.isLoaded)
                    shape.LoadData(shape.shapeInfo);
            return;
        }

        foreach (var shape in listElementLevelSpawn)
        {
            var check = shape.transform.position.y > 55 || shape.transform.position.y < -44;
            shape.transform.GetChild(0).gameObject.SetActive(!check);

            if (!check)
                if (!shape.isLoaded)
                    shape.LoadData(shape.shapeInfo);
        }
    }
    public void SetParentShape(Transform parentInProgress, Transform parentFinish, ref int countInProgress, ref int countFinish)
    {
        foreach (var ele in listElementLevelSpawn)
        {
            if (ele.shapeInfo.StateDone == StateDone.InProgress)
            {
                homeUIManager.panelMyWork.listInProgress.Add(ele);
                countInProgress++;
                ele.transform.SetParent(parentInProgress);
            }
            if (ele.shapeInfo.StateDone == StateDone.Done)
            {
                homeUIManager.panelMyWork.listFinished.Add(ele);
                countFinish++;
                ele.transform.SetParent(parentFinish);
            }
        }
    }
    public void ResetParentShape()
    {
        foreach (var ele in listElementLevelSpawn)
        {
            ele.transform.SetParent(parentSpawn.transform);
            ele.transform.SetSiblingIndex(ele.indexSibling);
        }
    }
}
