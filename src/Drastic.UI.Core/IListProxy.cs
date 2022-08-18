
using System.Collections;
using System.Collections.Specialized;

namespace Drastic.UI
{
	public interface IListProxy : IList
	{
		event NotifyCollectionChangedEventHandler CollectionChanged;
		IEnumerable ProxiedEnumerable { get; }
		bool TryGetValue(int index, out object value);
	}
}
