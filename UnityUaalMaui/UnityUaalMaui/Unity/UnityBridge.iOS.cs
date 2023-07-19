#if __IOS__

using System;
using Foundation;
using iOSBridge;

namespace UnityUaalMaui.Unity
{
    public class UnityBridge_iOS : INativeUnityBridge
    {


        public void SendContent(string eventName, string eventContent)
        {
            throw new NotImplementedException();
        }
    }

    public static partial class UnityBridge
    {
        public class UnityBridge_UnityFrameworkListener : UnityFrameworkListener
        {
            public override void UnityDidQuit(NSNotification notification)
            {
            }

            public override void UnityDidUnload(NSNotification notification)
            {
            }
        }

        // TODO: This has some issues when compiling? Maybe concat it to just "content" and split by a pipe for event/event name?
        //public class UnityBridge_UnityContentReceiver : UnityContentReceiver
        //{
        //    public override void ReceiveUnityContent(string eventName, string eventContent)
        //    {
        //    }
        //}

        public const string UnityFrameworkPath = "/Frameworks/UnityFramework.framework";

        private static UnityBridge_UnityFrameworkListener unityFrameworkListener = null;
        private static UnityFramework framework = null;
        public static bool IsUnityInitialised => framework != null && framework.AppController() != null;

        public static void ShowMainWindow()
        {

        }

        public static void ShowUnityWindow()
        {
            if (!IsUnityInitialised)
            {
                InitialiseUnity();
            }

            if (framework != null)
            {
                framework.ShowUnityWindow();
            }
        }

        // TODO: Tear down unity?

        private static void InitialiseUnity()
        {
            if (IsUnityInitialised)
            {
                return;
            }

            framework = UnityFramework.LoadUnity();

            framework.RunEmbedded();
        }
    }
}

#endif