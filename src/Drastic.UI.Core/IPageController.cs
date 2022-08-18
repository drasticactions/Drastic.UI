using System.Collections.ObjectModel;

namespace Drastic.UI
{
	public interface IPageController : IVisualElementController
	{
		Rectangle ContainerArea { get; set; }

		bool IgnoresContainerArea { get; set; }

		ObservableCollection<Element> InternalChildren { get; }

		void SendAppearing();

		void SendDisappearing();
	}
}