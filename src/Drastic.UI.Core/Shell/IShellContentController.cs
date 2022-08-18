using System;

namespace Drastic.UI
{
	public interface IShellContentController : IElementController
	{
		Page GetOrCreateContent();

		void RecyclePage(Page page);

		Page Page { get; }
		event EventHandler IsPageVisibleChanged;
	}
}