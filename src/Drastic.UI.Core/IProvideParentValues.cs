using System.Collections.Generic;

namespace Drastic.UI.Xaml
{
	internal interface IProvideParentValues : IProvideValueTarget
	{
		IEnumerable<object> ParentObjects { get; }
	}
}