using System.ComponentModel;

namespace Drastic.UI.Internals
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class ExpressionSearch
	{
		public static IExpressionSearch Default { get; set; }
	}
}