using System;
using System.Collections;

namespace Drastic.UI
{
	public delegate void CollectionSynchronizationCallback(IEnumerable collection, object context, Action accessMethod, bool writeAccess);
}