using System.Collections.Generic;

namespace Drastic.UI.Internals
{
	public interface IGestureController
	{
		IList<GestureElement> GetChildElements(Point point);

		IList<IGestureRecognizer> CompositeGestureRecognizers { get; }
	}
}