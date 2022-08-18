using System;

#if __MOBILE__
namespace Drastic.UI.Platform.iOS
#else

namespace Drastic.UI.Platform.MacOS
#endif
{
	public class VisualElementChangedEventArgs : ElementChangedEventArgs<VisualElement>
	{
		public VisualElementChangedEventArgs(VisualElement oldElement, VisualElement newElement)
			: base(oldElement, newElement)
		{
		}
	}

	public class ElementChangedEventArgs<TElement> : EventArgs where TElement : Element
	{
		public ElementChangedEventArgs(TElement oldElement, TElement newElement)
		{
			OldElement = oldElement;
			NewElement = newElement;
		}

		public TElement NewElement { get; private set; }

		public TElement OldElement { get; private set; }
	}
}