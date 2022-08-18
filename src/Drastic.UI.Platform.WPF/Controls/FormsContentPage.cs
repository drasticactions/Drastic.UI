﻿using System;
using System.Windows;
using WpfSize = System.Windows.Size;

namespace Drastic.UI.Platform.WPF.Controls
{
	public class FormsContentPage : FormsPage
	{
		public FormsContentPage()
		{
		}

		static FormsContentPage()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(FormsContentPage), new FrameworkPropertyMetadata(typeof(FormsContentPage)));
		}
	}
}
