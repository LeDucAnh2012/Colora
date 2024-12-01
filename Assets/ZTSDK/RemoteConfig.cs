using Firebase.Analytics;
using Firebase;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using System;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class ConfigData
{
    public int TimeIndexAoaInter = 10;
    public int TimeIndexAoaReward = 10;
    public int TimeIndexAoa = 10;
    public int TimeIndexInter = 20;
    public int TimeIndexInterReward = 10;
    public int IndexShowInter = 1;
    public int DistancePaint = 1;
    public int TimeShowBuyBooster = 15;
    public int TimeOfExistence = 5;
    public int TimeCallBackShowBooster = 30;

    public bool BannerCollapInLoading = true;
    public bool BannerCollapInBackHome = true;
    public bool BannerCollapInFrameBG = true;
    public bool BannerCollapInGameplay = false;
    public bool IsDebug = false;

    public bool ShowAOA = true;

    public string SpeedZoom = "8";

    public long TimeVibration = 150;
}

public class RemoteConfig : MonoBehaviour
{
    public static RemoteConfig instance;
    public ConfigData allConfigData;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        CheckRemoteConfigValues();
    }
    private void SetValue(FirebaseRemoteConfig remoteConfig)
    {
        allConfigData.TimeIndexAoaInter = (int)remoteConfig.GetValue("TimeIndexAoaInter").LongValue;
        allConfigData.TimeIndexAoa = (int)remoteConfig.GetValue("TimeIndexAoa").LongValue;
        allConfigData.TimeIndexInter = (int)remoteConfig.GetValue("TimeIndexInter").LongValue;
        allConfigData.TimeIndexInterReward = (int)remoteConfig.GetValue("TimeIndexInterReward").LongValue;
        allConfigData.TimeIndexAoaReward = (int)remoteConfig.GetValue("TimeIndexAoaReward").LongValue;
        allConfigData.IndexShowInter = (int)remoteConfig.GetValue("IndexShowInter").LongValue;
        allConfigData.DistancePaint = (int)remoteConfig.GetValue("DistancePaint").LongValue;
        allConfigData.TimeVibration = (int)remoteConfig.GetValue("TimeVibration").LongValue;
        allConfigData.TimeShowBuyBooster = (int)remoteConfig.GetValue("TimeShowBuyBooster").LongValue;
        allConfigData.TimeOfExistence = (int)remoteConfig.GetValue("TimeOfExistence").LongValue;
        allConfigData.TimeCallBackShowBooster = (int)remoteConfig.GetValue("TimeCallBackShowBooster").LongValue;

        allConfigData.BannerCollapInLoading = remoteConfig.GetValue("BannerCollapInLoading").BooleanValue;
        allConfigData.BannerCollapInBackHome = remoteConfig.GetValue("BannerCollapInBackHome").BooleanValue;
        allConfigData.BannerCollapInFrameBG = remoteConfig.GetValue("BannerCollapInFrameBG").BooleanValue;
        allConfigData.BannerCollapInGameplay = remoteConfig.GetValue("BannerCollapInGameplay").BooleanValue;
        allConfigData.ShowAOA = remoteConfig.GetValue("ShowAOA").BooleanValue;
        allConfigData.IsDebug = remoteConfig.GetValue("IsDebug").BooleanValue;

        allConfigData.SpeedZoom = remoteConfig.GetValue("SpeedZoom").StringValue;
    }
    public Task CheckRemoteConfigValues()
    {
        Debug.Log("Fetching data...");

        Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
        return fetchTask.ContinueWithOnMainThread(FetchComplete);
    }
    private void FetchComplete(Task fetchTask)
    {
        Debug.Log("FetchComplete");
        if (!fetchTask.IsCompleted)
        {
            Debug.LogError("Retrieval hasn't finished.");
            return;
        }

        var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
        var info = remoteConfig.Info;
        if (info.LastFetchStatus != LastFetchStatus.Success)
        {
            //Debug.LogError($"{nameof(FetchComplete)} was unsuccessful\n{nameof(info.LastFetchStatus)}: {info.LastFetchStatus}");
            return;
        }

        // Fetch successful. Parameter values must be activated to use.
        remoteConfig.ActivateAsync()
          .ContinueWithOnMainThread(
            task =>
            {
                // Debug.Log($"Remote data loaded and ready for use. Last fetch time {info.FetchTime}.");

                SetValue(remoteConfig);
                AdsConfig.instance.Init();

                //  print("Total values: " + remoteConfig.AllValues.Count);

                //foreach (var item in remoteConfig.AllValues)
                //{
                //    print("Key :" + item.Key);
                //    print("Value: " + item.Value.StringValue);
                //}
            });
    }
}