using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Toast
{
#if UNITY_ANDROID && !UNITY_EDITOR
    public static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    public static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
#else
    public static AndroidJavaClass unityPlayer;
    public static AndroidJavaObject currentActivity;
#endif

    public static void ShowToastMessage(string str)
    {
        AndroidJavaObject toast = new AndroidJavaObject("android.widget.Toast", currentActivity);
        toast.CallStatic<AndroidJavaObject>("makeText", currentActivity, str, 0).Call("show");
    }
}