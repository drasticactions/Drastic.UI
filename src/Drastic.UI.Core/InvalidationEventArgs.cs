using System;
using Drastic.UI.Internals;

namespace Drastic.UI
{
	internal class InvalidationEventArgs : EventArgs
	{
		public InvalidationEventArgs(InvalidationTrigger trigger)
		{
			Trigger = trigger;
		}

		public InvalidationTrigger Trigger { get; private set; }
	}
}