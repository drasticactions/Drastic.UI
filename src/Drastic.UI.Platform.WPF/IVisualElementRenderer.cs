using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Drastic.UI.Platform.WPF
{
	public interface IVisualElementRenderer : IRegisterable, IDisposable
	{
		FrameworkElement GetNativeElement();

		VisualElement Element { get; }

		event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint);

		void SetElement(VisualElement element);

	}
}
