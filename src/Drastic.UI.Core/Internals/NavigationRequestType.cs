using System.ComponentModel;

namespace Drastic.UI.Internals
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public enum NavigationRequestType
	{
		Unknown = 0,
		Push,
		Pop,
		PopToRoot,
		Insert,
		Remove,
	}
}