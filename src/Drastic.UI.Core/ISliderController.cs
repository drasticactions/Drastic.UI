using System.ComponentModel;

namespace Drastic.UI
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface ISliderController
	{
		void SendDragStarted();
		void SendDragCompleted();
	}
}
