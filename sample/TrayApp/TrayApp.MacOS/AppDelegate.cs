// Based on https://github.com/krdmllr/CrossTrayApplicationSample/blob/master/CrossTrayApplicationSample.MacOS/AppDelegate.cs
using AppKit;
using Drastic.UI;
using Drastic.UI.Platform.MacOS;

namespace TrayApp.MacOS;

[Register ("AppDelegate")]
public class AppDelegate : NSApplicationDelegate {

    public AppDelegate()
    {
        Drastic.UI.UI.Init();
    }

    public override void DidFinishLaunching (NSNotification notification)
	{
        CreateStatusItem();
        Application.SetCurrentApplication(new App());
    }

	public override void WillTerminate (NSNotification notification)
	{
		// Insert code here to tear down your application
	}

    private NSStatusItem _statusBarItem;
    private NSMenu _menu;
    private NSViewController mainPage;

    private void CreateStatusItem()
    {
        // Create the status bar item
        NSStatusBar statusBar = NSStatusBar.SystemStatusBar;
        _statusBarItem = statusBar.CreateStatusItem(NSStatusItemLength.Variable);
        _statusBarItem.Button.Image = NSImage.ImageNamed("TrayIcon.ico");

        // Listen to touches on the status bar item
        _statusBarItem.SendActionOn(NSTouchPhase.Any);
        _statusBarItem.Button.Activated += this.StatusItemActivated;

        // Create the menu that gets opened on a right click
        _menu = new NSMenu();
        var closeAppItem = new NSMenuItem("Close");
        closeAppItem.Activated += CloseAppItem_Activated;
        _menu.AddItem(closeAppItem);
    }

    private void StatusItemActivated(object sender, EventArgs e)
    {
        var currentEvent = NSApplication.SharedApplication.CurrentEvent;
        switch (currentEvent.Type)
        {
            case NSEventType.LeftMouseDown:
                ShowWindow();
                break;
            case NSEventType.RightMouseDown:
                _statusBarItem.PopUpStatusItemMenu(_menu);
                break;
        }
    }

    private void ShowWindow()
    {
        if (mainPage == null)
        {
            mainPage = Application.Current.MainPage.CreateViewController();
            mainPage.View.Frame = new CoreGraphics.CGRect(0, 0, 400, 400);
            Application.Current.SendStart();
        }
        else
        {
            Application.Current.SendResume();
        }

        var popover = new NSPopover
        {
            ContentViewController = mainPage,
            Behavior = NSPopoverBehavior.Transient,
            Delegate = new NSPopoverDelegate()
        };
        popover.Show(_statusBarItem.Button.Bounds, _statusBarItem.Button, NSRectEdge.MaxYEdge);
    }

    private void CloseAppItem_Activated(object? sender, EventArgs e)
    {
    }
}
