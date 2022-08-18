using System;

namespace Drastic.UI
{
	public class PropertyChangingEventArgs : EventArgs
	{
		public PropertyChangingEventArgs(string propertyName)
		{
			PropertyName = propertyName;
		}

		public virtual string PropertyName { get; }
	}
}