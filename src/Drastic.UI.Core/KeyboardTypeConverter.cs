using System;
using System.Linq;
using System.Reflection;
using Drastic.UI.Internals;

namespace Drastic.UI
{
	[Xaml.TypeConversion(typeof(Keyboard))]
	public class KeyboardTypeConverter : TypeConverter
	{
		public override object ConvertFromInvariantString(string value)
		{
			if (value != null)
			{
				string[] parts = value.Split('.');
				if (parts.Length == 1 || (parts.Length == 2 && parts[0] == "Keyboard"))
				{
					string keyboard = parts[parts.Length - 1];
					FieldInfo field = typeof(Keyboard).GetFields().FirstOrDefault(fi => fi.IsStatic && fi.Name == keyboard);
					if (field != null)
						return (Keyboard)field.GetValue(null);
					PropertyInfo property = typeof(Keyboard).GetProperties().FirstOrDefault(pi => pi.Name == keyboard && pi.CanRead && pi.GetMethod.IsStatic);
					if (property != null)
						return (Keyboard)property.GetValue(null, null);
				}
			}

			throw new InvalidOperationException(string.Format("Cannot convert \"{0}\" into {1}", value, typeof(Keyboard)));
		}

		public override string ConvertToInvariantString(object value)
		{
			if (!(value is Keyboard keyboard))
				throw new NotSupportedException();
			if (keyboard == Keyboard.Plain)
				return nameof(Keyboard.Plain);
			if (keyboard == Keyboard.Chat)
				return nameof(Keyboard.Chat);
			if (keyboard == Keyboard.Default)
				return nameof(Keyboard.Default);
			if (keyboard == Keyboard.Email)
				return nameof(Keyboard.Email);
			if (keyboard == Keyboard.Numeric)
				return nameof(Keyboard.Numeric);
			if (keyboard == Keyboard.Telephone)
				return nameof(Keyboard.Telephone);
			if (keyboard == Keyboard.Text)
				return nameof(Keyboard.Text);
			if (keyboard == Keyboard.Url)
				return nameof(Keyboard.Url);
			throw new NotSupportedException();
		}
	}
}