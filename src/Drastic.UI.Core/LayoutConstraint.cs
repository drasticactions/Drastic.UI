using System;

namespace Drastic.UI
{
	[Flags]
	internal enum LayoutConstraint
	{
		None = 0,
		HorizontallyFixed = 1 << 0,
		VerticallyFixed = 1 << 1,
		Fixed = HorizontallyFixed | VerticallyFixed
	}
}