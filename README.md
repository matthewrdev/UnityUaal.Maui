# UnityUaal.Maui
This code sample demonstrates how to embed [Unity](https://unity.com) into [.NET MAUI](https://learn.microsoft.com/en-us/dotnet/maui/what-is-maui).

![<img src="/img/unity-maui-embedding.gif" width="250"/>](/img/unity-maui-embedding.gif)

See: https://github.com/Unity-Technologies/uaal-example

Requires:

 * Unity (using 23.01.0a20)
 * Xcode
 * Android Studio
 * Visual Studio Mac with MAUI workloads installed

Additionally, assumes familarity, or at least exposure to:

 * Native platform development using either Xcode or Android Studio.
 * .NET development using the Visual Studio family.
 * Unity development.
   
If you want assistance in doing the Unity => MAUI integration, please reach out via:

 * Email: matthew@mfractor.com
 * LinkedIn: https://www.linkedin.com/in/matthewrdev/

My time is somewhat limited but I'll provide assistance where I can.

## Why Embedd Unity Into MAUI?

Lets compare the strengths and weakness of app development using Unity vs .NET MAUI:

**Unity**
| Pro  |  Con |
|---|---|
| Fully fledged 3D engine  |  [Multiple In-Engine UI frameworks (no native UI support)](https://docs.unity3d.com/Manual/UI-system-compare.html) |
| [Simplified augmented and virtual reality](https://unity.com/unity/features/ar)  | Indirect access to native platform features (requires plugin)   |
| [Rich eco-system of 3D tools, plugins and assets](https://assetstore.unity.com)  | [Dated versions of .NET](https://docs.unity3d.com/Manual/dotnetProfileSupport.html) |
|   | Difficult integration of SQLite |
|   | Limited Authentication Vendors |
|   | Highly specialised skillset to hire for. |

**MAUI**
| Pro  |  Con |
|---|---|
| Native UIs in a single language and project | No 3D engine support  |
| Easily access native features in C# | Access to 3D features requires complex per-platform implementation |
| Use latest .NET version |  |
| Rich ecosystem of packages (nuget.org) |  |
| "Simple" binding and consumption of native libraries |  |

While Unity is an incredible platform to create 3D experiences, it is not a good choice for a modern, native apps. Unity to simplifies building augmented or virtual reality experiences, and gives us a full engine for rendering 3D environments however it's UI frameworks are limited as are its access to 3rd party vendors.

On the other side, MAUI can create fully native iOS and Android apps. We gain access to the full .NET ecosystem, utilising the latest language features and a large libary of NuGet packages. However creating a 3d experience in MAUI requires writing a 3d rendering pipeline from near scratch... and this is not an easy task!

Therefore, by combining these two technologies, we can get the best of both worlds:

 * Use an industry leader in games/3D technologies.
 * Fully x-plat code base in a native app. A fully native UI with no restrictions!
 * .NET across the whole stack.

## Integrating Unity Into MAUI

This section briefly outlines the 

### Unity Project Overview

The [Unity Project](/UnityApp) contains the following:

 * A single scene with some geometry.
 * A [script for controlling the camera via touch gestures](/UnityApp/Assets/Scripts/CameraZoom.cs).
 * A [script that defines APIs for sharing data from Unity to MAUI via the data bridge](/UnityApp/Assets/Scripts/Bridge.cs).
 * Native plugins for [iOS](/UnityApp/Assets/Plugins/iOS) and [Android](/UnityApp/Assets/Plugins/Android) that define APIs for sharing data from MAUI to Unity via the data bridge.

### Exporting Platform Projects

When building a Unity app for Android and iOS, it exports itself as an Android gradle project and Xcode project.

Each of these projects contains two parts:

 * The Unity app encapsulated into a standalone library.
 * The native "shim" project that references the Unity app library and starts it.

Please follow the instructions in the Unity As A Library for [iOS](https://github.com/Unity-Technologies/uaal-example/blob/master/docs/ios.md) and [Android](https://github.com/Unity-Technologies/uaal-example/blob/master/docs/android.md) to create the native projects.

### Building Native Frameworks

Once you have exported the native projects for iOS and Android, you will need to build the `framework` (iOS) and `aar` (Android) that will be consumed in .NET.

**Android**
To create the Android aar:

 * Open the generated gradle project in Android Studio.
 * Change the build type to `Release` by selecting **Build** => **Select Build Variant** and changing the **:unityLibrary** configuration to **Release**
 * Select **Build** => **Make Project**

The created `aar` will be found under `[Exported Unity App Android Folder]/unityLibrary/build/outputs/aar/unityLibrary-release.aar`.

Please note that you may need to install several additional components for Android Studio such as the NDK and cmake.

**iOS**
To create the iOS framework:

 * Open the generated project in XCode.
 * Change the build type to `Release` by selecting **Product** => **Scheme** => **Edit Scheme** and then selecting the **Unity Framework** item from the title of the window, then changing build configuration to **Release**.
 * Ensuring that the `Data` folder is included into the framework (click on item and set to public)
 * Ensuring that the Bridge.h is included and can accessed by our iOS binding (click on item and set to public).

Make the following modifications to the main.mm and UnityFramework.h file:

 **Additions to Classes\main.mm**
````
+ (UnityFramework*)loadUnity
{
    UnityFramework* ufw = [UnityFramework getInstance];
    if (![ufw appController])
    {
        // unity is not initialized
        //[ufw setExecuteHeader: &_mh_execute_header];
    }

    [ufw setDataBundleId: "com.unity3d.framework"];
    
    return ufw;
}


- (char**)getArgumentArray
{
    NSArray* args = [[NSProcessInfo processInfo]arguments];

    unsigned count = [args count];
    char** array = (char **)malloc((count+ 1) * sizeof(char*));

    for (unsigned i = 0; i< count; i++)
    {
        array[i] = strdup([[args objectAtIndex:i] UTF8String]);
    }
    array[count] = NULL;
    return array;
}

- (unsigned)getArgumentCount
{
    NSArray* args = [[NSProcessInfo processInfo]arguments];

    unsigned count = [args count];
    return count;
}

- (void)freeArray:(char **)array
{
    if (array != NULL)
    {
        for (unsigned index = 0; array[index] != NULL; index++)
        {
            free(array[index]);
        }
        free(array);
    }
}

- (void)runEmbedded
{
    char** argv = [self getArgumentArray];
    unsigned argc = [self getArgumentCount];
    NSDictionary* appLaunchOpts = [[NSDictionary alloc] init];
    
    if (self->runCount)
    {
        // initialize from partial unload ( sceneLessMode & onPause )
        UnityLoadApplicationFromSceneLessState();
        [self pause: false];
        [self showUnityWindow];
    }
    else
    {
        // full initialization from ground up
        [self frameworkWarmup: argc argv: argv];

        id app = [UIApplication sharedApplication];

        id appCtrl = [[NSClassFromString([NSString stringWithUTF8String: AppControllerClassName]) alloc] init];
        [appCtrl application: app didFinishLaunchingWithOptions: appLaunchOpts];

        [appCtrl applicationWillEnterForeground: app];
        [appCtrl applicationDidBecomeActive: app];
    }

    self->runCount += 1;
}

//this method already exists, just add the difference
- (void)unloadApplication
{
    freeArray:([self getArgumentArray]); //added line of code
    UnityUnloadApplication();
}
````

Replace the `unloadApplication` implementation generated by Unity with the one above.

**Additions to UnityFramework\UnityFramework.h**
```
+ (UnityFramework*)loadUnity;

- (void)runEmbedded;
```

These changes make it much simpler for Unity to run in embedded mode in our MAUI app.

[Credit](https://github.com/FlorianAuer/Unity_as_a_Library-Xamarin-example/tree/master/UnityProject/buildiOS_modifications)

 Finally, select **Product** => **Build** to compile and generate the release framework.

The created `framework` will be found under `[Exported Unity App iOS Folder]/Build/Products/Release-iphoneos/UnityFramework.framework`.

Check that the outputted framework contains the following content:

 * The `Data` folder in the root of the framework.
 * The file `Headers/Bridge.h`.

### .NET Native Bindings

Once you have built the `framework` and `aar` libraries, you will need to create an Android and iOS binding project alongside the 

For Android: 

 * Create a new Android .NET binding project.
 * Create a new folder named **Jars**
 * Drag and drop the `unityLibrary-release.aar` into the **Jars** folder.

For iOS:

 * Create a new iOS .NET binding project.
 * Right click on **Native References** and choose **Add Native Reference**. Locate the `UnityFramework.framework` release artifact from the previous step.
 * Open the ApiDefinition.cs file and replace with the following content: 

**ApiDefinitions.cs**
```
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
        [Export("receiveUnityContent:eventContent:")]
        void ReceiveUnityContent ([PlainString] string eventName, [PlainString] string eventContent);
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
```
 
 * Open the StructsAndEnums.cs file and replace with the following content:

```
using System.Runtime.InteropServices;
using Foundation;

namespace iOSBridge
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MachHeader
    {
        public uint magic;     /* mach magic number identifier */
        public int cputype; /* cpu specifier ; cpu_type_t*/
        public int cpusubtype;   /* machine specifier ; cpu_subtype_t */
        public uint filetype;  /* type of file */
        public uint ncmds;     /* number of load commands */
        public uint sizeofcmds;    /* the size of all the load commands */
        public uint flags;     /* flags */
        public uint reserved;  /* reserved */
    }
}
```

### Starting Unity In MAUI

To start the Unity app in MAUI:

**Android**

Create a new `Activity` under `Platforms/Android/` named `UnityActivity` and replace it with the content defined in [/UnityActivity.cs](UnityUaalMaui/UnityUaalMaui/Platforms/Android/UnityActivity.cs).

Please review the code carefully as this file:

 * Instantiates a new UnityPlayer and adds it as the root control of the actity.
 * Connects the core activity callbacks into the Unity Player.
 * Implements the data bridging API calls.

To start Unity, start the activity with a new Intent:

```
public static void ShowUnityWindow()
{
	var intent = new Android.Content.Intent(Microsoft.Maui.ApplicationModel.Platform.CurrentActivity, typeof(UnityActivity));
	intent.AddFlags(Android.Content.ActivityFlags.ReorderToFront);

	Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.StartActivity(intent);
}
```

**iOS**

To start Unity, first initialise the Unity framework:

```
private static UnityFramework framework = null;
public static bool IsUnityInitialised => framework != null && framework.AppController() != null;

private static void InitialiseUnity()
{
    if (IsUnityInitialised)
    {
	return;
    }

    framework = UnityFramework.LoadUnity();

    framework.RegisterFrameworkListener(new UnityBridge_UnityFrameworkListener());
    Bridge.RegisterUnityContentReceiver(new UnityBridge_UnityContentReceiver());

    framework.RunEmbedded();
}
```

Then open the Unity ViewController by calling `framework.ShowUnityWindow()`:

```
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
```

### Communicating Between Unity and MAUI

To send and receive content from Unity, please review the platform specific implementations of the UnityBridge:

 * [UnityBridge.Android.cs](UnityUaalMaui/UnityUaalMaui/Unity/UnityBridge.Android.cs)
 * [UnityBridge.iOS.cs](UnityUaalMaui/UnityUaalMaui/Unity/UnityBridge.iOS.cs)

## Known Issues + Limitations

 * Sometimes crashes on Android when *receiving* data from Unity when debugger attached. Does not happen in non-debugging builds.
 * Somestimes launching the Unity engine can freeze app on Android. No known cause or solution yet, still investigating. 

