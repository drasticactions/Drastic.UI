using System;
using UIKit;

namespace Drastic.UI.Platform.iOS
{
	public interface IShellTabBarAppearanceTracker : IDisposable
	{
		void ResetAppearance(UITabBarController controller);
		void SetAppearance(UITabBarController controller, ShellAppearance appearance);
		void UpdateLayout(UITabBarController controller);
	}
}