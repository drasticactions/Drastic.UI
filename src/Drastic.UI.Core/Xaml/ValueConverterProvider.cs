using System;
using System.Reflection;

using Drastic.UI;
using Drastic.UI.Xaml;

[assembly: Dependency(typeof(ValueConverterProvider))]
namespace Drastic.UI.Xaml
{
	class ValueConverterProvider : IValueConverterProvider
	{
		public object Convert(object value, Type toType, Func<MemberInfo> minfoRetriever, IServiceProvider serviceProvider)
		{
			var ret = value.ConvertTo(toType, minfoRetriever, serviceProvider, out Exception exception);
			if (exception != null)
			{
				var lineInfo = (serviceProvider.GetService(typeof(IXmlLineInfoProvider)) is IXmlLineInfoProvider lineInfoProvider) ? lineInfoProvider.XmlLineInfo : new XmlLineInfo();
				throw new XamlParseException(exception.Message, serviceProvider, exception);
			}
			return ret;
		}
	}
}
