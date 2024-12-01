package com.gametown.utility;

import android.app.Activity;
import android.content.Context;
import android.content.SharedPreferences;
import android.preference.PreferenceManager;

public class PlayerPrefsNativeAndroid {

    private static PlayerPrefsNativeAndroid _instance;
    public static PlayerPrefsNativeAndroid instance() {
        if (null == _instance)
            _instance = new PlayerPrefsNativeAndroid();
        return _instance;
    }

    public String GetString(Object object, String key, String defaultValue) {
        Context context = (Context)object;
        if (context != null) {
            try {
                SharedPreferences sharedPreferences = PreferenceManager.getDefaultSharedPreferences(context);
                        return sharedPreferences.getString(key, defaultValue);
            } catch (Exception ex) {
                System.out.println(ex.getMessage());
                return defaultValue;
            }
        }
        return defaultValue;
    }
    
    public int GetInt(Object object, String key, int defaultValue) {
            Context context = (Context)object;
            if (context != null) {
                try {
                    SharedPreferences sharedPreferences = PreferenceManager.getDefaultSharedPreferences(context);
                            return sharedPreferences.getInt(key, defaultValue);
                } catch (Exception ex) {
                    System.out.println(ex.getMessage());
                    return defaultValue;
                }
            }
            return defaultValue;
        }
}
