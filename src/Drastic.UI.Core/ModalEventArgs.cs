using System;

namespace Drastic.UI
{
	public abstract class ModalEventArgs : EventArgs
	{
		protected ModalEventArgs(Page modal)
		{
			Modal = modal;
		}

		public Page Modal { get; private set; }
	}
}