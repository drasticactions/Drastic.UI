using System;

namespace Drastic.UI
{
	public class ElementEventArgs : EventArgs
	{
		public ElementEventArgs(Element element) => Element = element ?? throw new ArgumentNullException(nameof(element));

		public Element Element { get; private set; }
	}
}