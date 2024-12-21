using Firebase;
using Firebase.Analytics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CC_LogFirebase : MonoBehaviour
{
    public static CC_LogFirebase instance;
    private bool firebaseInitialized = false;

    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;

    private void Awake()
    {
        instance = this;
      
    }
    private void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebaseLogEvent();
            }
            else
            {
                Debug.Log ("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }
    public void InitializeFirebaseLogEvent()
    {
        Debug.Log("init firebase = " + firebaseInitialized);
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
        // Set the user ID.
        FirebaseAnalytics.SetUserId(SystemInfo.deviceUniqueIdentifier);
        // Set default session duration values.
        //FirebaseAnalytics.SetMinimumSessionDuration(new TimeSpan(0, 0, 10));
        FirebaseAnalytics.SetSessionTimeoutDuration(new TimeSpan(0, 30, 0));
        firebaseInitialized = true;

        RemoteConfig.instance.CheckRemoteConfigValues();
    }
    public void LogEventWithString(string eventName)
    {
        if (firebaseInitialized)
            FirebaseAnalytics.LogEvent(eventName);
    }
}
