using System.ComponentModel;

namespace Drastic.UI.Internals
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IDynamicResourceHandler
	{
		void SetDynamicResource(BindableProperty property, string key);
	}
}