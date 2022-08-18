using System;
using System.Reflection;

namespace Drastic.UI.Xaml
{
	interface IValueConverterProvider
	{
		object Convert(object value, Type toType, Func<MemberInfo> minfoRetriever, IServiceProvider serviceProvider);
	}
}