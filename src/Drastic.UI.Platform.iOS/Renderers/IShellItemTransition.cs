using System.Threading.Tasks;

namespace Drastic.UI.Platform.iOS
{
	public interface IShellItemTransition
	{
		Task Transition(IShellItemRenderer oldRenderer, IShellItemRenderer newRenderer);
	}
}