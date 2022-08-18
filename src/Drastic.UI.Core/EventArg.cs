using System;
using System.ComponentModel;

namespace Drastic.UI.Internals
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class EventArg<T> : EventArgs
	{
		// Property variable

		// Constructor
		public EventArg(T data)
		{
			Data = data;
		}

		// Property for EventArgs argument
		public T Data { get; }
	}
}