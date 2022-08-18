using System;
using System.Windows.Input;

namespace Drastic.UI
{
	public interface IButtonController : IViewController
	{
		void SendClicked();
		void SendPressed();
		void SendReleased();
	}
}