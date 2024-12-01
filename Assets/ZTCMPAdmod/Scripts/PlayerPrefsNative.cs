using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace GT.Utils
{
    public static class PlayerPrefsNative
    {
        private static AndroidJavaObject _currentActivity;
        private static AndroidJavaClass _apiClass;
        private static AndroidJavaObject _apiInstance;
        
        public static string GetString(string key, string valueDefault)
        {
            try
            {
                #if UNITY_ANDROID && !UNITY_EDITOR

                            if (_apiInstance == null)
                            {
                                if (null == _apiClass)
                                    _apiClass = new AndroidJavaClass("com.gametown.utility.PlayerPrefsNativeAndroid");

                                _apiInstance = _apiClass.CallStatic<AndroidJavaObject>("instance");
                            }

                            if (null == _currentActivity)
                                _currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"); //com.unity3d.player.UnityPlayer

                            if (_apiInstance != null && _currentActivity != null)
                            {
                                string  result = _apiInstance.Call<string>("GetString", _currentActivity,key, valueDefault);
                                Debug.Log("SharePref "+ key + " : " + result);
                                return result;
                            }
                            else
                            {
                                Debug.Log("remainMemory nullllll");
                            }

                #endif
                                
                #if UNITY_IOS && !UNITY_EDITOR
                        return _getStringForKeyWithDefaultValue(key, valueDefault);
                #endif
            }
            catch (Exception ex)
            {

                Debug.LogError($"PlayerPrefsNative GetString is error: {ex.GetBaseException()}\n{ex.StackTrace}");
            }

            return valueDefault;
        }
        
        
        public static int GetInt(string key, int valueDefault)
        {
            try
            {
                #if UNITY_ANDROID && !UNITY_EDITOR

                                            if (_apiInstance == null)
                                            {
                                                if (null == _apiClass)
                                                    _apiClass = new AndroidJavaClass("com.gametown.utility.PlayerPrefsNativeAndroid");

                                                _apiInstance = _apiClass.CallStatic<AndroidJavaObject>("instance");
                                            }

                                            if (null == _currentActivity)
                                                _currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"); //com.unity3d.player.UnityPlayer

                                            if (_apiInstance != null && _currentActivity != null)
                                            {
                                                int  result = _apiInstance.Call<int>("GetInt", _currentActivity,key, valueDefault);
                                                Debug.Log("SharePref "+ key + " : " + result);
                                                return result;
                                            }
                                            else
                                            {
                                                Debug.Log("remainMemory nullllll");
                                            }

                #endif
                                                
                #if UNITY_IOS && !UNITY_EDITOR
                                        return _getIntForKeyWithDefaultValue(key, valueDefault);
                #endif
            }
            catch (Exception ex)
            {

                Debug.LogError($"PlayerPrefsNative GetInt is error: {ex.GetBaseException()}\n{ex.StackTrace}");
            }

            return valueDefault;
        }
            
        #if UNITY_IOS && !UNITY_EDITOR
            [DllImport("__Internal")]
            private static extern string _getStringForKeyWithDefaultValue(string key, string defaultValue);

             [DllImport("__Internal")]
            private static extern int _getIntForKeyWithDefaultValue(string key, int defaultValue);
        #endif
    }
}