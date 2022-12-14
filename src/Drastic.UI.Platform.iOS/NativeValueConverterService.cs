using System;
using Drastic.UI.Xaml.Internals;
using Drastic.UI.Internals;
#if __MOBILE__
using UIKit;

[assembly: Drastic.UI.Dependency(typeof(Drastic.UI.Platform.iOS.NativeValueConverterService))]
namespace Drastic.UI.Platform.iOS
#else
using UIView = AppKit.NSView;

[assembly: Drastic.UI.Dependency(typeof(Drastic.UI.Platform.MacOS.NativeValueConverterService))]

namespace Drastic.UI.Platform.MacOS
#endif
{
	[Preserve(AllMembers = true)]
	class NativeValueConverterService : INativeValueConverterService
	{
		public bool ConvertTo(object value, Type toType, out object nativeValue)
		{
			nativeValue = null;
			if (typeof(UIView).IsInstanceOfType(value) && toType.IsAssignableFrom(typeof(View)))
			{
				nativeValue = ((UIView)value).ToView();
				return true;
			}
			return false;
		}
	}
}