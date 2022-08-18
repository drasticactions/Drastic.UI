using System;

namespace Drastic.UI
{
	public class BackButtonPressedEventArgs : EventArgs
	{
		public bool Handled { get; set; }
	}
}