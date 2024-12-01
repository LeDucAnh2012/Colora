using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeTutorial : MonoBehaviour
{
    public int id;
    [SerializeField] Image imgColor;
    [SerializeField] Color col;
    [SerializeField] Sprite sprColor;
    [SerializeField] Sprite sprNum;

    public void SetColor()
    {
        imgColor.sprite = sprColor;
        imgColor.color = col;
    }
    public void ResetColor()
    {
        imgColor.color = Color.white;
        imgColor.sprite = sprNum;
    }
    public void SetImage()
    {
        imgColor = GetComponent<Image>();
    }
}
