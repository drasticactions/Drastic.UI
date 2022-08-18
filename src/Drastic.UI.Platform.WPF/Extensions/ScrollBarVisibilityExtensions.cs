using WpfScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility;

namespace Drastic.UI.Platform.WPF.Extensions
{
	static class ScrollBarVisibilityExtensions
	{
		internal static WpfScrollBarVisibility ToWpfScrollBarVisibility(this ScrollBarVisibility visibility)
		{
			switch (visibility)
			{
				case ScrollBarVisibility.Always:
					return WpfScrollBarVisibility.Visible;
				case ScrollBarVisibility.Default:
					return WpfScrollBarVisibility.Auto;
				case ScrollBarVisibility.Never:
					return WpfScrollBarVisibility.Hidden;
				default:
					return WpfScrollBarVisibility.Auto;
			}
		}
	}
}