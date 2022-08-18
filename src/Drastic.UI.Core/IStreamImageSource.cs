using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Drastic.UI
{
	public interface IStreamImageSource
	{
		Task<Stream> GetStreamAsync(CancellationToken userToken = default(CancellationToken));
	}
}