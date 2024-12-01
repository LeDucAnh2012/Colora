using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FrameBGInfo
{
    public int id;
    public Sprite spr;
    public TypeUnlock typeUnlock;
    public TypeElement typeElement;
    public Vector2 localPos;
    public Vector2 recPos;
    public float sizeCamShot;
    public bool IsUnlock
    {
        get
        {
            if (typeUnlock == TypeUnlock.Free)
                IsUnlock = true;
            return PlayerPrefs.GetInt(typeElement.ToString() + "Info" + id, 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt(typeElement.ToString() + "Info" + id, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
[CreateAssetMenu]
public class DataFrameBG : ScriptableObject
{
    private static readonly ResourceAsset<DataFrameBG> assets = new ResourceAsset<DataFrameBG>("Data/DataFrameBG");
    public List<FrameBGInfo> listFrame = new List<FrameBGInfo>();
    public List<FrameBGInfo> listBG = new List<FrameBGInfo>();

    public static int CountFrame => GetListFrame().Count;
    public static int CountBG => GetListBG().Count;
    public static List<FrameBGInfo> GetListFrame()
    {
        return assets.Value.listFrame;
    }
    public static List<FrameBGInfo> GetListBG()
    {
        return assets.Value.listBG;
    }

    public static FrameBGInfo GetFrameBGInfo(TypeElement type, int id)
    {
        foreach (var frame in type == TypeElement.Frame ? GetListFrame() : GetListBG())
            if (frame.id == id) return frame;

        return null;
    }


    public bool isChange = false;
    private void OnValidate()
    {
        if (!isChange) return;

        for (int i = 0; i < CountFrame; i++)
        {
            GetListFrame()[i].id = i - 1;
        }
        for (int i = 0; i < CountBG; i++)
        {
            GetListBG()[i].id = i - 1;
        }
    }
}
