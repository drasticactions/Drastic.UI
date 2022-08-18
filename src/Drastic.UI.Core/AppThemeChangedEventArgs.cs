using System;

namespace Drastic.UI
{
	public class AppThemeChangedEventArgs : EventArgs
	{
		public AppThemeChangedEventArgs(OSAppTheme appTheme) =>
			RequestedTheme = appTheme;

		public OSAppTheme RequestedTheme { get; }
	}
}