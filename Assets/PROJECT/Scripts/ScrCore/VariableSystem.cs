using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using System.Threading;
using UnityEngine;

public static class VariableSystem
{

    #region Setting
    //public static string LanguageCodeCurrent
    //{
    //    get => PlayerPrefs.GetString(nameof(LanguageCodeCurrent), I2.Loc.LocalizationManager.GetLanguageCode(I2.Loc.LocalizationManager.GetCurrentDeviceLanguage()));
    //    set
    //    {
    //        PlayerPrefs.SetString(nameof(LanguageCodeCurrent), value);
    //        PlayerPrefs.Save();
    //    }
    //}
    //public static TypeBooster typeBooster
    //{
    //    set => PlayerPrefs.SetString("TypeBooster" , value.ToString());
    //    get
    //    {
    //        TypeBooster _typeBooster;
    //        string type = PlayerPrefs.GetString("TypeBooster" , "Number");
    //        Enum.TryParse(type, out _typeBooster);
    //        return _typeBooster;
    //    }
    //}
    public static bool Sound
    {
        get => PlayerPrefs.GetInt(nameof(Sound), 1) == 1;
        set
        {
            PlayerPrefs.SetInt(nameof(Sound), value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
    public static bool Music
    {
        get => PlayerPrefs.GetInt(nameof(Music), 1) == 1;
        set
        {
            PlayerPrefs.SetInt(nameof(Music), value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
    public static bool Vibrate
    {
        get => PlayerPrefs.GetInt(nameof(Vibrate), 1) == 1;
        set
        {
            PlayerPrefs.SetInt(nameof(Vibrate), value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    #endregion

    #region Gameplay
    public static bool IsInSessionGame = false;
    public static bool IsResetRewardDailyQuest = false;
    public static bool IsBackFromDIY = false;
    public static int DistancePaintColor
    {
        get => RemoteConfig.instance.allConfigData.DistancePaint - 1;
    }

    public static bool IsQuitGameInCompleteStep
    {
        get => PlayerPrefs.GetInt(nameof(IsQuitGameInCompleteStep)) == 1;
        set
        {
            PlayerPrefs.SetInt(nameof(IsQuitGameInCompleteStep), value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
    public static bool IsClearStep
    {
        get => PlayerPrefs.GetInt(nameof(IsClearStep), 1) == 1;
        set
        {
            PlayerPrefs.SetInt(nameof(IsClearStep), value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
    public static bool IsQuitGame
    {
        get => PlayerPrefs.GetInt(nameof(IsQuitGame)) == 1;
        set
        {
            PlayerPrefs.SetInt(nameof(IsQuitGame), value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
    public static int StoreHeart
    {
        get => PlayerPrefs.GetInt(nameof(StoreHeart), 0);
        set
        {
            PlayerPrefs.SetInt(nameof(StoreHeart), value);
            PlayerPrefs.Save();
        }
    }
    public static bool IsBeginShowInter
    {
        get => PlayerPrefs.GetInt(nameof(IsBeginShowInter)) == 1;
        set
        {
            PlayerPrefs.SetInt(nameof(IsBeginShowInter), value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
    public static int CountClickPlay
    {
        get => PlayerPrefs.GetInt(nameof(CountClickPlay));
        set
        {
            PlayerPrefs.SetInt(nameof(CountClickPlay), value);
            PlayerPrefs.Save();
        }
    }
    public static int CountClickContinue
    {
        get => PlayerPrefs.GetInt(nameof(CountClickContinue));
        set
        {
            PlayerPrefs.SetInt(nameof(CountClickContinue), value);
            PlayerPrefs.Save();
        }
    }
    public static bool FirstOpenGame
    {
        get => PlayerPrefs.GetInt(nameof(FirstOpenGame)) == 1;
        set
        {
            PlayerPrefs.SetInt(nameof(FirstOpenGame), value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
    #region Rate && Remove Ads

    public static bool IsRate
    {
        get => PlayerPrefs.GetInt(nameof(IsRate)) == 1;
        set
        {
            PlayerPrefs.SetInt(nameof(IsRate), value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static int CountShowRate
    {
        get => PlayerPrefs.GetInt(nameof(CountShowRate), 3);
        set
        {
            PlayerPrefs.SetInt(nameof(CountShowRate), value);
            PlayerPrefs.Save();
        }
    }
    public static int CountRate
    {
        get => PlayerPrefs.GetInt(nameof(CountRate), 0);
        set
        {
            PlayerPrefs.SetInt(nameof(CountRate), value);
            PlayerPrefs.Save();
        }
    }
    public static int CountShowRemoveAds
    {
        get => PlayerPrefs.GetInt(nameof(CountShowRemoveAds), 0);
        set
        {
            PlayerPrefs.SetInt(nameof(CountShowRemoveAds), value);
            PlayerPrefs.Save();
        }
    }
    public static bool RemoveAds
    {
        get => PlayerPrefs.GetInt(nameof(RemoveAds)) == 1;
        set
        {
            PlayerPrefs.SetInt(nameof(RemoveAds), value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
    public static int IndexShowAds
    {
        get => PlayerPrefs.GetInt(nameof(IndexShowAds), 0);
        set
        {
            PlayerPrefs.SetInt(nameof(IndexShowAds), value);
            PlayerPrefs.Save();
        }
    }
    public static bool IsCanShowInter
    {
        get => PlayerPrefs.GetInt(nameof(IsCanShowInter), 0) == 1;
        set
        {
            PlayerPrefs.SetInt(nameof(IsCanShowInter), value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
    #endregion

    #endregion

    #region Hack
    public static bool OnOffColorCam
    {
        get => PlayerPrefs.GetInt(nameof(OnOffColorCam), 0) == 1;
        set
        {
            PlayerPrefs.SetInt(nameof(OnOffColorCam), value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
    public static bool OnOffUIGameplay
    {
        get => PlayerPrefs.GetInt(nameof(OnOffUIGameplay), 1) == 1;
        set
        {
            PlayerPrefs.SetInt(nameof(OnOffUIGameplay), value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
    public static bool IsClearData
    {
        get => PlayerPrefs.GetInt(nameof(IsClearData), 0) == 1;
        set
        {
            PlayerPrefs.SetInt(nameof(IsClearData), value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
    public static bool RemoveAdsHack
    {
        get => PlayerPrefs.GetInt(nameof(RemoveAdsHack), 0) == 1;
        set
        {
            PlayerPrefs.SetInt(nameof(RemoveAdsHack), value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }


    public static bool IsDebug
    {
        get => PlayerPrefs.GetInt(nameof(IsDebug), 0) == 1;
        set
        {
            PlayerPrefs.SetInt(nameof(IsDebug), value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
    public static bool IsUseIdTest
    {
        get => PlayerPrefs.GetInt(nameof(IsUseIdTest), 0) == 1;
        set
        {
            PlayerPrefs.SetInt(nameof(IsUseIdTest), value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
    public static bool OffUIInGame
    {
        get => PlayerPrefs.GetInt(nameof(OffUIInGame), 0) == 1;
        set
        {
            PlayerPrefs.SetInt(nameof(OffUIInGame), value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
    public static bool UnlockAll
    {
        get => PlayerPrefs.GetInt(nameof(UnlockAll), 0) == 1;
        set
        {
            PlayerPrefs.SetInt(nameof(UnlockAll), value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
    public static bool OffBG
    {
        get => PlayerPrefs.GetInt(nameof(OffBG), 0) == 1;
        set
        {
            PlayerPrefs.SetInt(nameof(OffBG), value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
    #endregion

    #region DailyReward
    private static string KeyLogTask = "LogTask_";
    public static void SetLogTask(int id)
    {
        if (PlayerPrefs.GetInt(KeyLogTask + id) == 0)
        {
            ActionHelper.LogEvent(KeyLogFirebase.DoneTask + id);
            PlayerPrefs.SetInt(KeyLogTask + id, 1);
        }
    }
    public static void ResetLogTask(int id)
    {
        PlayerPrefs.SetInt(KeyLogTask + id, 0);
    }

    public static int DayOfYear
    {
        get => PlayerPrefs.GetInt(nameof(DayOfYear));
        set
        {
            PlayerPrefs.SetInt(nameof(DayOfYear), value);
            PlayerPrefs.Save();
        }
    }
    public static string ListTaskOn
    {
        get => PlayerPrefs.GetString(nameof(ListTaskOn), "null");
        set
        {
            PlayerPrefs.SetString(nameof(ListTaskOn), value);
            PlayerPrefs.Save();
        }
    }


    #endregion

    #region [ Daily Reward ]
    public static bool IsCollectX2
    {
        get => PlayerPrefs.GetInt("IsCollectX2", 0) == 1;
        set => PlayerPrefs.SetInt("IsCollectX2", value ? 1 : 0);
    }
    public static bool IsCollect
    {
        get => PlayerPrefs.GetInt("IsCollect", 0) == 1;
        set => PlayerPrefs.SetInt("IsCollect", value ? 1 : 0);
    }
    /// <summary>
    /// Day 1 -> Day 7
    /// </summary>
    public static int CountReceivedDaily
    {
        get => PlayerPrefs.GetInt("CountReceivedDaily", -1);
        set => PlayerPrefs.SetInt("CountReceivedDaily", value);
    }

    #endregion

    #region Booster

    public static int FindBooster
    {
        get => PlayerPrefs.GetInt(nameof(TypeBooster.Find), 10);
        set
        {
            PlayerPrefs.SetInt(nameof(TypeBooster.Find), value);
            PlayerPrefs.Save();
        }
    }

    public static int FillByNumBooster
    {
        get => PlayerPrefs.GetInt(nameof(TypeBooster.Number), 10);
        set
        {
            PlayerPrefs.SetInt(nameof(TypeBooster.Number), value);
            PlayerPrefs.Save();
        }
    }

    public static int FillByBomBooster
    {
        get => PlayerPrefs.GetInt(nameof(TypeBooster.Bomb), 10);
        set
        {
            PlayerPrefs.SetInt(nameof(TypeBooster.Bomb), value);
            PlayerPrefs.Save();
        }
    }

    #endregion

    #region DIY
    public static int LevelDIY
    {
        get => PlayerPrefs.GetInt("LevelDIY");
        set
        {
            PlayerPrefs.SetInt("LevelDIY", value);
            PlayerPrefs.Save();
        }
    }
    #endregion

    #region SDK
    public static int CountDayLogin
    {
        get => PlayerPrefs.GetInt(nameof(CountDayLogin));
        set
        {
            PlayerPrefs.SetInt(nameof(CountDayLogin), value);
            PlayerPrefs.Save();
        }
    }
    public static bool IsScheduleNotify
    {
        get => PlayerPrefs.GetInt(nameof(IsScheduleNotify)) == 1;
        set
        {
            PlayerPrefs.SetInt(nameof(IsScheduleNotify), value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
    #endregion

}