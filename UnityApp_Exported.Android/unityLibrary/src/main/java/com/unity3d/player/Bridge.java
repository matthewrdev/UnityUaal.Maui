package com.unity3d.player;

public class Bridge {
    public static final Bridge instance = new Bridge();

    private IUnityContentReceiver unityContentReceiver = null;

    public void registerUnityContentReceiver(IUnityContentReceiver receiver) {
        unityContentReceiver = receiver;
    }

    public void onUnityContent(String eventName, String eventContent) {
        if (unityContentReceiver == null) {
            return;
        }

        unityContentReceiver.onUnityContent(eventName, eventContent);
    }
}
