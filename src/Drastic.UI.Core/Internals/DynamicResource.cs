using System.ComponentModel;

namespace Drastic.UI.Internals
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class DynamicResource
	{
		public string Key { get; private set; }
		public DynamicResource(string key) => Key = key;
	}
}