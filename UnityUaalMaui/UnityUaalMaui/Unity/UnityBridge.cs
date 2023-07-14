using System;

namespace UnityUaalMaui.Unity
{
    public static class UnityBridge
    {
        public static void Init()
        {
#if __ANDROID__
            Com.Unity3d.Player.Bridge.Instance.RegisterUnityContentReceiver(new UnityContentReceiver()); // TODO: GC cleanup/deregistering of this receiver?
#endif
        }

        private static WeakReference<INativeUnityBridge> nativeBridgeReference;

        public static void RegisterNativeBridge(INativeUnityBridge nativeBridge)
        {
            if (nativeBridge == null)
            {
                nativeBridgeReference = null;
            }
            else
            {
                nativeBridgeReference = new WeakReference<INativeUnityBridge>(nativeBridge);
            }
        }

		public static event EventHandler<UnityContentReceivedEventArgs> OnContentReceived;

#if __ANDROID__
        protected class UnityContentReceiver : Java.Lang.Object, Com.Unity3d.Player.IUnityContentReceiver
        {
            public UnityContentReceiver()
            {
            }

            public UnityContentReceiver(nint handle, Android.Runtime.JniHandleOwnership transfer) : base(handle, transfer)
            {
            }

            public void OnUnityContent(string eventName, string eventContent)
            {
                OnContentReceived?.Invoke(this, new UnityContentReceivedEventArgs(eventName, eventContent));
            }
        }
#endif

#if __IOS__
#endif

        public static void SendContent(string eventName, string eventContent)
        {
            if (string.IsNullOrWhiteSpace(eventName))
            {
                throw new ArgumentException($"'{nameof(eventName)}' cannot be null or whitespace.", nameof(eventName));
            }

            if (!eventName.All(char.IsLetterOrDigit))
            {
                throw new ArgumentException($"'{nameof(eventName)}' must be only alpha-numeric characters.", nameof(eventName));
            }

            if (nativeBridgeReference != null && nativeBridgeReference.TryGetTarget(out var nativeBridge))
            {
                try
                {
                    nativeBridge.SendContent(eventName, eventContent);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
	}
}

