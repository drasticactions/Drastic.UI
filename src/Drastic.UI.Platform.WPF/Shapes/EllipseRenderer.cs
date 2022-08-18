using Drastic.UI.Shapes;

#if WINDOWS_UWP
using WEllipse = Windows.UI.Xaml.Shapes.Ellipse;

namespace Drastic.UI.Platform.UWP
#else
using WEllipse = System.Windows.Shapes.Ellipse;

namespace Drastic.UI.Platform.WPF
#endif
{
	public class EllipseRenderer : ShapeRenderer<Ellipse, WEllipse>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Ellipse> args)
		{
			if (Control == null && args.NewElement != null)
			{
				SetNativeControl(new WEllipse());
			}

			base.OnElementChanged(args);
		}
	}
}
