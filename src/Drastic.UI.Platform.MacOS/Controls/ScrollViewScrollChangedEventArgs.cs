using System;
using PointF = CoreGraphics.CGPoint;

namespace Drastic.UI.Platform.MacOS
{
	internal class ScrollViewScrollChangedEventArgs : EventArgs
	{
		public PointF CurrentScrollPoint { get; set; }
	}
}