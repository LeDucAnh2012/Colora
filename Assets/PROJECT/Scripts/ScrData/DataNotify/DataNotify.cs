using I2.Loc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class NotifyConfig
{
    public int idDay;
    public List<InfoNotify> listInfoNoti = new List<InfoNotify>();

    public NotifyConfig(int idDay, List<InfoNotify> listInfoNoti)
    {
        this.idDay = idDay;
        this.listInfoNoti = listInfoNoti;
    }
}

[System.Serializable]
public class InfoNotify
{
    public float time;
    public string strTitle;
    public string strText;

    public InfoNotify(float time, string strTitle, string strText)
    {
        this.time = time;
        this.strTitle = strTitle;
        this.strText = strText;
    }
}


[CreateAssetMenu]
public class DataNotify : ScriptableObject
{
    private readonly static ResourceAsset<DataNotify> asset = new ResourceAsset<DataNotify>("Data/DataNotify");
    public List<NotifyConfig> listNotiConfig = new List<NotifyConfig>();

    public static int Count => GetListNotify().Count;
    public static List<NotifyConfig> GetListNotify()
    {
        return asset.Value.listNotiConfig;
    }
}
