package com.unity3d.player;

public class Bridge {
    public static final Bridge instance = new Bridge();

    private IUnityContentReceiver unityContentReceiver = null;

    public void registerUnityContentReceiver(IUnityContentReceiver receiver) {
        android.util.Log.i("Bridge", "Registered new IUnityContentReceiver");
        unityContentReceiver = receiver;
    }

    public void onUnityContent(String eventName, String eventContent) {
        if (unityContentReceiver == null) {

            android.util.Log.i("Bridge", "onUnityContent => No registered IUnityContentReceiver");
            return;
        }
        android.util.Log.i("Bridge", "Send to IUnityContentReceiver => onUnityContent(" + eventName + ", " + eventContent + ")");
        
        unityContentReceiver.onUnityContent(eventName, eventContent);
    }
}
