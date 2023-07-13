using System;
namespace UnityUaalMaui
{
	public class UnityContentReceivedEventArgs : EventArgs
	{
        public UnityContentReceivedEventArgs(string eventName, string eventContent)
        {
            EventName = eventName;
            EventContent = eventContent;
        }

		public string EventName { get; }

		public string EventContent { get; }
	}

	public static class UnityBridge
	{
		public static event EventHandler<UnityContentReceivedEventArgs> OnContentReceived;

#if __ANDROID__
		protected class UnityContentReceiver : Java.Lang.Object, Com.Unity3d.Player.IUnityContentReceiver
		{

		}
#endif

#if __IOS__
#endif


        public UnityBridge()
		{
		}
	}
}

