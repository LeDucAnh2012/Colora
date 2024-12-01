using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.Notifications.Android;
using UnityEngine;
using UnityEngine.Playables;

public class CC_Notifycation : MonoBehaviour
{
    public static CC_Notifycation instance;
    private string channel_id = "channel_id";

    //AndroidNotificationCenter.NotificationReceivedCallback receivedNotificationHandler =
    //delegate (AndroidNotificationIntentData data)
    //{
    //    var msg = "Notification received : " + data.Id + "\n";
    //    msg += "\n Notification received: ";
    //    msg += "\n .Title: " + data.Notification.Title;
    //    msg += "\n .Body: " + data.Notification.Text;
    //    msg += "\n .Channel: " + data.Channel;
    //    // Debug.Log(msg);
    //};
    private string TimeLoop
    {
        get => PlayerPrefs.GetString(nameof(TimeLoop));
        set
        {
            PlayerPrefs.SetString(nameof(TimeLoop), value);
            PlayerPrefs.Save();
        }
    }
    private bool IsFirstPush
    {
        get => PlayerPrefs.GetInt(nameof(IsFirstPush)) == 1;
        set
        {
            PlayerPrefs.SetInt(nameof(IsFirstPush), value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    private void Awake()
    {
        instance = this;
        //AndroidNotificationCenter.OnNotificationReceived += receivedNotificationHandler;
    }
    private void Start()
    {
        //return;
        if (!VariableSystem.IsScheduleNotify)
        {
            InitNotify();
            SendNotify(0, 0);
            TimeLoop = DateTime.Now.ToString();
            VariableSystem.IsScheduleNotify = true;
        }
        else
        {
            if (!IsFirstPush)
            {
                IsFirstPush = true;
                DateTime now = DateTime.Now;
                DateTime old = DateTime.Parse(TimeLoop);

                if (now.Month == old.Month)
                    if (now.Day == old.Day)
                        if (now.Hour < old.Hour + 3)
                        {
                            AndroidNotificationCenter.CancelAllNotifications();
                            AndroidNotificationCenter.CancelAllScheduledNotifications();
                            VariableSystem.CountDayLogin = TimeManager.instance.GetDayOfYear() + 7;
                            SendNotify(1, 0);
                        }
                        else
                        {
                            IsFirstPush = false;
                        }
            }
        }


        if (VariableSystem.CountDayLogin < TimeManager.instance.GetDayOfYear())
            LoopNotify();
    }
    private void InitNotify()
    {
        var c = new AndroidNotificationChannel()
        {
            Id = channel_id,
            Name = "Default Channel",
            Importance = Importance.High,
            Description = "DIY Doll Dress Up: Makeup Game notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(c);
    }
    private void LoopNotify()
    {
        VariableSystem.CountDayLogin = TimeManager.instance.GetDayOfYear() + 7;
        AndroidNotificationCenter.CancelAllNotifications();
        AndroidNotificationCenter.CancelAllScheduledNotifications();

        for (int i = 0; i < 10; i++)
        {
            var notification = new AndroidNotification();

            var info = DataNotify.GetListNotify()[DataNotify.Count - 1].listInfoNoti[UnityEngine.Random.Range(0, 3)];
            notification.Title = info.strTitle;   // "SomeTitle";
            notification.Text = info.strText;     // "SomeText";

            int hour = DateTime.Now.Hour;
            hour = 24 * (i + 1) - hour;
            notification.FireTime = DateTime.Now.AddHours(info.time + hour);

            AndroidNotificationCenter.SendNotification(notification, channel_id);
            // if (AndroidNotificationCenter.CheckScheduledNotificationStatus(identifier) == NotificationStatus.Scheduled)
        }
    }
    private void SendNotify(int id, int index)
    {
        var notification = new AndroidNotification();
        if (index >= DataNotify.GetListNotify()[id].listInfoNoti.Count)
            index = DataNotify.GetListNotify()[id].listInfoNoti.Count - 1;

        InfoNotify info = DataNotify.GetListNotify()[id].listInfoNoti[index];
        notification.Title = info.strTitle;   // "SomeTitle";
        notification.Text = info.strText;     // "SomeText";

        if (id == 0)
            notification.FireTime = System.DateTime.Now.AddHours(info.time);
        else
        {

            int hour = DateTime.Now.Hour;
            hour = 24 * (id + 1) - hour;
            notification.FireTime = System.DateTime.Now.AddHours(info.time + hour);
        }

        var identifier = AndroidNotificationCenter.SendNotification(notification, channel_id);

        if (AndroidNotificationCenter.CheckScheduledNotificationStatus(identifier) == NotificationStatus.Scheduled)
        {
            if (id == 0)
            {
                TimeLoop = DateTime.Now.ToString();
            }

            // Replace the currently scheduled notification with a new notification.
            if (id == 7)
            {
                // update
                for (int i = 0; i < 10; i++)
                {
                    notification = new AndroidNotification();

                    info = DataNotify.GetListNotify()[DataNotify.Count - 1].listInfoNoti[UnityEngine.Random.Range(0, 3)];
                    notification.Title = info.strTitle;   // "SomeTitle";
                    notification.Text = info.strText;     // "SomeText";

                    int hour = DateTime.Now.Hour;
                    hour = 24 * (i + id + 1) - hour;
                    notification.FireTime = DateTime.Now.AddHours(info.time + hour);

                    if (AndroidNotificationCenter.CheckScheduledNotificationStatus(identifier) == NotificationStatus.Scheduled)
                        AndroidNotificationCenter.SendNotification(notification, channel_id);
                }
                return;
            }
            else
            {
                index++;
                if (index >= DataNotify.GetListNotify()[id].listInfoNoti.Count)
                {
                    id++;
                    index = 0;
                }
            }

            SendNotify(id, index);
        }
        else if (AndroidNotificationCenter.CheckScheduledNotificationStatus(identifier) == NotificationStatus.Delivered)
        {
            //Remove the notification from the status bar
            AndroidNotificationCenter.CancelNotification(identifier);
        }
        else if (AndroidNotificationCenter.CheckScheduledNotificationStatus(identifier) == NotificationStatus.Unknown)
        {
            // AndroidNotificationCenter.SendNotification(newNotification, "channel_id");
        }
    }
}
