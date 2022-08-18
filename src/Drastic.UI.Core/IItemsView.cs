using System;
using System.Collections;

namespace Drastic.UI
{
	public interface IItemsView<T> where T : BindableObject
	{
		T CreateDefault(object item);
		void SetupContent(T content, int index);
		void UnhookContent(T content);
	}
}