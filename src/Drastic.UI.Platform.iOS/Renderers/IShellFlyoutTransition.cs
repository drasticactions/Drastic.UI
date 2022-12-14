using CoreGraphics;
using System;
using UIKit;

namespace Drastic.UI.Platform.iOS
{
	public interface IShellFlyoutTransition
	{
		void LayoutViews(CGRect bounds, nfloat openPercent, UIView flyout, UIView shell, FlyoutBehavior behavior);
	}
}