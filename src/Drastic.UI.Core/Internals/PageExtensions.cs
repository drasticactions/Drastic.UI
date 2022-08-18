using System.ComponentModel;

namespace Drastic.UI.Internals
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class PageExtensions
	{
		public static Page AncestorToRoot(this Page page)
		{
			Element parent = page;
			while (!Application.IsApplicationOrNull(parent.RealParent))
				parent = parent.RealParent;

			return parent as Page;
		}
	}
}
