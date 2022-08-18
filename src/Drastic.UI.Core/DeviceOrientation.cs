using System.ComponentModel;

namespace Drastic.UI.Internals
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public enum DeviceOrientation
	{
		Portrait,
		Landscape,
		PortraitUp,
		PortraitDown,
		LandscapeLeft,
		LandscapeRight,
		Other
	}
}