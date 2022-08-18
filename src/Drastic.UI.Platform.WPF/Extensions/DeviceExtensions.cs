using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drastic.UI.Internals;

namespace Drastic.UI.Platform.WPF
{
	internal static class DeviceExtensions
	{
		public static DeviceOrientation ToDeviceOrientation(this System.Windows.Window page)
		{
			return page.Height > page.Width ? DeviceOrientation.Portrait : DeviceOrientation.Landscape;
		}
	}
}
