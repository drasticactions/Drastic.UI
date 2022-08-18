﻿using System.ComponentModel;
using FormsRectangle = Drastic.UI.Shapes.Rectangle;

#if WINDOWS_UWP
using WRectangle = Windows.UI.Xaml.Shapes.Rectangle;

namespace Drastic.UI.Platform.UWP
#else
using WRectangle = System.Windows.Shapes.Rectangle;

namespace Drastic.UI.Platform.WPF
#endif
{
	public class RectangleRenderer : ShapeRenderer<FormsRectangle, WRectangle>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<FormsRectangle> args)
		{
			if (Control == null && args.NewElement != null)
			{
				SetNativeControl(new WRectangle());
			}

			base.OnElementChanged(args);

			if (args.NewElement != null)
			{
				UpdateRadiusX();
				UpdateRadiusY();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			base.OnElementPropertyChanged(sender, args);

			if (args.PropertyName == FormsRectangle.RadiusXProperty.PropertyName)
				UpdateRadiusX();
			else if (args.PropertyName == FormsRectangle.RadiusYProperty.PropertyName)
				UpdateRadiusY();
		}

		void UpdateRadiusX()
		{
			Control.RadiusX = Element.RadiusX;
		}

		void UpdateRadiusY()
		{
			Control.RadiusY = Element.RadiusY;
		}
	}
}