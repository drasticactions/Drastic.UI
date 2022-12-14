using System;

namespace Drastic.UI.Platform.iOS
{
	public class IOSAppIndexingProvider : IAppIndexingProvider
	{
		public IAppLinks AppLinks => new IOSAppLinks();
	}
}