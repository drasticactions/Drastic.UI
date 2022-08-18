using Drastic.UI.Platform;

namespace Drastic.UI.Shapes
{
	[RenderWith(typeof(_EllipseRenderer))]
	public sealed class Ellipse : Shape
	{
		public Ellipse()
		{
			Aspect = Stretch.Fill;
		}
	}
}