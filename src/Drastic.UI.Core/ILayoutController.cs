using System.Collections.Generic;

namespace Drastic.UI
{
	public interface ILayoutController
	{
		IReadOnlyList<Element> Children { get; }
	}
}