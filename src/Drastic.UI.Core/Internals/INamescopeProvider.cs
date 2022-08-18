using System;
using System.ComponentModel;
using Drastic.UI.Internals;

namespace Drastic.UI.Xaml.Internals
{
	[Obsolete]
	[EditorBrowsable(EditorBrowsableState.Never)]
	interface INameScopeProvider
	{
		INameScope NameScope { get; }
	}
}