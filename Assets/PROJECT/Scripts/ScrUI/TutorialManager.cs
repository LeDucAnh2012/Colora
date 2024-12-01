using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TutorialManager : PanelBase
{
    [SerializeField] private GameObject objTut_1;
    [SerializeField] private GameObject objTut_2;
    [SerializeField] private GameObject objTut_3;

    [TabGroup("1", "Tut_1")][SerializeField] private List<ShapeTutorial> listShape = new List<ShapeTutorial>();
    [TabGroup("1", "Tut_1")][SerializeField] private List<Transform> listPoint = new List<Transform>();
    [TabGroup("1", "Tut_1")][SerializeField] private Image imgHand;
    [TabGroup("1", "Tut_1")][SerializeField] private Image imgNum_1;
    [TabGroup("1", "Tut_1")][SerializeField] private Color colPress;

    [TabGroup("1", "Tut_3")][SerializeField] private List<ShapeTutorial> listShapeFillNum = new List<ShapeTutorial>();
    [TabGroup("1", "Tut_3")][SerializeField] private List<ShapeTutorial> listShapeFillBomb = new List<ShapeTutorial>();
    [TabGroup("1", "Tut_3")][SerializeField] private Image imgFrameBomb;
    [TabGroup("1", "Tut_3")][SerializeField] private Image imgIconBomb;
    [TabGroup("1", "Tut_3")][SerializeField] private Image imgFrameFillNum;
    [TabGroup("1", "Tut_3")][SerializeField] private Image imgIconFillNum;
    [TabGroup("1", "Tut_3")][SerializeField] private Image imgHandTut_3;

    public bool isTut_1 = true;
    public int count = 0;
    public float timeMove = 0.1f;

    private int indexTut = 0;
    public void ShowTut()
    {
        base.Show();
        isTut_1 = true;
        StartCoroutine(MoveHand());
    }
    private void Update()
    {
        Tut_1();
    }
    #region Tut_1
    public void Tut_1()
    {
        if (!isTut_1) return;

        RaycastHit2D hit = Physics2D.Raycast(imgHand.transform.position, Vector2.down, 0.01f, LayerMask.GetMask("ShapeTutorial"));
        if (hit.collider != null)
        {
            var shape = hit.collider.gameObject.GetComponent<ShapeTutorial>();
            if (shape != null && shape.id == 1)
            {
                shape.SetColor();
            }
        }
    }
    public IEnumerator MoveHand()
    {
        yield return new WaitForSeconds(0.2f);
        imgHand.transform.position = imgNum_1.transform.position;
        foreach (var shape in listShape)
            shape.ResetColor();

        imgNum_1.DOColor(colPress, 0.2f).From(Color.white).SetEase(Ease.Linear);
        imgNum_1.transform.DOScale(0.9f, 0.2f).From(1).SetEase(Ease.Linear);

        imgHand.transform.DOScale(0.9f, 0.2f).From(1).SetEase(Ease.Linear).OnComplete(() =>
        {
            imgNum_1.DOColor(Color.white, 0.2f).From(colPress).SetEase(Ease.Linear);
            imgNum_1.transform.DOScale(1, 0.2f).From(0.9f).SetEase(Ease.Linear);

            imgHand.transform.DOScale(1, 0.2f).From(0.9f).SetEase(Ease.Linear).OnComplete(() =>
            {
                StartCoroutine(IE_Move());
            });
        });
    }

    private IEnumerator IE_Move()
    {
        yield return new WaitForSeconds(0.2f);
        imgHand.transform.position = listPoint[count++].position;
        Move();
    }
    private void Move()
    {
        if (count >= listPoint.Count)
        {
            StartCoroutine(ActionHelper.StartAction(() =>
            {
                StopCoroutine(IE_Move());
                count = 0;
                StartCoroutine(MoveHand());
            }, 1));
            return;
        }

        imgHand.transform.DOMove(listPoint[count++].position, timeMove).SetEase(Ease.Linear).OnComplete(() =>
        {
            Move();
        });
    }
    #endregion

    #region Tut_3
    private IEnumerator Step1_Tut_3()
    {
        yield return new WaitForSeconds(0.5f);
        imgHandTut_3.transform.position = imgFrameFillNum.transform.position;
        foreach (var shape in listShapeFillBomb)
            shape.ResetColor();

        foreach (var shape in listShapeFillNum)
            shape.ResetColor();

        ClickBooster(imgFrameFillNum, imgIconFillNum, () =>
        {
            Move_Tut3();
        });
    }
    private void Move_Tut3()
    {
        imgHandTut_3.transform.DOMove(listShapeFillNum[0].transform.position, 1).SetEase(Ease.Linear).OnComplete(() =>
        {
            imgHandTut_3.transform.DOScale(0.9f, 0.2f).From(1).SetEase(Ease.Linear).OnComplete(() =>
            {
                imgHandTut_3.transform.DOScale(1, 0.2f).From(0.9f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    foreach (var shape in listShapeFillNum)
                        shape.SetColor();
                    StartCoroutine(Step2_Tut_3());
                });
            });
        });
    }
    private IEnumerator Step2_Tut_3()
    {
        yield return new WaitForSeconds(1);
        imgHandTut_3.transform.position = imgFrameBomb.transform.position;
        ClickBooster(imgFrameBomb, imgIconBomb, () =>
        {
            Move_Tut3_1();
        });
    }
    private void Move_Tut3_1()
    {
        imgHandTut_3.transform.DOMove(listShapeFillBomb[0].transform.position, 1).SetEase(Ease.Linear).OnComplete(() =>
        {
            imgHandTut_3.transform.DOScale(0.9f, 0.2f).From(1).SetEase(Ease.Linear).OnComplete(() =>
            {
                imgHandTut_3.transform.DOScale(1, 0.2f).From(0.9f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    foreach (var shape in listShapeFillBomb)
                        shape.SetColor();
                    StartCoroutine(Step1_Tut_3());
                });
            });
        });
    }
    private void ClickBooster(Image img1, Image img2, UnityAction callback)
    {
        img1.DOColor(colPress, 0.2f).From(Color.white).SetEase(Ease.Linear);
        img1.transform.DOScale(0.9f, 0.2f).From(1).SetEase(Ease.Linear);

        img2.DOColor(colPress, 0.2f).From(Color.white).SetEase(Ease.Linear);
        img2.transform.DOScale(0.9f, 0.2f).From(1).SetEase(Ease.Linear);

        imgHandTut_3.transform.DOScale(0.9f, 0.2f).From(1).SetEase(Ease.Linear).OnComplete(() =>
        {
            img1.DOColor(Color.white, 0.2f).From(colPress).SetEase(Ease.Linear);
            img1.transform.DOScale(1, 0.2f).From(0.9f).SetEase(Ease.Linear);

            img2.DOColor(Color.white, 0.2f).From(colPress).SetEase(Ease.Linear);
            img2.transform.DOScale(1, 0.2f).From(0.9f).SetEase(Ease.Linear);

            imgHandTut_3.transform.DOScale(1, 0.2f).From(0.9f).SetEase(Ease.Linear).OnComplete(() =>
            {
                callback?.Invoke();
            });
        });
    }

    #endregion
    public void OnClickContinue()
    {
        SoundClickButton();
        switch (indexTut)
        {
            case 0:
                ActionHelper.LogEvent(KeyLogFirebase.TutorialStep + "1");
                StopCoroutine(IE_Move());
                objTut_1.SetActive(false);
                objTut_2.SetActive(true);
                isTut_1 = false;
                break;

            case 1:
                ActionHelper.LogEvent(KeyLogFirebase.TutorialStep + "2");
                StopAllCoroutines();
                objTut_2.SetActive(false);
                objTut_3.SetActive(true);
                isTut_1 = false;
                StartCoroutine(Step1_Tut_3());
                break;
            case 2:
                ActionHelper.LogEvent(KeyLogFirebase.TutorialStep + "3");
                StopAllCoroutines();
                Destroy(gameObject);
                break;
        }
        indexTut++;
    }

    [Button]
    void SetImage()
    {
        foreach (var shape in listShapeFillNum)
        {
            shape.SetImage();
        }
        foreach (var shape in listShapeFillBomb)
        {
            shape.SetImage();
        }
    }
}
