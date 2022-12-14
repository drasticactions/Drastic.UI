using System;
using UIKit;

namespace Drastic.UI.Platform.iOS
{
	public interface IShellSectionRenderer : IDisposable
	{
		bool IsInMoreTab { get; set; }
		ShellSection ShellSection { get; set; }
		UIViewController ViewController { get; }
	}
}