using System;
using UIKit;

namespace Drastic.UI.Platform.iOS
{
	public interface IButtonLayoutRenderer
	{
		UIButton Control { get; }
		Button Element { get; }
		IImageVisualElementRenderer ImageVisualElementRenderer { get; }
		nfloat MinimumHeight { get; }
		event EventHandler<ElementChangedEventArgs<Button>> ElementChanged;
	}
}
