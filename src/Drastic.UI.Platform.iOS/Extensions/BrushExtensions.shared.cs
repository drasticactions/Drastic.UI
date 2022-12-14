﻿using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using Foundation;

#if __MOBILE__
namespace Drastic.UI.Platform.iOS
#else
namespace Drastic.UI.Platform.MacOS
#endif
{
	public partial class BrushExtensions
	{
		static CGPoint GetRadialGradientBrushEndPoint(Point startPoint, double radius)
		{
			double x = startPoint.X == 1 ? (startPoint.X - radius) : (startPoint.X + radius);

			if (x < 0)
				x = 0;

			if (x > 1)
				x = 1;

			double y = startPoint.Y == 1 ? (startPoint.Y - radius) : (startPoint.Y + radius);

			if (y < 0)
				y = 0;

			if (y > 1)
				y = 1;

			return new CGPoint(x, y);
		}

		static NSNumber[] GetCAGradientLayerLocations(List<GradientStop> gradientStops)
		{
			if (gradientStops == null || gradientStops.Count == 0)
				return new NSNumber[0];

			if (gradientStops.Count > 1 && gradientStops.Any(gt => gt.Offset != 0))
				return gradientStops.Select(x => new NSNumber(x.Offset)).ToArray();
			else
			{
				int itemCount = gradientStops.Count;
				int index = 0;
				float step = 1.0f / itemCount;

				NSNumber[] locations = new NSNumber[itemCount];

				foreach (var gradientStop in gradientStops)
				{
					float location = step * index;
					bool setLocation = !gradientStops.Any(gt => gt.Offset > location);

					if (gradientStop.Offset == 0 && setLocation)
						locations[index] = new NSNumber(location);
					else
						locations[index] = new NSNumber(gradientStop.Offset);

					index++;
				}

				return locations;
			}
		}

		static CGColor[] GetCAGradientLayerColors(List<GradientStop> gradientStops)
		{
			if (gradientStops == null || gradientStops.Count == 0)
				return new CGColor[0];

			CGColor[] colors = new CGColor[gradientStops.Count];

			int index = 0;
			foreach (var gradientStop in gradientStops)
			{
				if (gradientStop.Color == Color.Transparent)
				{
					var color = gradientStops[index == 0 ? index + 1 : index - 1].Color;
					CGColor nativeColor;
#if __MOBILE__
					nativeColor = color.ToUIColor().ColorWithAlpha(0.0f).CGColor;
#else
					nativeColor = color.ToNSColor().ColorWithAlphaComponent(0.0f).CGColor;
#endif
					colors[index] = nativeColor;
				}
				else
					colors[index] = gradientStop.Color.ToCGColor();

				index++;
			}

			return colors;
		}
	}
}
