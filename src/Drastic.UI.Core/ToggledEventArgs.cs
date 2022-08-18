using System;

namespace Drastic.UI
{
	public class ToggledEventArgs : EventArgs
	{
		public ToggledEventArgs(bool value)
		{
			Value = value;
		}

		public bool Value { get; private set; }
	}
}