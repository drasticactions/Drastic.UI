using Foundation;

namespace Drastic.UI.Platform.MacOS
{
	internal class NSPageContainer : NSObject
	{
		public NSPageContainer(Page element, int index)
		{
			Page = element;
			Index = index;
		}

		public Page Page { get; }

		public int Index { get; set; }
	}
}