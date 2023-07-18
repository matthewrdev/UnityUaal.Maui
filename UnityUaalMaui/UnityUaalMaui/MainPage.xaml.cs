
using System;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

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
            case Unity.UnityEvents.FocusAppWindow:
#if __ANDROID__
                var intent = new Android.Content.Intent(Microsoft.Maui.ApplicationModel.Platform.CurrentActivity, typeof(MainActivity));
                intent.AddFlags(Android.Content.ActivityFlags.ReorderToFront | Android.Content.ActivityFlags.SingleTop);

                Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.StartActivity(intent);
#endif

#if __IOS__
#endif
                break;
        }
    }

    void openUnityButton_Clicked(System.Object sender, System.EventArgs e)
    {
#if __ANDROID__
        var intent = new Android.Content.Intent(Microsoft.Maui.ApplicationModel.Platform.CurrentActivity, typeof(UnityActivity));
        intent.AddFlags(Android.Content.ActivityFlags.ReorderToFront);

        Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.StartActivity(intent);
#endif

#if __IOS__
		// TODO: Start the Unity ViewController here.
#endif
    }

    public void sendContentButton_Clicked(object sender, EventArgs e)
    {
        Unity.UnityBridge.SendContent("SendContent", this.textContent.Text);
    }

}


