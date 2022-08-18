using System.Collections.ObjectModel;

namespace Drastic.UI
{
	internal class ElementCollection<T> : ObservableWrapper<Element, T> where T : Element
	{
		public ElementCollection(ObservableCollection<Element> list) : base(list)
		{
		}
	}
}