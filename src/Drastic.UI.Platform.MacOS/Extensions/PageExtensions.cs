using System;
using AppKit;

namespace Drastic.UI.Platform.MacOS
{
	public static class PageExtensions
	{
		public static NSViewController CreateViewController(this Page view)
		{
			if (!UI.IsInitialized)
				throw new InvalidOperationException("call UI.Init() before this");

			if (!(view.RealParent is Application))
			{
				Application app = new DefaultApplication();
				app.MainPage = view;
			}

			var result = new Platform();
			result.SetPage(view);
			return result.ViewController;
		}

		class DefaultApplication : Application
		{
		}
	}
}