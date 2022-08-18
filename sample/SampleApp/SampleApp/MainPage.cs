using System;
using Drastic.UI;
using Drastic.UI.Markup;

namespace SampleApp
{
	public class MainPage : ContentPage
	{
		public MainPage()
		{
			this.Content = new Label() { Text = "Hello World!", HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
		}
	}
}

