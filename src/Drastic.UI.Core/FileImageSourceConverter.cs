using System;

namespace Drastic.UI
{
	[Xaml.TypeConversion(typeof(FileImageSource))]
	public sealed class FileImageSourceConverter : TypeConverter
	{
		public override object ConvertFromInvariantString(string value)
		{
			if (value != null)
				return (FileImageSource)ImageSource.FromFile(value);

			throw new InvalidOperationException(string.Format("Cannot convert \"{0}\" into {1}", value, typeof(FileImageSource)));
		}

		public override string ConvertToInvariantString(object value)
		{
			if (!(value is FileImageSource fis))
				throw new NotSupportedException();
			return fis.File;
		}
	}
}