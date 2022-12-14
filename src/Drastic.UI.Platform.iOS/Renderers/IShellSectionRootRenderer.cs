using System;
using UIKit;

namespace Drastic.UI.Platform.iOS
{
	public interface IShellSectionRootRenderer : IDisposable
	{
		bool ShowNavBar { get; }

		UIViewController ViewController { get; }
	}
}