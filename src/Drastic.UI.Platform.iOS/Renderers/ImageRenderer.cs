using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Foundation;
using UIKit;
using Drastic.UI.Internals;
using RectangleF = CoreGraphics.CGRect;
using PreserveAttribute = Foundation.PreserveAttribute;

namespace Drastic.UI.Platform.iOS
{
	public static class ImageExtensions
	{
		public static UIViewContentMode ToUIViewContentMode(this Aspect aspect)
		{
			switch (aspect)
			{
				case Aspect.AspectFill:
					return UIViewContentMode.ScaleAspectFill;
				case Aspect.Fill:
					return UIViewContentMode.ScaleToFill;
				case Aspect.AspectFit:
				default:
					return UIViewContentMode.ScaleAspectFit;
			}
		}
	}

	public class ImageRenderer : ViewRenderer<Image, FormsUIImageView>, IImageVisualElementRenderer
	{
		bool _isDisposed;

		[Preserve(Conditional = true)]
		public ImageRenderer() : base()
		{
			ImageElementManager.Init(this);
		}

		protected override void Dispose(bool disposing)
		{
			if (_isDisposed)
				return;

			if (disposing)
			{
				UIImage oldUIImage;
				if (Control != null && (oldUIImage = Control.Image) != null)
				{
					ImageElementManager.Dispose(this);
					oldUIImage.Dispose();
				}
			}

			_isDisposed = true;

			base.Dispose(disposing);
		}

		protected override async void OnElementChanged(ElementChangedEventArgs<Image> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null)
				{
					var imageView = new FormsUIImageView();
					imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
					imageView.ClipsToBounds = true;
					SetNativeControl(imageView);
				}

				await TrySetImage(e.OldElement as Image);
				UpdateBackground();
			}

			base.OnElementChanged(e);
		}

		protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Image.SourceProperty.PropertyName)
				await TrySetImage().ConfigureAwait(false);
			else if (e.PropertyName == VisualElement.BackgroundProperty.PropertyName)
				UpdateBackground();
		}

		protected virtual async Task TrySetImage(Image previous = null)
		{
			// By default we'll just catch and log any exceptions thrown by SetImage so they don't bring down
			// the application; a custom renderer can override this method and handle exceptions from
			// SetImage differently if it wants to

			try
			{
				await SetImage(previous).ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				Log.Warning(nameof(ImageRenderer), "Error loading image: {0}", ex);
			}
			finally
			{
				((IImageController)Element)?.SetIsLoading(false);
			}
		}

		protected async Task SetImage(Image oldElement = null)
		{
			await ImageElementManager.SetImage(this, Element, oldElement).ConfigureAwait(false);
		}

		void IImageVisualElementRenderer.SetImage(UIImage image) => Control.Image = image;

		bool IImageVisualElementRenderer.IsDisposed => _isDisposed;

		UIImageView IImageVisualElementRenderer.GetImage() => Control;

		void UpdateBackground()
		{
			var parent = Control?.Superview;

			if (parent == null)
				return;

			parent.UpdateBackground(Element.Background);
		}
	}

	public interface IImageSourceHandler : IRegisterable
	{
		Task<UIImage> LoadImageAsync(ImageSource imagesource, CancellationToken cancelationToken = default(CancellationToken), float scale = 1);
	}

	public interface IAnimationSourceHandler : IRegisterable
	{
		Task<FormsCAKeyFrameAnimation> LoadImageAnimationAsync(ImageSource imagesource, CancellationToken cancelationToken = default(CancellationToken), float scale = 1);
	}

	public sealed class FileImageSourceHandler : IImageSourceHandler, IAnimationSourceHandler
	{
		[Preserve(Conditional = true)]
		public FileImageSourceHandler()
		{
		}

		public Task<UIImage> LoadImageAsync(ImageSource imagesource, CancellationToken cancelationToken = default(CancellationToken), float scale = 1f)
		{
			UIImage image = null;
			var filesource = imagesource as FileImageSource;
			var file = filesource?.File;
			if (!string.IsNullOrEmpty(file))
				image = File.Exists(file) ? new UIImage(file) : UIImage.FromBundle(file);

			if (image == null)
			{
				Log.Warning(nameof(FileImageSourceHandler), "Could not find image: {0}", imagesource);
			}

			return Task.FromResult(image);
		}

		public Task<FormsCAKeyFrameAnimation> LoadImageAnimationAsync(ImageSource imagesource, CancellationToken cancelationToken = default(CancellationToken), float scale = 1)
		{
			FormsCAKeyFrameAnimation animation = ImageAnimationHelper.CreateAnimationFromFileImageSource(imagesource as FileImageSource);
			if (animation == null)
			{
				Log.Warning(nameof(FileImageSourceHandler), "Could not find image: {0}", imagesource);
			}

			return Task.FromResult(animation);
		}
	}

	public sealed class StreamImagesourceHandler : IImageSourceHandler, IAnimationSourceHandler
	{
		[Preserve(Conditional = true)]
		public StreamImagesourceHandler()
		{
		}

		public async Task<UIImage> LoadImageAsync(ImageSource imagesource, CancellationToken cancelationToken = default(CancellationToken), float scale = 1f)
		{
			UIImage image = null;
			var streamsource = imagesource as StreamImageSource;
			if (streamsource?.Stream != null)
			{
				using (var streamImage = await ((IStreamImageSource)streamsource).GetStreamAsync(cancelationToken).ConfigureAwait(false))
				{
					if (streamImage != null)
						image = UIImage.LoadFromData(NSData.FromStream(streamImage), scale);
				}
			}

			if (image == null)
			{
				Log.Warning(nameof(StreamImagesourceHandler), "Could not load image: {0}", streamsource);
			}

			return image;
		}

		public async Task<FormsCAKeyFrameAnimation> LoadImageAnimationAsync(ImageSource imagesource, CancellationToken cancelationToken = default(CancellationToken), float scale = 1)
		{
			FormsCAKeyFrameAnimation animation = await ImageAnimationHelper.CreateAnimationFromStreamImageSourceAsync(imagesource as StreamImageSource, cancelationToken).ConfigureAwait(false);
			if (animation == null)
			{
				Log.Warning(nameof(StreamImagesourceHandler), "Could not find image: {0}", imagesource);
			}

			return animation;
		}
	}

	public sealed class ImageLoaderSourceHandler : IImageSourceHandler, IAnimationSourceHandler
	{
		[Preserve(Conditional = true)]
		public ImageLoaderSourceHandler()
		{
		}

		public async Task<UIImage> LoadImageAsync(ImageSource imagesource, CancellationToken cancelationToken = default(CancellationToken), float scale = 1f)
		{
			UIImage image = null;
			var imageLoader = imagesource as UriImageSource;
			if (imageLoader?.Uri != null)
			{
				using (var streamImage = await imageLoader.GetStreamAsync(cancelationToken).ConfigureAwait(false))
				{
					if (streamImage != null)
						image = UIImage.LoadFromData(NSData.FromStream(streamImage), scale);
				}
			}

			if (image == null)
			{
				Log.Warning(nameof(ImageLoaderSourceHandler), "Could not load image: {0}", imageLoader);
			}

			return image;
		}

		public async Task<FormsCAKeyFrameAnimation> LoadImageAnimationAsync(ImageSource imagesource, CancellationToken cancelationToken = default(CancellationToken), float scale = 1)
		{
			FormsCAKeyFrameAnimation animation = await ImageAnimationHelper.CreateAnimationFromUriImageSourceAsync(imagesource as UriImageSource, cancelationToken).ConfigureAwait(false);
			if (animation == null)
			{
				Log.Warning(nameof(FileImageSourceHandler), "Could not find image: {0}", imagesource);
			}

			return animation;
		}
	}

	public sealed class FontImageSourceHandler : IImageSourceHandler
	{
		readonly Color _defaultColor = ColorExtensions.LabelColor.ToColor();

		[Preserve(Conditional = true)]
		public FontImageSourceHandler()
		{
		}

		public Task<UIImage> LoadImageAsync(
			ImageSource imagesource,
			CancellationToken cancelationToken = default(CancellationToken),
			float scale = 1f)
		{
			UIImage image = null;
			var fontsource = imagesource as FontImageSource;
			if (fontsource != null)
			{
				//This will allow lookup from the Embedded Fonts
				var cleansedname = FontExtensions.CleanseFontName(fontsource.FontFamily);
				var font = UIFont.FromName(cleansedname ?? string.Empty, (float)fontsource.Size) ??
					UIFont.SystemFontOfSize((float)fontsource.Size);
				var iconcolor = fontsource.Color.IsDefault ? _defaultColor : fontsource.Color;
				var attString = new NSAttributedString(fontsource.Glyph, font: font, foregroundColor: iconcolor.ToUIColor());
				var imagesize = ((NSString)fontsource.Glyph).GetSizeUsingAttributes(attString.GetUIKitAttributes(0, out _));
				
				UIGraphics.BeginImageContextWithOptions(imagesize, false, 0f);
				var ctx = new NSStringDrawingContext();
				var boundingRect = attString.GetBoundingRect(imagesize, (NSStringDrawingOptions)0, ctx);
				attString.DrawString(new RectangleF(
					imagesize.Width / 2 - boundingRect.Size.Width / 2,
					imagesize.Height / 2 - boundingRect.Size.Height / 2,
					imagesize.Width,
					imagesize.Height));
				image = UIGraphics.GetImageFromCurrentImageContext();
				UIGraphics.EndImageContext();

				if (image != null && iconcolor != _defaultColor)
					image = image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
			}
			return Task.FromResult(image);

		}
	}
}
