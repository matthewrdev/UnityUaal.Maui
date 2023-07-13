using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using UnityEngine;

#if UNITY_IOS || UNITY_TVOS
public class NativeAPI {
    [DllImport("__Internal")]
    public static extern void onUnityContent(string eventName, string eventContent);
}
#endif

public class Bridge : MonoBehaviour
{
    void Update()
    {
    }
    
    public void SendContent(string eventName, string eventContent)
    {
#if UNITY_ANDROID
        try
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.Bridge");
            AndroidJavaObject overrideActivity = jc.GetStatic<AndroidJavaObject>("instance");
            overrideActivity.Call("onUnityContent", eventName, eventContent);
        } catch(Exception e)
        {
            // TODO: Exception handling
        }
#elif UNITY_IOS || UNITY_TVOS
        NativeAPI.onUnityContent(eventName, eventContent);
#endif
    }
}


