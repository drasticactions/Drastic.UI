using System;

namespace Drastic.UI
{
	public class TappedEventArgs : EventArgs
	{
		public TappedEventArgs(object parameter)
		{
			Parameter = parameter;
		}

		public object Parameter { get; private set; }
	}
}