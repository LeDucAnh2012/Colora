using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectGift : PanelBase
{
    public Text txtVal;
    public Image imgGift;

    public void ShowGift(Sprite sprGift, string valGift, float posX = 0)
    {
        imgGift.sprite = sprGift;
        txtVal.text = valGift;
        imgGift.SetNativeSize();

        for (int i = 0; i < transform.childCount; i++)
        {
            var pos = transform.GetChild(i).gameObject.GetComponent<RectTransform>().anchoredPosition;
            transform.GetChild(i).gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, pos.y);
        }
        base.Show();
    }
}
