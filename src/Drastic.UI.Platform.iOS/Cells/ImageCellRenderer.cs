using System.ComponentModel;
using System.Threading.Tasks;
using Foundation;
using UIKit;

namespace Drastic.UI.Platform.iOS
{
	public class ImageCellRenderer : TextCellRenderer
	{
		[Preserve(Conditional = true)]
		public ImageCellRenderer()
		{
		}

		public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
		{
			var result = (CellTableViewCell)base.GetCell(item, reusableCell, tv);

			var imageCell = (ImageCell)item;

			WireUpForceUpdateSizeRequested(item, result, tv);

			SetImage(imageCell, result);

			return result;
		}

		protected override void HandleCellPropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			var imageCell = (ImageCell)sender;
			var tvc = (CellTableViewCell)GetRealCell(imageCell);

			base.HandleCellPropertyChanged(sender, args);

			if (args.PropertyName == ImageCell.ImageSourceProperty.PropertyName)
				SetImage(imageCell, tvc);
		}

		async void SetImage(ImageCell cell, CellTableViewCell target)
		{
			var source = cell.ImageSource;

			target.ImageView.Image = null;

			var uiimage = await source.GetNativeImageAsync().ConfigureAwait(false);
			if (uiimage != null)
			{
				NSRunLoop.Main.BeginInvokeOnMainThread(() =>
				{
					if (target.Cell != null)
					{
						target.ImageView.Image = uiimage;
						target.SetNeedsLayout();
					}
					else
					{
						uiimage?.Dispose();
					}
				});
			}
		}
	}
}
