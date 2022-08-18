using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Drastic.UI.Internals
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IDeserializer
	{
		Task<IDictionary<string, object>> DeserializePropertiesAsync();
		Task SerializePropertiesAsync(IDictionary<string, object> properties);
	}
}