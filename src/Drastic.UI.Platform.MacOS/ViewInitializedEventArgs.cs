using System;
#if __MOBILE__
using TNativeView = UIKit.UIView;
#else
using TNativeView = AppKit.NSView;

#endif

namespace Drastic.UI
{
	public class ViewInitializedEventArgs : EventArgs
	{
		public TNativeView NativeView { get; internal set; }

		public VisualElement View { get; internal set; }
	}
}