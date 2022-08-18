using System.Collections.Generic;

namespace Drastic.UI
{
	public interface IGestureRecognizers
	{
		IList<IGestureRecognizer> GestureRecognizers { get; }
	}
}