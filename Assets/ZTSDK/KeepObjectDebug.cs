using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepObjectDebug : MonoBehaviour
{
    public static KeepObjectDebug instance;
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        if (KeepObject.instance.mode == TypeMode.Development)
        {
            gameObject.SetActive(true);
            VariableSystem.IsDebug = true;
        }
        else
        {
            gameObject.SetActive(false);
            VariableSystem.IsDebug = false;
        }
    }
}
