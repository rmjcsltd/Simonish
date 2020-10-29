# Simonish

One of the best ways to learn something is to do it, so I've done a little Microsoft Xamarin Forms development to get a feel for what it can do. I've created a small game called Simonish and taken it all the way through to publishing it on the [Google Play Store](https://play.google.com/store/apps/details?id=com.rmjcs.simonish) for Android. Whilst Xamarin Forms can also target IOS apps I'm not an Apple guy so I'll leave the Apple App Store for another time.

The game looks a bit like the classic electronic game Simon (originally launched by Milton Bradley in 1978) but is a lot easier to play, just requiring quick responses rather than a good memory.

# Development

This solution requires Visual Studio 2019. I use ReSharper but it should not be required. The projects are;
- Simonish - the Xamarin Forms class library, all of the app code is in here.
- Simonish.Android - the Android project, almost unmodified apart from the manifest and app icon.
- Simonish.IOS - the IOS project, unused so far, kept for future use.
- UITests - UI tests against the Android app.
- UnitTests - NUnit unit tests against the Simonish project.

The Simonish project follows an MVVM pattern. There are folders for;
- Pages - (AKA views) XAML UI only, no code (almost), use XAML binding to connect to ViewModels.
- ViewModels - expose bindable properties and actions for pages to use. Can have UI related logic but not business (i.e. non-UI) logic. ViewModels use a service to handle business logic.
- Services - raise events to communicate with their owners. Used for business logic, has no notion of a UI. Services use models to hold data.
- Models - data, any code is validating or protecting the data.
- Helpers - support classes.

Throughout the app the following terminology is used;
- Game - one cycle of the countdown -> playing -> game over phases.
- Phase - the different stages of gameplay; launched, countdown, playing and game over.
- Start - begin a new game.
- Countdown - the 3-2-1 after Start before the first target is lit.
- Play - the action phase of the game, after the countdown, until time runs out.
- Target - the currently lit button.
- Hit - the action verb for the game.
- Result - the output of a game. 

## Running the UnitTests
The UnitTests are not configured to build under the UITests build configuration so use Debug or Release.

## Running the UITests
The UITests were originally problematic but the following approach to running them has been reliable.
1. Switch to UITest configuration.
3. Run (F5) the app - this will build and deploy it to the emulator. Once the app is running in the emulator just quit.
3. Run the tests in UITests project. If the tests don't run in the ReSharper test runner, try the Visual Studio test runner. 

Note: The Simonish.Android project is not referenced by the UITests project - it's not necessary if the above steps are followed and it causes issues because the target frameworks are incompatible.

## Useful Links

Styling for Multiple Device Resolutions
https://devblogs.microsoft.com/xamarin/styling-for-multiple-device-resolutions/

Google, Material Design, Tools for picking colors
https://material.io/design/color/the-color-system.html#tools-for-picking-colors

# Release

The steps to build and release a new version of the app are as follows;
1. Update the Version Number and Version Name in the Android Manifest (Android project properties), and the Package Version in Simonish project properties.
2. Switch to Release configuration, clean solution, delete bin and obj in Android project, build solution.
3. Simonish.Android project, right click, Archive... archive will be created.
4. Distribute new archive, Google Play.
5. Select the keystore, select the Google Play account.
6. Choose the Internal track, then upload. The password for the keystore is in the AKS notes.
7. Complete the rollout of the new upload in the Google Play Console.

Note: In the Simonish.Android options don't set the linker to 'all' because it removes too much.

In the [Google Play Console](https://play.google.com/console/developers/6459310918995459273/app/4975510888703776554/app-dashboard?timespan=thirtyDays) pre-launch report, there are a couple of Non-SDK API warnings - these are a known issue, not something to worry about at the moment. See https://github.com/xamarin/Xamarin.Forms/issues/4118.

Use [Firebase](https://console.firebase.google.com/project/api-6459310918995459273-282239/overview) for more device testing.


# License
Licensed under the MIT license.
