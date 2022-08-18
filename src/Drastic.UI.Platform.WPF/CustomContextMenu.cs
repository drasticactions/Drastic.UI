﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WApplication = System.Windows.Application;
using WMenuItem = System.Windows.Controls.MenuItem;

namespace Drastic.UI.Platform.WPF
{
	public sealed class CustomContextMenu : ContextMenu
	{
		protected override DependencyObject GetContainerForItemOverride()
		{
			var item = new WMenuItem();
			item.SetBinding(HeaderedItemsControl.HeaderProperty, new System.Windows.Data.Binding("Text"));

			item.Click += (sender, args) =>
			{
				IsOpen = false;

				var menuItem = item.DataContext as MenuItem;
				if (menuItem != null)
					((IMenuItemController)menuItem).Activate();
			};
			return item;
		}
	}
}
