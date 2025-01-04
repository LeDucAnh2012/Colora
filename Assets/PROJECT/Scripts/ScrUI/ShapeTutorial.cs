using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeTutorial : MonoBehaviour
{
    public int id;
    [SerializeField] Image imgColor;
    [SerializeField] Color col;

    public void SetColor()
    {
        imgColor.color = col;
    }
    public void ResetColor()
    {
        imgColor.color = Color.white;
    }
    public void SetImage()
    {
        imgColor = GetComponent<Image>();
    }
}
