using System;
using System.ComponentModel;

namespace Drastic.UI.Internals
{
	[Obsolete]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IDataTemplate
	{
		Func<object> LoadTemplate { get; set; }
	}
}