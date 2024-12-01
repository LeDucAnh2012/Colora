using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;
    public bool IsReloadAds;
    public bool isLoaded = false;

    private static bool isReady = false;
    private bool isHasInternet = false;
    public bool IsHasInternet
    {
        get
        {
            //if (!isHasInternet)
            //StartCoroutine(IE_CheckInternet());
            return Application.internetReachability != NetworkReachability.NotReachable;
        }
        set { isHasInternet = value; }
    }

    private DateTime localDateTime;
    private DateTime utcDateTime;
    private int dayOfYear;
    struct ServerDataTime
    {
        public string datetime;
        public string utc_datetime;
        public int day_of_year;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            isReady = true;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (!isReady)
            {
                Destroy(gameObject);
            }
        }
    }
    public DateTime GetServerTime()
    {
        return localDateTime.AddSeconds(Time.realtimeSinceStartup);
    }
    public DateTime GetServerTimeUTC()
    {
        return utcDateTime.AddSeconds(Time.realtimeSinceStartup);
    }
    public int GetDayOfYear()
    {
        return isLoaded ? dayOfYear : DateTime.Now.DayOfYear;
    }
    private void Start()
    {
        StartCoroutine(GetDateTimeFromServer());
    }
    private IEnumerator GetDateTimeFromServer()
    {
        UnityWebRequest request = UnityWebRequest.Get("https://worldtimeapi.org/api/ip");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            ServerDataTime serverDateTime = JsonUtility.FromJson<ServerDataTime>(request.downloadHandler.text);
            localDateTime = ParseToDateTime(serverDateTime.datetime);
            utcDateTime = ParseToDateTime(serverDateTime.utc_datetime);
            dayOfYear = serverDateTime.day_of_year;
            isLoaded = true;
        }
        else
        {
            Debug.Log("failed to load datetime from server with error " + request.result.ToString());
        }
    }
    private DateTime ParseToDateTime(string value)
    {
        string date = Regex.Match(value, @"^\d{4}-\d{2}-\d{2}").Value;
        string time = Regex.Match(value, @"\d{2}:\d{2}:\d{2}").Value;
        return DateTime.Parse(string.Format("{0} {1}", date, time));
    }

    private IEnumerator IE_CheckInternet()
    {
        UnityWebRequest request = UnityWebRequest.Get("https://worldtimeapi.org/api/ip");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            IsHasInternet = true;
        }
        else
        {
            Debug.Log("failed to load datetime from server with error " + request.result.ToString());
        }
    }

    
}
