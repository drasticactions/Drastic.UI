using System;
using CoreGraphics;
using PointF = CoreGraphics.CGPoint;
using RectangleF = CoreGraphics.CGRect;
using SizeF = CoreGraphics.CGSize;
#if __MOBILE__
using UIKit;
namespace Drastic.UI.Platform.iOS
#else
using AppKit;
using UIColor = AppKit.NS;

namespace Drastic.UI.Platform.MacOS
#endif
{
	// Ignore obsolete Xamarin.iOS colors for now
	// until this Xamarin.iOS version has been adopted more widely
	// This is just a change in Xamarin.iOS not UIKit
#pragma warning disable CS0618 // Type or member is obsolete
	public static class ColorExtensions
	{
#if __MOBILE__
		internal static readonly UIColor Black = UIColor.Black;
		internal static readonly UIColor SeventyPercentGrey = new UIColor(0.7f, 0.7f, 0.7f, 1);

		internal static UIColor LabelColor
		{
			get
			{
				if (Forms.IsiOS13OrNewer)
					return UIColor.Label;

				return UIColor.Black;
			}
		}

		internal static UIColor PlaceholderColor
		{
			get
			{
				if (Forms.IsiOS13OrNewer)
					return UIColor.PlaceholderText;

				return SeventyPercentGrey;
			}
		}

		internal static UIColor SecondaryLabelColor
		{
			get
			{
				if (Forms.IsiOS13OrNewer)
					return UIColor.SecondaryLabel;

				return new Color(.32, .4, .57).ToUIColor();
			}
		}

		internal static UIColor BackgroundColor
		{
			get
			{
				if (Forms.IsiOS13OrNewer)
					return UIColor.SystemBackground;

				return UIColor.White;
			}
		}

		internal static UIColor SeparatorColor
		{
			get
			{
				if (Forms.IsiOS13OrNewer)
					return UIColor.Separator;

				return UIColor.Gray;
			}
		}

		internal static UIColor OpaqueSeparatorColor
		{
			get
			{
				if (Forms.IsiOS13OrNewer)
					return UIColor.OpaqueSeparator;

				return UIColor.Black;
			}
		}

		internal static UIColor GroupedBackground
		{
			get
			{
				if (Forms.IsiOS13OrNewer)
					return UIColor.SystemGroupedBackground;

				return new UIColor(247f / 255f, 247f / 255f, 247f / 255f, 1);
			}
		}

		internal static UIColor AccentColor
		{
			get
			{
				if (Forms.IsiOS13OrNewer)
					return UIColor.SystemBlue;

				return Color.FromRgba(50, 79, 133, 255).ToUIColor();
			}
		}

		internal static UIColor Red
		{
			get
			{
				if (Forms.IsiOS13OrNewer)
					return UIColor.SystemRed;

				return UIColor.FromRGBA(255, 0, 0, 255);
			}
		}

		internal static UIColor Gray
		{
			get
			{
				if (Forms.IsiOS13OrNewer)
					return UIColor.SystemGray;

				return UIColor.Gray;
			}
		}

		internal static UIColor LightGray
		{
			get
			{
				if (Forms.IsiOS13OrNewer)
					return UIColor.SystemGray2;
				return UIColor.LightGray;

			}
		}

#else
		internal static readonly NSColor Black = NSColor.Black;
		internal static readonly NSColor SeventyPercentGrey = NSColor.FromRgba(0.7f, 0.7f, 0.7f, 1);
		internal static readonly NSColor AccentColor = Color.FromRgba(50, 79, 133, 255).ToNSColor();

		internal static NSColor LabelColor
		{
			get
			{
				if (Forms.IsMojaveOrNewer)
					return NSColor.Label;

				return NSColor.Black.UsingColorSpace("NSCalibratedRGBColorSpace");
			}
		}

		internal static NSColor TextColor
		{
			get
			{
				if (Forms.IsMojaveOrNewer)
					return NSColor.Text;

				return NSColor.Black;
			}
		}

		internal static NSColor ControlBackgroundColor
		{
			get
			{
				if (Forms.IsMojaveOrNewer)
					return NSColor.ControlBackground;

				return NSColor.Clear;
			}
		}

		internal static NSColor WindowBackgroundColor
		{
			get
			{
				if (Forms.IsMojaveOrNewer)
					return NSColor.WindowBackground;

				return NSColor.White;
			}
		}

		internal static NSColor PlaceholderColor
		{
			get
			{
				if (Forms.IsMojaveOrNewer)
					return NSColor.PlaceholderText;

				return SeventyPercentGrey;
			}
		}

		internal static NSColor SecondaryLabelColor
		{
			get
			{
				if (Forms.IsMojaveOrNewer)
					return NSColor.SecondaryLabel;

				return new Color(.32, .4, .57).ToNSColor();
			}
		}

		internal static NSColor GroupedBackground
		{
			get
			{
				if (Forms.IsMojaveOrNewer)
					return NSColor.SystemGray;

				return Color.LightGray.ToNSColor();
			}
		}
#pragma warning restore CS0618 // Type or member is obsolete

#endif

		public static CGColor ToCGColor(this Color color)
		{
#if __MOBILE__
			return color.ToUIColor().CGColor;
#else
            return color.ToNSColor().CGColor;
#endif
        }

#if __MOBILE__
        public static UIColor FromPatternImageFromBundle(string bgImage)
		{
			var image = UIImage.FromBundle(bgImage);
			if (image == null)
				return UIColor.White;

			return UIColor.FromPatternImage(image);
		}
#endif

		public static Color ToColor(this UIColor color)
		{
			nfloat red;
			nfloat green;
			nfloat blue;
			nfloat alpha;
#if __MOBILE__
			color.GetRGBA(out red, out green, out blue, out alpha);
#else
			if (color.Type == NSColorType.Catalog)
				throw new InvalidOperationException("Cannot convert a NSColorType.Catalog color without specifying the color space, use the overload to specify an NSColorSpace");

			color.GetRgba(out red, out green, out blue, out alpha);
#endif
			return new Color(red, green, blue, alpha);
		}

#if __MACOS__
		public static Color ToColor(this UIColor color, NSColorSpace colorSpace)
		{
			var convertedColor = color.UsingColorSpace(colorSpace);

			return convertedColor.ToColor();
		}
#endif

#if __MOBILE__
		public static UIColor ToUIColor(this Color color)
		{
			return new UIColor((float)color.R, (float)color.G, (float)color.B, (float)color.A);
		}

		public static UIColor ToUIColor(this Color color, Color defaultColor)
		{
			if (color.IsDefault)
				return defaultColor.ToUIColor();

			return color.ToUIColor();
		}

		public static UIColor ToUIColor(this Color color, UIColor defaultColor)
		{
			if (color.IsDefault)
				return default;

			return color.ToUIColor();
		}
#else
		public static NSColor ToNSColor(this Color color)
		{
			return NSColor.FromRgba((float)color.R, (float)color.G, (float)color.B, (float)color.A);
		}

		public static NSColor ToNSColor(this Color color, Color defaultColor)
		{
			if (color.IsDefault)
				return defaultColor.ToNSColor();

			return color.ToNSColor();
		}

		public static NSColor ToNSColor(this Color color, NSColor defaultColor)
		{
			if (color.IsDefault)
				return default;

			return color.ToNSColor();
		}
#endif
	}

	public static class PointExtensions
	{
		public static Point ToPoint(this PointF point)
		{
			return new Point(point.X, point.Y);
		}

		public static PointF ToPointF(this Point point)
		{
			return new PointF(point.X, point.Y);
		}
	}

	public static class SizeExtensions
	{
		public static SizeF ToSizeF(this Size size)
		{
			return new SizeF((float)size.Width, (float)size.Height);
		}
	}

	public static class RectangleExtensions
	{
		public static Rectangle ToRectangle(this RectangleF rect)
		{
			return new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static RectangleF ToRectangleF(this Rectangle rect)
		{
			return new RectangleF((nfloat)rect.X, (nfloat)rect.Y, (nfloat)rect.Width, (nfloat)rect.Height);
		}
	}
}