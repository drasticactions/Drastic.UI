using System;
using System.Windows.Input;

namespace Drastic.UI
{
	public interface ISwipeItem
	{
		bool IsVisible { get; set; }
		ICommand Command { get; set; }
		object CommandParameter { get; set; }

		event EventHandler<EventArgs> Invoked;
		void OnInvoked();
	}
}