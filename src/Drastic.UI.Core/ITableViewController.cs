using System;

namespace Drastic.UI
{
	public interface ITableViewController
	{
		event EventHandler ModelChanged;

		ITableModel Model { get; }
	}
}
