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

    public void ReceiveContent(string content)
    {
        string[] splitContent = content.Split("|");
        string eventName = splitContent[0];
        string eventContent = "";
        if (splitContent.Length > 0)
        {
            eventContent = splitContent[1];
        }

        Debug.Log("Received a message from MAUI: Name=" + eventName + ", Content=" + eventContent);
    }
    
    public void SendContent(string eventName, string eventContent)
    {
        Debug.Log("Send content request: " + eventName + "|" + eventContent);
#if UNITY_ANDROID
        try
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.Bridge");
            AndroidJavaObject bridgeInstance = jc.GetStatic<AndroidJavaObject>("instance");
            bridgeInstance.Call("onUnityContent", eventName, eventContent);
        } catch(Exception e)
        {
            Debug.Log("Send content error");
            Debug.Log(e.ToString());
            // TODO: Exception handling?
        }
#elif UNITY_IOS || UNITY_TVOS
        NativeAPI.onUnityContent(eventName, eventContent);
#endif
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle("button");
        style.fontSize = 30;
        if (GUI.Button(new Rect(10, 10, 200, 100), "Red", style)) SendContent("ButtonTap", "Red");
        if (GUI.Button(new Rect(10, 110, 200, 100), "Blue", style)) SendContent("ButtonTap", "Red");
        if (GUI.Button(new Rect(10, 300, 400, 100), "Show Main With Color", style)) SendContent("ButtonTap", "ShowMainWindow");

    }
}


