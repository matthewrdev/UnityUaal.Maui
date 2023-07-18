using System.Runtime.InteropServices;
using Foundation;
using UnityFramework;

namespace Unity
{
	static class CFunctions
	{
		// extern void UnityRegisterLifeCycleListener (id<LifeCycleListener> obj);
		[DllImport ("__Internal")]
		[Verify (PlatformInvoke)]
		static extern void UnityRegisterLifeCycleListener (LifeCycleListener obj);

		// extern void UnityUnregisterLifeCycleListener (id<LifeCycleListener> obj);
		[DllImport ("__Internal")]
		[Verify (PlatformInvoke)]
		static extern void UnityUnregisterLifeCycleListener (LifeCycleListener obj);

		// extern UnityAppController * GetAppController ();
		[DllImport ("__Internal")]
		[Verify (PlatformInvoke)]
		static extern UnityAppController GetAppController ();

		// extern void AppController_SendNotification (NSString *name);
		[DllImport ("__Internal")]
		[Verify (PlatformInvoke)]
		static extern void AppController_SendNotification (NSString name);

		// extern void AppController_SendNotificationWithArg (NSString *name, id arg);
		[DllImport ("__Internal")]
		[Verify (PlatformInvoke)]
		static extern void AppController_SendNotificationWithArg (NSString name, NSObject arg);

		// extern void AppController_SendUnityViewControllerNotification (NSString *name);
		[DllImport ("__Internal")]
		[Verify (PlatformInvoke)]
		static extern void AppController_SendUnityViewControllerNotification (NSString name);
	}
}
