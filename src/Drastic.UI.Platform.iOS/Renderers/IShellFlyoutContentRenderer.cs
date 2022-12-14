using System;
using UIKit;

namespace Drastic.UI.Platform.iOS
{
	public interface IShellFlyoutContentRenderer
	{
		UIViewController ViewController { get; }

		event EventHandler WillAppear;

		event EventHandler WillDisappear;
	}
}