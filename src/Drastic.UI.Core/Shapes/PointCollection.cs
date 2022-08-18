using System.Collections.ObjectModel;

namespace Drastic.UI.Shapes
{
	[TypeConverter(typeof(PointCollectionConverter))]
	public sealed class PointCollection : ObservableCollection<Point>
	{

	}
}