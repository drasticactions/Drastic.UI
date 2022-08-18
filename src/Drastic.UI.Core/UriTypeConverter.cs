﻿using System;

namespace Drastic.UI
{
	[Xaml.ProvideCompiled("Drastic.UI.Core.XamlC.UriTypeConverter")]
	[Xaml.TypeConversion(typeof(Uri))]
	public class UriTypeConverter : TypeConverter
	{
		public override object ConvertFromInvariantString(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return null;
			return new Uri(value, UriKind.RelativeOrAbsolute);
		}

		public override string ConvertToInvariantString(object value)
		{
			if (!(value is Uri uri))
				throw new NotSupportedException();
			return uri.ToString();
		}
	}
}
