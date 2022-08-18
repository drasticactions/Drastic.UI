﻿using System;

namespace Drastic.UI
{
	public interface IListViewController : IViewController
	{
		event EventHandler<ScrollToRequestedEventArgs> ScrollToRequested;

		ListViewCachingStrategy CachingStrategy { get; }
		Element FooterElement { get; }
		Element HeaderElement { get; }
		bool RefreshAllowed { get; }

		Cell CreateDefaultCell(object item);
		string GetDisplayTextFromGroup(object cell);
		void NotifyRowTapped(int index, int inGroupIndex, Cell cell);
		void NotifyRowTapped(int index, int inGroupIndex, Cell cell, bool isContextMenuRequested);
		void NotifyRowTapped(int index, Cell cell);
		void NotifyRowTapped(int index, Cell cell, bool isContextMenuRequested);
		void SendCellAppearing(Cell cell);
		void SendCellDisappearing(Cell cell);
		void SendRefreshing();
	}
}