using System;

namespace Drastic.UI
{
	public class SelectedPositionChangedEventArgs : EventArgs
	{
		public SelectedPositionChangedEventArgs(int selectedPosition)
		{
			SelectedPosition = selectedPosition;
		}

		public object SelectedPosition { get; private set; }
	}
}