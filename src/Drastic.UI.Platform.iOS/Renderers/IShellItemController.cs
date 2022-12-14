﻿using System;
using UIKit;

namespace Drastic.UI.Platform.iOS
{
	public interface IShellItemRenderer : IDisposable
	{
		ShellItem ShellItem { get; set; }

		UIViewController ViewController { get; }
	}
}