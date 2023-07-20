
using System;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using iOSBridge;
using UnityUaalMaui.Unity;

namespace UnityUaalMaui;

public partial class MainPage : ContentPage
{
    int count = 0;
    public MainPage()
    {
		InitializeComponent();

        Unity.UnityBridge.OnContentReceived += Bridge_OnContentReceived;
	}

    private void Bridge_OnContentReceived(object sender, Unity.UnityContentReceivedEventArgs e)
    {
        ToastDuration duration = ToastDuration.Short;
        double fontSize = 14;

        var toast = Toast.Make($"Received from Unity: {e.EventName}{e.EventContent}", duration, fontSize);

        toast.Show().ConfigureAwait(false);

        switch (e.EventName)
        {
            case Unity.UnityEvents.ShowMainWindow:
                UnityBridge.ShowMainWindow();
                break;
        }
    }

    void openUnityButton_Clicked(System.Object sender, System.EventArgs e)
    {
        UnityBridge.ShowUnityWindow();
    }

    public void sendContentButton_Clicked(object sender, EventArgs e)
    {
        UnityBridge.SendContent("SendContent", this.textContent.Text);
    }

}


