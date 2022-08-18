using AppKit;
using Drastic.UI.Internals;

namespace Drastic.UI.Platform.MacOS
{
	internal class MacDeviceInfo : DeviceInfo
	{
		Size _pixelScreenSize;
		Size _scaledScreenSize;
		double _scalingFactor;

		public MacDeviceInfo()
		{
			UpdateScreenSize();
		}

		public override Size PixelScreenSize => _pixelScreenSize;
		public override Size ScaledScreenSize => _scaledScreenSize;

		public override double ScalingFactor => _scalingFactor;

		void UpdateScreenSize()
		{
			_scalingFactor = NSScreen.MainScreen.BackingScaleFactor;
			_scaledScreenSize = new Size(NSScreen.MainScreen.Frame.Width, NSScreen.MainScreen.Frame.Height);
			_pixelScreenSize = new Size(_scaledScreenSize.Width * _scalingFactor, _scaledScreenSize.Height * _scalingFactor);
		}
	}
}