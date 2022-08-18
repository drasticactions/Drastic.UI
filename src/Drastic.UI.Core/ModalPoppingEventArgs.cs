namespace Drastic.UI
{
	public class ModalPoppingEventArgs : ModalEventArgs
	{
		public ModalPoppingEventArgs(Page modal) : base(modal)
		{
		}

		public bool Cancel { get; set; }
	}
}