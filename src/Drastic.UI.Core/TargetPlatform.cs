using System;
using System.ComponentModel;

namespace Drastic.UI
{
	[Obsolete]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public enum TargetPlatform
	{
		Other,
		iOS,
		Android,
		WinPhone,
		Windows
	}
}
