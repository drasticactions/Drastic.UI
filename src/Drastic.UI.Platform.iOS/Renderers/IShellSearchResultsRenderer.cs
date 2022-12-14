using System;
using UIKit;

namespace Drastic.UI.Platform.iOS
{
	public interface IShellSearchResultsRenderer : IDisposable
	{
		UIViewController ViewController { get; }

		SearchHandler SearchHandler { get; set; }

		event EventHandler<object> ItemSelected;
	}
}