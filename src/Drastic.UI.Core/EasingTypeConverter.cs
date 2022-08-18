﻿using System;
using System.Linq;
using System.Reflection;
using static Drastic.UI.Easing;

namespace Drastic.UI
{
	[Xaml.ProvideCompiled("Drastic.UI.Core.XamlC.EasingTypeConverter")]
	[Xaml.TypeConversion(typeof(Easing))]
	public class EasingTypeConverter : TypeConverter
	{
		public override object ConvertFromInvariantString(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return null;

			value = value?.Trim() ?? "";
			var parts = value.Split('.');
			if (parts.Length == 2 && parts[0] == nameof(Easing))
				value = parts[parts.Length - 1];

			if (value.Equals(nameof(Linear), StringComparison.OrdinalIgnoreCase))
				return Linear;
			if (value.Equals(nameof(SinIn), StringComparison.OrdinalIgnoreCase))
				return SinIn;
			if (value.Equals(nameof(SinOut), StringComparison.OrdinalIgnoreCase))
				return SinOut;
			if (value.Equals(nameof(SinInOut), StringComparison.OrdinalIgnoreCase))
				return SinInOut;
			if (value.Equals(nameof(CubicIn), StringComparison.OrdinalIgnoreCase))
				return CubicIn;
			if (value.Equals(nameof(CubicOut), StringComparison.OrdinalIgnoreCase))
				return CubicOut;
			if (value.Equals(nameof(CubicInOut), StringComparison.OrdinalIgnoreCase))
				return CubicInOut;
			if (value.Equals(nameof(BounceIn), StringComparison.OrdinalIgnoreCase))
				return BounceIn;
			if (value.Equals(nameof(BounceOut), StringComparison.OrdinalIgnoreCase))
				return BounceOut;
			if (value.Equals(nameof(SpringIn), StringComparison.OrdinalIgnoreCase))
				return SpringIn;
			if (value.Equals(nameof(SpringOut), StringComparison.OrdinalIgnoreCase))
				return SpringOut;

			var fallbackValue = typeof(Easing)
				.GetTypeInfo()
				.DeclaredFields
				.FirstOrDefault(f => f.Name.Equals(value, StringComparison.OrdinalIgnoreCase))
				?.GetValue(null);

			if (fallbackValue is Easing fallbackEasing)
				return fallbackEasing;

			throw new InvalidOperationException($"Cannot convert \"{value}\" into {typeof(Easing)}");
		}

		public override string ConvertToInvariantString(object value)
		{
			if (!(value is Easing easing))
				throw new NotSupportedException();

			if (easing == Linear)
				return nameof(Linear);
			if (easing == SinIn)
				return nameof(SinIn);
			if (easing == SinOut)
				return nameof(SinOut);
			if (easing == SinInOut)
				return nameof(SinInOut);
			if (easing == CubicIn)
				return nameof(CubicIn);
			if (easing == CubicOut)
				return nameof(CubicOut);
			if (easing == CubicInOut)
				return nameof(CubicInOut);
			if (easing == BounceIn)
				return nameof(BounceIn);
			if (easing == BounceOut)
				return nameof(BounceOut);
			if (easing == SpringIn)
				return nameof(SpringIn);
			if (easing == SpringOut)
				return nameof(SpringOut);
			throw new NotSupportedException();
		}
	}
}
