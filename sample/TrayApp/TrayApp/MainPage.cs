using System;
using Drastic.UI;
using Drastic.UI.Markup;
using System.Reflection.Metadata;
using System.Reflection;

namespace TrayApp
{
    public class MainPage : ContentPage
    {
        public MainPage()
        {
            Content = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Children = {
                    new Label
                        {
                            Text = "Hello Drastic.UI!",
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.Center
                        },
                    new Image
                    {
                        Source = ImageSource.FromResource("TrayApp.Resources.icon.png", typeof(MainPage).GetTypeInfo().Assembly)
                    }
                },
            };
        }
    }
}

