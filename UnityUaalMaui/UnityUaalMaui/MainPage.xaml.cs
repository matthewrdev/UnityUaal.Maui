
using System;namespace UnityUaalMaui;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

    void openUnityButton_Clicked(System.Object sender, System.EventArgs e)
    {
#if __ANDROID__
        var intent = new Android.Content.Intent(Microsoft.Maui.ApplicationModel.Platform.CurrentActivity, typeof(UnityActivity));

        Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.StartActivity(intent);
#endif

#if __IOS__
		// TODO: Start the Unity ViewController here.
#endif
    }

    public void sendContentButton_Clicked(object sender, EventArgs e)
    {
    }

}


