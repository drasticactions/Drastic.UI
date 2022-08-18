using System.Collections.Generic;

namespace Drastic.UI
{
	public interface IViewContainer<T> where T : VisualElement
	{
		IList<T> Children { get; }
	}
}