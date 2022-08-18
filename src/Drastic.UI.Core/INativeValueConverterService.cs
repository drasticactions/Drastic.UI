using System;

namespace Drastic.UI.Xaml.Internals
{
	public interface INativeValueConverterService
	{
		bool ConvertTo(object value, Type toType, out object nativeValue);
	}
}