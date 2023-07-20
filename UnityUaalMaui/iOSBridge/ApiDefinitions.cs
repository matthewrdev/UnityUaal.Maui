using System;
using CoreAnimation;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace iOSBridge
{
    interface IUnityContentReceiver { }

    [BaseType(typeof(NSObject))]
    [Model]
    [Protocol]
    interface UnityContentReceiver 
    {
        [Export("onReceivedUnityContent:")]
        void OnReceiveUnityContent ([PlainString] string content);
	}

    [BaseType(typeof(NSObject))]
    interface Bridge
    {
        [Static, Export("registerUnityContentReceiver:")]
        void RegisterUnityContentReceiver(IUnityContentReceiver contentReceiver);
    }

    [BaseType(typeof(NSObject))]
    interface UnityAppController : IUIApplicationDelegate
    {
        [Export("quitHandler", ArgumentSemantic.Copy)]
        Action QuitHandler { get; set; }

        [Export("rootView", ArgumentSemantic.Copy)]
        UIView RootView { get; }

        [Export("rootViewController", ArgumentSemantic.Copy)]
        UIViewController RootViewController { get; }
    }

    interface IUnityFrameworkListener { }

    [BaseType(typeof(NSObject))]
    [Model]
    [Protocol]
    interface UnityFrameworkListener
    {
        [Export("unityDidUnload:")]
        void UnityDidUnload(NSNotification notification);

        [Export("unityDidQuit:")]
        void UnityDidQuit(NSNotification notification);
    }

    [BaseType(typeof(NSObject))]
    interface UnityFramework
    {
        [Export("appController")]
        UnityAppController AppController();

        [Static, Export("getInstance")]
        UnityFramework GetInstance();

        [Export("setDataBundleId:")]
        void SetDataBundleId([PlainString] string bundleId);

        [Static, Export("loadUnity")]
        UnityFramework LoadUnity();

        [Internal, Export("runUIApplicationMainWithArgc:argv:")]
        void RunUIApplicationMainWithArgc(int argc, IntPtr argv);

        [Export("runEmbedded")]
        void RunEmbedded();

        [Internal, Export("runEmbeddedWithArgc:argv:appLaunchOpts:")]
        void RunEmbeddedWithArgc(int argc, IntPtr argv, NSDictionary options);

        [Export("unloadApplication")]
        void UnloadApplication();

        [Export("quitApplication:")]
        void QuitApplication(int exitCode);

        [Export("registerFrameworkListener:")]
        void RegisterFrameworkListener(IUnityFrameworkListener obj);

        [Export("unregisterFrameworkListener:")]
        void UnregisterFrameworkListener(IUnityFrameworkListener obj);

        [Export("showUnityWindow")]
        void ShowUnityWindow();

        [Export("pause:")]
        void Pause(bool pause);

        [Export("setExecuteHeader:")]
        void SetExecuteHeader(ref MachHeader header);

        [Export("sendMessageToGOWithName:functionName:message:")]
        void SendMessageToGOWithName([PlainString] string goName, [PlainString] string functionName, [PlainString] string msg);
    }

}
