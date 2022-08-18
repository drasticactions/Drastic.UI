using System.ComponentModel;

namespace Drastic.UI.Internals
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface ISystemResourcesProvider
	{
		IResourceDictionary GetSystemResources();
	}
}