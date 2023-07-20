package com.unity3d.player;

public abstract class BaseUnityContentReceiver implements IUnityContentReceiver {
    @Override public void receiveUnityContent(String content) {
        onReceivedUnityContent(econtent);
    }

    abstract protected void onReceivedUnityContent(String content);
}