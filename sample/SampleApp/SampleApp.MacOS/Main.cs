using SampleApp.MacOS;

// This is the main entry point of the application.
NSApplication.Init();
NSApplication.SharedApplication.Delegate = new AppDelegate(); // add this line
NSApplication.Main(args);
