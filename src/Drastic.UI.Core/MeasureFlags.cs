using System;

namespace Drastic.UI
{
	[Flags]
	public enum MeasureFlags
	{
		None = 0,
		IncludeMargins = 1 << 0
	}
}