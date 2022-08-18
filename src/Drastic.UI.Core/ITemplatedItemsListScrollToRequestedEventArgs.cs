using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drastic.UI
{
	public interface ITemplatedItemsListScrollToRequestedEventArgs
	{
		object Group { get; }
		object Item { get; }
	}
}
