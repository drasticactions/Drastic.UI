using System;

namespace Drastic.UI.Xaml
{
	public interface IValueProvider
	{
		object ProvideValue(IServiceProvider serviceProvider);
	}
}