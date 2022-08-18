using System.ComponentModel;

namespace Drastic.UI.Internals
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IDataTemplateController
	{
		int Id { get; }
		string IdString { get; }
	}
}
