using System;
#if __IOS__
using iOSBridge;
#endif

namespace UnityUaalMaui.Unity
{
    public static partial class UnityBridge
    {
        public static void Init()
        {
#if __ANDROID__
            Com.Unity3d.Player.Bridge.Instance.RegisterUnityContentReceiver(new UnityContentReceiver()); // TODO: GC cleanup/deregistering of this receiver?
#endif

#if __IOS__

            var bundlePath = Foundation.NSBundle.MainBundle.BundlePath;
            bundlePath += "/Frameworks/UnityFramework.framework";

            var bundle = Foundation.NSBundle.FromPath(bundlePath);
            if (bundle.IsLoaded == false)
            {
                bundle.Load();
            }

            var frameowrk = UnityFramework.GetInstance();
            if (frameowrk == null)
            {
                // TODO: Log that startup failed!
            }
            else
            {
                var controller = frameowrk.AppController();
            }
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

