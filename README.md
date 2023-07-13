# UnityUaal.Maui
Embedding the Unity game engine into .NET MAUI.

What is Unity? What is UAAL? What is MAUI?

## Why?

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
 * 

## Creating Unity Project

 * Outline steps to create a new unity project -> Link to sample included here

## Exporting Unity Project

 * Outline how to configure the unity project export (Android, iOS => IL2CPP + require runtimes)

## Building Platform Binaries

### Android

Building the Android aar using the generated gradle file.

Where to find the artifact created?

### iOS

Building the iOS framework using the generated xcode project.

Where to find

## Binding Platform Binaries

### Android

Creating the Java binding project.

### iOS

Creating the 

## Integrating Into MAUI

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
 Creating an activity (either usin
 Launching this activity.

### iOS

 Linking to binding project.
 Creating the unity view controller.
 Loading the unity runtime.
 Launchgint the view controller

## Communicating Between Unity and MAUI

More than likely need to send data between.

 * Creating data sharing bridge in Unity (iOS and Android impl)
 * Implementing the data bridge in MAUI
 * Sending the data via Unity
 * Sending the data via Maui.

## Summary

TODO: Summarise everything above (steps, complexities, why the heck we'd do this?)
