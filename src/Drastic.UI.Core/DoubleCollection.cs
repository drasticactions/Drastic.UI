using System.Collections.ObjectModel;

namespace Drastic.UI
{
	[TypeConverter(typeof(DoubleCollectionConverter))]
	public sealed class DoubleCollection : ObservableCollection<double>
	{

	}
}