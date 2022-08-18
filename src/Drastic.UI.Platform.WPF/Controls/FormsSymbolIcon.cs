using System.Windows;
using Drastic.UI.Platform.WPF.Enums;

namespace Drastic.UI.Platform.WPF.Controls
{
	public class FormsSymbolIcon : FormsElementIcon
	{
		public static readonly DependencyProperty SymbolProperty = DependencyProperty.Register("Symbol", typeof(Symbol), typeof(FormsSymbolIcon));

		public Symbol Symbol
		{
			get { return (Symbol)GetValue(SymbolProperty); }
			set { SetValue(SymbolProperty, value); }
		}

		public FormsSymbolIcon()
		{
			this.DefaultStyleKey = typeof(FormsSymbolIcon);
		}
	}
}
