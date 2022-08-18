﻿using System;

namespace Drastic.UI
{
	public class PositionChangedEventArgs : EventArgs
	{
		public int PreviousPosition { get; }
		public int CurrentPosition { get; }

		internal PositionChangedEventArgs(int previousPosition, int currentPosition)
		{
			PreviousPosition = previousPosition;
			CurrentPosition = currentPosition;
		}
	}
}
