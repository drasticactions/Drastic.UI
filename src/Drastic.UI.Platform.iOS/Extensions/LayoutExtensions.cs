using System.Collections.Generic;
using CoreGraphics;
#if __MOBILE__
using UIKit;
using Drastic.UI.Platform.iOS;
namespace Drastic.UI.Platform.iOS
#else
using AppKit;
using UIView = AppKit.NSView;
using Drastic.UI.Platform.MacOS;

namespace Drastic.UI.Platform.MacOS
#endif
{
	public delegate SizeRequest? GetDesiredSizeDelegate(
		NativeViewWrapperRenderer renderer, double widthConstraint, double heightConstraint);

	public delegate CGSize? SizeThatFitsDelegate(CGSize size);

	public delegate bool LayoutSubviewsDelegate();

	public static class LayoutExtensions
	{
		public static void Add(this IList<View> children, UIView view, GetDesiredSizeDelegate getDesiredSizeDelegate = null,
			SizeThatFitsDelegate sizeThatFitsDelegate = null,
			LayoutSubviewsDelegate layoutSubViews = null)
		{
			children.Add(view.ToView(getDesiredSizeDelegate, sizeThatFitsDelegate, layoutSubViews));
		}

		public static View ToView(this UIView view, GetDesiredSizeDelegate getDesiredSizeDelegate = null,
			SizeThatFitsDelegate sizeThatFitsDelegate = null, LayoutSubviewsDelegate layoutSubViews = null)
		{
			return new NativeViewWrapper(view, getDesiredSizeDelegate, sizeThatFitsDelegate, layoutSubViews);
		}
	}
}