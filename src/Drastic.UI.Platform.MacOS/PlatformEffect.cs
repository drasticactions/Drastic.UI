#if __MOBILE__
using UIKit;
namespace Drastic.UI.Platform.iOS
#else
using UIView = AppKit.NSView;

namespace Drastic.UI.Platform.MacOS
#endif
{
	public abstract class PlatformEffect : PlatformEffect<UIView, UIView>
	{
	}
}