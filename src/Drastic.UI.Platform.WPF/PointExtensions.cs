#if WINDOWS_UWP
using WPoint = Windows.Foundation.Point;

namespace Drastic.UI.Platform.UWP
#else
using WPoint = System.Windows.Point;

namespace Drastic.UI.Platform.WPF
#endif
{
	public static class PointExtensions
	{
		public static WPoint ToWindows(this Point point)
		{
			return new WPoint(point.X, point.Y);
		}
	}
}