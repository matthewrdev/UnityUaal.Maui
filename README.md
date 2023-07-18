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

To begin, lets compare the strengths and weakness of app development using Unity vs the "native" way:

**Unity**
| Pro  |  Con |
|---|---|
| Fully fledged 3D engine  |  Cannot  |
| Simplified augmented and virtual reality  | In-direct access to native platform features (requires plugin)  |
|   | Bespoke UI frameworks  |


**Native Platforms (iOS/Android)**
| Pro  |  Con |
|---|---|
| Fully fledged 3D engine  |   |
| Simplified augmented and virtual reality  | In-direct access to native platform features (requires plugin)  |
|   | Bespoke UI frameworks  |

Using 3D techs in MAUI would require writing engine from scratch.

With this way we can use Unity:

 * Use of full fledged 3D engine.
 * Use of x-plat augmented reality features.

Unity has limitations so using MAUI is useful:

 * Unity is constrained to .NET 4.5 (very, very old -> 2012!!!)
 * With Unity, must use its own UI frameworks and cannot access platform UI features. This makes it hard, cumbersome to build modern UIs. Highly specialist skill-set to hire for.
 * Unity is deeply restricted when using 3rd party vendors (EG: Auth0 for identity)
 * 

Therefore combining these two techs means we can get the best of both worlds:

 * One of the industry leaders in games/3D techs.
 * Fully x-plat code base in a native app. A fully native UI with no restrictions!
 * .NET across the whole stack.

## Creating Unity Project

 * Outline steps to create a new unity project -> Link to sample included here

Sample is a ground plane with a few pieces of geometry + camera controls

## Exporting Unity Project

 * Outline how to configure the Unity project export (Android, iOS => IL2CPP + require runtimes)
 * Android => Export project tick box then click export

## Building Platform Binaries

After we've generated the platform specific projects, the next step is to create the binary artificats (`aar` for Android, `framework` for iOS) that we will reference in our MAUI app.

### Android

Building the Android aar using the generated gradle file.

 * Open in Android studio

Where to find the artifact created?

### iOS

Building the iOS framework using the generated xcode project.

Where to find

## Binding Platform Binaries

Now that our platform binaries have been built, lets create the platform specific .NET projects that will be utilised in the MAUI app.

### Android

Creating the Java binding project in VS Mac.
Drag and drop the unity aar into Jars folder.
Done! (.NET auto-generates the interop code, usually we don't need to do anyuthing further

### iOS

Creating the iOS binding project.
Drag and drop the framework into project.

Using ObjectiveSharpie to create our platform interop code.

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
 
 Linking to binding project.
 Creating an activity (either using the inbuilt unity launcher or through a custom activity) => Recomend custom binding, can copy the flags/settings 
 Launching this activity.

 Removing the AAR unity activity fom the manifest



### iOS

 Linking to binding project.
 Creating the unity view controller.
 Loading the unity runtime.
 Launchgint the view controller

## Communicating Between Unity and MAUI

More than likely need to send data between MAUI and Unity. To do so we create a data bridge to send stringly typed content.

 => Receving content from unity.

### Android

Implemneting the java bruidge and IContentReciever object/interface

### iOS

Implmenting IContentReceiver protocol.



### Maui

Implemnetingn receivers in MAUI


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

 * Sometimes can crash on Android when *receivinb* data from Unity when debugger attached. Does not happen in non-debugging builds.
 * Somestimes launchign unity engine can freeze app on Android. No known cause or solution yet, still investigating



## Summary

TODO: Summarise everything above (steps, complexities, why the heck we'd do this?)

Link out to LinkedIn + email/contact details if people need help doing this.

