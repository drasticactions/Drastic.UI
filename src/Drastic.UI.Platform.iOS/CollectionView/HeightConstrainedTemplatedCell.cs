﻿using CoreGraphics;
using Foundation;

namespace Drastic.UI.Platform.iOS
{
	internal abstract partial class HeightConstrainedTemplatedCell : TemplatedCell
	{
		[Export("initWithFrame:")]
		[Internals.Preserve(Conditional = true)]
		public HeightConstrainedTemplatedCell(CGRect frame) : base(frame)
		{
		}

		public override void ConstrainTo(CGSize constraint)
		{
			ClearConstraints();
			ConstrainedDimension = constraint.Height;
		}

		protected override (bool, Size) NeedsContentSizeUpdate(Size currentSize)
		{
			var size = Size.Zero;

			if (VisualElementRenderer?.Element == null)
			{
				return (false, size);
			}

			var bounds = VisualElementRenderer.Element.Bounds;

			if (bounds.Width <= 0 || bounds.Height <= 0)
			{
				return (false, size);
			}

			var desiredBounds = VisualElementRenderer.Element.Measure(double.PositiveInfinity, bounds.Height, 
				MeasureFlags.IncludeMargins);

			if (desiredBounds.Request.Width == currentSize.Width)
			{
				// Nothing in the cell needs more room, so leave it as it is
				return (false, size);
			}

			return (true, desiredBounds.Request);
		}
	}
}