namespace UnityUaalMaui;

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
		// TODO: Start the Unity Activity here.
#elif __IOS__
		// TODO: Start the Unity ViewController here.
#endif
	}
}


