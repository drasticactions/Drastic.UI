#if __MOBILE__
using NativeImageView = UIKit.UIImageView;
using NativeImage = UIKit.UIImage;
namespace Drastic.UI.Platform.iOS
#else
using NativeImageView = AppKit.NSImageView;
using NativeImage = AppKit.NSImage;
namespace Drastic.UI.Platform.MacOS
#endif
{
	public interface IImageVisualElementRenderer : IVisualNativeElementRenderer
	{
		void SetImage(NativeImage image);
		bool IsDisposed { get; }
		NativeImageView GetImage();
	}
}