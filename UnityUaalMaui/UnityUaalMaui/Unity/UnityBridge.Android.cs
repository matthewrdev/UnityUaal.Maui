#if __ANDROID__

using System;
namespace UnityUaalMaui.Unity
{
	public static partial class UnityBridge
	{
		public static void ShowMainWindow()
		{
			var intent = new Android.Content.Intent(Microsoft.Maui.ApplicationModel.Platform.CurrentActivity, typeof(MainActivity));
            intent.AddFlags(Android.Content.ActivityFlags.ReorderToFront | Android.Content.ActivityFlags.SingleTop);

            Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.StartActivity(intent);
		}

		public static void ShowUnityWindow()
		{
			var intent = new Android.Content.Intent(Microsoft.Maui.ApplicationModel.Platform.CurrentActivity, typeof(UnityActivity));
			intent.AddFlags(Android.Content.ActivityFlags.ReorderToFront);

			Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.StartActivity(intent);
		}
	}
}
#endif