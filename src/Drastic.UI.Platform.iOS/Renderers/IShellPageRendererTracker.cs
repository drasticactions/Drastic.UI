using System;
using UIKit;

namespace Drastic.UI.Platform.iOS
{
	public interface IShellPageRendererTracker : IDisposable
	{
		bool IsRootPage { get; set; }

		UIViewController ViewController { get; set; }

		Page Page { get; set; }
	}
}