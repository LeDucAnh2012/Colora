using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class DataCountTime
{
    public bool isCountTime = false;
    public float time = 10;

    public DataCountTime(bool isCountTime, float time)
    {
        this.isCountTime = isCountTime;
        this.time = time;
    }
}
public class AdsConfig : MonoBehaviour
{
    public static AdsConfig instance;

    private DataCountTime countTimeAoaInter = new DataCountTime(false, 10);
    private DataCountTime countTimeAoaReward = new DataCountTime(false, 10);
    private DataCountTime countTimeAoa = new DataCountTime(false, 10);
    private DataCountTime countTimeInter = new DataCountTime(false, 20);
    public DataCountTime countTimeInterReward = new DataCountTime(false, 10);

    private ConfigData configData = null;

    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        if (configData == null)
            return;

        CountTime(countTimeAoaInter, configData.TimeIndexAoaInter);
        CountTime(countTimeAoaReward, configData.TimeIndexAoaReward);
        CountTime(countTimeAoa, configData.TimeIndexAoa);
        CountTime(countTimeInter, configData.TimeIndexInter);
        CountTime(countTimeInterReward, configData.TimeIndexInterReward);
    }
    private void CountTime(DataCountTime dataCountTime, int timeCompare)
    {
        if (dataCountTime.isCountTime)
        {
            dataCountTime.time += Time.deltaTime;
            if (dataCountTime.time >= timeCompare)
                dataCountTime.isCountTime = false;
        }
    }
    public void Init()
    {
        configData = RemoteConfig.instance.allConfigData;

        countTimeAoaInter.time = configData.TimeIndexAoaInter;
        countTimeAoaReward.time = configData.TimeIndexAoaReward;
        countTimeAoa.time = configData.TimeIndexAoa;
        countTimeInter.time = configData.TimeIndexInter;
        countTimeInterReward.time = configData.TimeIndexInterReward;
    }
    public void CheckShowInter(Action<bool> callback = null)
    {

        if (configData != null)
        {
            if (countTimeInterReward.time < configData.TimeIndexInterReward)
            {
                Debug.Log("return countTimeInterReward");
                callback?.Invoke(false);
                return;
            }

            if (countTimeInter.time < configData.TimeIndexInter)
            {
                Debug.Log("return countTimeInter = " + configData.TimeIndexInter);
                callback?.Invoke(false);
                return;
            }
        }
        countTimeInter.time = 0;
        countTimeInter.isCountTime = true;

        countTimeAoaInter.time = 0;
        countTimeAoaInter.isCountTime = true;
        Debug.Log("configData.TimeIndexInter = " + configData.TimeIndexInter);
        CC_Interface.instance.ShowInter(callback);
    }

    public void CheckShowAoa(UnityAction callback = null)
    {
        if (VariableSystem.RemoveAds || VariableSystem.RemoveAdsHack || !RemoteConfig.instance.allConfigData.ShowAOA)
        {
            Debug.Log("AOA return remove ads");
            callback?.Invoke();
            return;
        }

        if (configData != null)
        {
            if (countTimeAoaInter.time < configData.TimeIndexAoaInter)
            {
                Debug.Log("AOA return countTimeAoaInter");
                callback?.Invoke();
                return;
            }

            if (countTimeAoa.time < configData.TimeIndexAoa)
            {
                Debug.Log("AOA return countTimeAoa");
                callback?.Invoke();
                return;
            }

            if (countTimeAoaReward.time < configData.TimeIndexAoaReward)
            {
                Debug.Log("AOA return countTimeAoaReward");
                callback?.Invoke();
                return;
            }
        }

        countTimeAoa.time = 0;
        countTimeAoa.isCountTime = true;

        CC_Interface.instance.ShowAppOpenAd(callback);
    }

    public void ShowBannerCollapseNow(bool isShowLoading, UnityAction callbackLoad)
    {
        CC_BannerCollapse.instance.isShowLoading = isShowLoading;
        CC_BannerCollapse.instance.LoadAd(callbackLoad);
    }
}
