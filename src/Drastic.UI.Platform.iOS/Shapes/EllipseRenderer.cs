using CoreGraphics;
using Drastic.UI.Shapes;

#if __MOBILE__
namespace Drastic.UI.Platform.iOS
#else
namespace Drastic.UI.Platform.MacOS
#endif
{
    public class EllipseRenderer : ShapeRenderer<Ellipse, EllipseView>
    {
        [Internals.Preserve(Conditional = true)]
        public EllipseRenderer()
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Ellipse> args)
        {
            if (Control == null && args.NewElement != null)
            {
                SetNativeControl(new EllipseView());
            }

            base.OnElementChanged(args);
        }
    }

    public class EllipseView : ShapeView
    {
        public EllipseView()
        {
            UpdateShape();
        }

        void UpdateShape()
        {
			var path = new CGPath();
            path.AddEllipseInRect(new CGRect(0, 0, 1, 1));
            ShapeLayer.UpdateShape(path);
        }
    }
}