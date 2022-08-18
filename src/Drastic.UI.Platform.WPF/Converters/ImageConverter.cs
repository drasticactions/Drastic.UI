using System;
using System.Globalization;
using Drastic.UI.Internals;
using WImageSource = System.Windows.Media.ImageSource;

namespace Drastic.UI.Platform.WPF
{
	public sealed class ImageConverter : System.Windows.Data.IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var task = (value as ImageSource)?.ToWindowsImageSourceAsync();
			return task?.AsAsyncValue() ?? AsyncValue<WImageSource>.Null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
