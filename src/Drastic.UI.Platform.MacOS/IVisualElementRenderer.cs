using System;
#if __MOBILE__
using UIKit;
using NativeView = UIKit.UIView;
using NativeViewController = UIKit.UIViewController;

namespace Drastic.UI.Platform.iOS
#else
using NativeView = AppKit.NSView;
using NativeViewController = AppKit.NSViewController;

namespace Drastic.UI.Platform.MacOS
#endif
{
	public interface IVisualElementRenderer : IDisposable, IRegisterable
	{
		VisualElement Element { get; }

		NativeView NativeView { get; }

		NativeViewController ViewController { get; }

		event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint);

		void SetElement(VisualElement element);

		void SetElementSize(Size size);
	}
}