# UnityUaal.Maui
Embedding the Unity game engine into .NET MAUI.

What is Unity? What is UAAL? What is MAUI?

This tutorial builds on the 
See also: https://github.com/Unity-Technologies/uaal-example

Requires:

 * Unity (using 23.01.0a20)
 * Xcode
 * Android Studio
 * Visual Studio Mac with MAUI workloads installed

Additionally, assumes familarity, or at least exposure to:

 * Native platform development using either Xcode or Android Studio.
 * .NET development using the Visual Studio family.
 * 
## Why?

To begin, lets compare the strengths and weakness of app development using Unity vs the MAUI way:

**Unity**
| Pro  |  Con |
|---|---|
| Fully fledged 3D engine  |  [Multiple In-Engine UI frameworks (no native UI support)](https://docs.unity3d.com/Manual/UI-system-compare.html) |
| [Simplified augmented and virtual reality](https://unity.com/unity/features/ar)  | Indirect access to native platform features (requires plugin)   |
| [Rich eco-system of 3D tools, plugins and assets](https://assetstore.unity.com)  | [Dated versions of .NET](https://docs.unity3d.com/Manual/dotnetProfileSupport.html) |
|   | Difficult integration of SQLite |
|   | Limited Authentication Vendors |

**MAUI**
| Pro  |  Con |
|---|---|
| Native UIs in a single language and project |   |
| Easily access native features in C# |  |
| Use latest .NET version |  |
| Rich ecosystem of packages (nuget.org) |  |

Using 3D techs in MAUI would require writing engine from scratch.

With this way we can use Unity:

 * Use of full fledged 3D engine.
 * Use of x-plat augmented reality features.

Unity has limitations so using MAUI is useful:

 * Unity is constrained to .NET 4.5 OR .NET Standard
 * With Unity, must use its own UI frameworks and cannot access platform UI features. This makes it hard, cumbersome to build modern UIs. Highly specialist skill-set to hire for.
 * Unity is deeply restricted when using 3rd party vendors (EG: Auth0 for identity)
   

Therefore combining these two techs means we can get the best of both worlds:

 * One of the industry leaders in games/3D techs.
 * Fully x-plat code base in a native app. A fully native UI with no restrictions!
 * .NET across the whole stack.

## Creating Unity Project

 * Outline steps to create a new unity project -> Link to sample included here

Sample is a ground plane with a few pieces of geometry + camera controls + a GameObject with the Bridge.cs script attached (we will use this to talk abetween the app engines)

### Data Bridge Scaffolding

Before continuing into the export and build stages of the embedding, lets define the data bridge we will use to "talk" between Unity and MAUI.

We are defining this now as we want this code to be included into our unity exports so we don't need to redo the following steps again 😅

#### Unity

TODO: Bridge.cs that has send content and receive content.
We will talk about 

#### Android

Plugins => bridge + UnityContentReceiver interface.

#### iOS

Plugins => Bridge.h and Bridge.mm to define Bridge + UnityContentReceiver.

## Exporting Unity Project

 * Outline how to configure the Unity project export (Android, iOS => IL2CPP + require runtimes)
 * Android => Export project tick box then click export

## Building Platform Binaries

After we've generated the platform specific projects, the next step is to create the binary artificats (`aar` for Android, `framework` for iOS) that we will reference in our MAUI app.

### Android

Building the Android aar using the generated gradle file.

 * Open in Android Studio
 * Switch to release mode
 * Build

Where to find the artifact created?

### iOS

Building the iOS framework using the generated xcode project.

 * Open in xcode
 * Adding additional 
 * Exposing the

 https://github.com/FlorianAuer/Unity_as_a_Library-Xamarin-example/tree/master/UnityProject/buildiOS_modifications

 **Additions to Classes\main.mm**
```
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
```

Replace the `unloadApplication` implementation generated by Unity with the one above.

**Additions to UnityFramework\UnityFramework.h**
```
+ (UnityFramework*)loadUnity;

- (void)runEmbedded;
```


  => TODO: Embed these suggested changes into this repo so they are not lost in case that repo is killed!

## Binding Platform Binaries

Now that our platform binaries have been built, lets create the platform specific .NET projects that will be utilised in the MAUI app.

### Android

Creating the Java binding project in VS Mac.
Drag and drop the unity aar into Jars folder.
Done! (.NET auto-generates the interop code, usually we don't need to do anyuthing further

### iOS

Creating the iOS binding project.
Drag and drop the framework into project.


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

## Launching the Unity Runtime

How do we launch the Unity runtime?

### MAUI Wrapper Code

### Android

Adding the required resources for Unity:
 
**strings.xml**
```
<?xml version="1.0" encoding="UTF-8" ?>
<resources>
    <string name="game_view_content_description">Game view</string>
</resources>
```

Without this resource the app will crash on launch!

 Linking to binding project.
 Creating an activity (either using the inbuilt unity launcher or through a custom activity) => Recomend custom binding, can copy the flags/settings 
 Launching this activity.

 Removing the AAR unity activity fom the manifest

### iOS

 Linking to binding project.
 Creating the unity view controller.
 Loading the unity runtime.
 Launchgint the view controller

 TODO: Discuss partial classes with ifdef'ing per platform?

## Communicating Between Unity and MAUI

More than likely need to send data between MAUI and Unity. To do so we create a data bridge to send stringly typed content.

In our first few steps, we did some setup for the data communications throught the 

 => Receving content from unity.

### Android

Implemneting the java bruidge and IUnityContentReceiver interface

### iOS

Implmenting IUnityContentReceiver protocol.

### Maui

Implemnetingn receivers in MAUI

-----

=> Sending content to Unity.

 * Creating data sharing bridge in Unity
 * Implementing the data sharing bridge in Java via an Android Unity plugin
 * Implementing the data sharing bridge in Objective C via an iOS Unity plugin.
 * Implementing the data bridge in MAUI
 * Sending the data via Unity to MAUI (Bridge.SendContent)
 * Receiving the data in MAUI
 * 
 * Sending data via Maui to Unity

 * Returning back to the .NET app

## Known Issues + Limitations

 * Sometimes crashes on Android when *receiving* data from Unity when debugger attached. Does not happen in non-debugging builds.
 * Somestimes launchign unity engine can freeze app on Android. No known cause or solution yet, still investigating. 

## Summary

TODO: Summarise everything above (steps, complexities, why the heck we'd do this?)

If you want assistance in doing the Unity => MAUI integration, please reach out via:

 * Email: matthew@mfractor.com
 * LinkedIn: https://www.linkedin.com/in/matthewrdev/

