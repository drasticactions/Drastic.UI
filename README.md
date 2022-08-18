# Drastic.UI

**Yet another "Quick And Dirty" Cross Platform Desktop Framework**, for macOS and Windows.

[![NuGet version (Drastic.UI)](https://img.shields.io/nuget/v/Drastic.UI.svg?style=flat-square)](https://www.nuget.org/packages/Drastic.UI/)

## What it's about:

Drastic UI is a cross-platform UI framework for macOS and Windows, intended for quickly hacking together UI. Unlike other frameworks that aim for beautiful UIs with an artistic vision that looks great on the app store, Drastic.UI is for the "I just need a button and a click handler" crowd. 

A direct port of the macOS Forms platforms, Drastic UI has all of its features. I've only changed the namespace (to not conflict with Xamarin Forms) and updated the implementations to work on .NET6+. All included controls should work as they do in Xamarin Forms 5, but honestly, I only cared about the grid and buttons. YMMV.

I do not intend Drastic UI as a "Production" framework. It's a hack that lets me quickly make a UI with an API I already know, which could be helpful for others in the same situation. 

## Setup:

Setting up the project is similar to setting up Xamarin.Forms on [macOS](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/platform/other/mac) or [WPF](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/platform/other/wpf).

- Setup your macOS (`dotnet new macos`) or WPF (`dotnet new wpf`) app.
- Reference the [Nuget](https://www.nuget.org/packages/Drastic.UI/) or Source Projects into that project. You can also reference it in a `netstandard2.0` project if you want to share the UI between them.
- If you want to use the full "Application" layer and treat it like a Xamarin.Forms app, reference the XF docs above and set up the `FormsApplication` scaffolding for the projects. You can reference the `samples` directory as well to see how to do it. The biggest differences are changing `Xamarin.Forms` to `Drastic.UI` and `Forms.Init()` to `UI.Init()`. This features the same limitations as Xamarin.Forms, as it's functionally the same thing.
- You can also use the "Native Embedding" feature to inject the Drastic.UI layouts into an existing layout. Again, this is the same as Xamarin.Forms. The [docs](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/platform/native-forms) for that apply to Drastic.UI as well.
- Of the two, I prefer using the Native Embedding model as it's the most flexable. But removing `Application` would take far more effort than its worth just for this project, and it still could be useful if you wanted to port an existing Xamarin.Forms app over.

## Differences from Xamarin.Forms:

- XAML support is not included. It _could_ work but figuring out how to get the build tasks working and bundled into the library was far too time consuming. For the work I wanted this for, I didn't need it anyway. However, it could be done. `Xamarin.Forms.XAML` itself is `netstandard2.0`, and while the build tasks from Forms use msbuild and .NET Framework specific commands, backporting changes from MAUI should make that work too. If anyone wants to do it, I wouldn't be against merging that in.
- The other Xamarin.Forms platforms (iOS, Android, etc.) are not included. Those were out of scope for what I wanted to do here (quickly hack on UI for desktops). You can port the [Xamarin.Forms stack to .NET 6](https://github.com/drasticactions/xamarin.forms) though if you really want to go down that road. At this point, however, I would move to MAUI or Avalonia if you really want a cross platform UI framework.