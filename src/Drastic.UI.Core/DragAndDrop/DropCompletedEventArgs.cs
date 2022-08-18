using System;

namespace Drastic.UI
{
	public class DropCompletedEventArgs : EventArgs
	{
		DataPackageOperation DropResult { get; }
	}
}
