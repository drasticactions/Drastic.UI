using Drastic.UI;
using Drastic.UI.Platform.MacOS;

namespace SampleApp.MacOS;

[Register ("AppDelegate")]
public class AppDelegate : FormsApplicationDelegate
{
    NSWindow window;
    public AppDelegate()
    {
        var style = NSWindowStyle.Closable | NSWindowStyle.Resizable | NSWindowStyle.Titled;

        var rect = new CoreGraphics.CGRect(200, 1000, 1024, 768);
        window = new NSWindow(rect, style, NSBackingStore.Buffered, false);
        window.Title = "Drastic.UI";
        window.TitleVisibility = NSWindowTitleVisibility.Hidden;
    }

    public override NSWindow MainWindow
    {
        get { return window; }
    }

    public override void DidFinishLaunching(NSNotification notification)
    {
        UI.Init();
        LoadApplication(new App());
        base.DidFinishLaunching(notification);
    }
}
