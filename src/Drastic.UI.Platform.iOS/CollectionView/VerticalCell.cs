﻿using CoreGraphics;
using Foundation;
using UIKit;

namespace Drastic.UI.Platform.iOS
{
	internal sealed class VerticalCell : WidthConstrainedTemplatedCell
	{
		public static NSString ReuseId = new NSString("Drastic.UI.Platform.iOS.VerticalCell");

		[Export("initWithFrame:")]
		[Internals.Preserve(Conditional = true)]
		public VerticalCell(CGRect frame) : base(frame)
		{
		}

		public override CGSize Measure()
		{
			var measure = VisualElementRenderer.Element.Measure(ConstrainedDimension,
				double.PositiveInfinity, MeasureFlags.IncludeMargins);

			return new CGSize(ConstrainedDimension, measure.Request.Height);
		}

		protected override bool AttributesConsistentWithConstrainedDimension(UICollectionViewLayoutAttributes attributes)
		{
			return attributes.Frame.Width == ConstrainedDimension;
		}
	}
}