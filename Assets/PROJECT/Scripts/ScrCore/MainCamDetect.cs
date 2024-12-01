using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamDetect : MonoBehaviour
{
    private void Start()
    {
        CanvasAllScene.instance.myCanvas.worldCamera = gameObject.GetComponent<Camera>();
    }
}
