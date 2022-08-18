using System;

namespace Drastic.UI
{
	public interface IOpenGlViewController : IViewController
	{
		event EventHandler DisplayRequested;
	}
}