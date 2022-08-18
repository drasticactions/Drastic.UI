using System.ComponentModel;

namespace Drastic.UI
{
	[TypeConverter(typeof(ItemsLayoutTypeConverter))]
	public interface IItemsLayout : INotifyPropertyChanged { }
}