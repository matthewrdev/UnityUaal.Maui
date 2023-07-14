package com.unity3d.player;

public abstract class BaseUnityContentReceiver implements IUnityContentReceiver {

    // Callback before Unity player process is killed
    @Override public void receiveUnityContent(String eventName, String eventContent) {
        onReceivedUnityContent(eventName, eventContent);
    }

    abstract protected void onReceivedUnityContent(String eventName, String eventContent);
}