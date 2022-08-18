using System.ComponentModel;
using Drastic.UI.Platform.WPF.Controls;

namespace Drastic.UI.Platform.WPF
{
	public class CarouselPageRenderer : VisualMultiPageRenderer<CarouselPage, ContentPage, FormsCarouselPage>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<CarouselPage> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null) // construct and SetNativeControl and suscribe control event
				{
					SetNativeControl(new FormsCarouselPage() { ContentLoader = new FormsContentLoader() });
				}
			}

			base.OnElementChanged(e);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
		}
	}
}
