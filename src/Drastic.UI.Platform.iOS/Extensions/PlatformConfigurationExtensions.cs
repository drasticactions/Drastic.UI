#if __MOBILE__
using CurrentPlatform = Drastic.UI.PlatformConfiguration.iOS;
namespace Drastic.UI.Platform.iOS
#else
using CurrentPlatform = Drastic.UI.PlatformConfiguration.macOS;

namespace Drastic.UI.Platform.MacOS
#endif
{
	public static class PlatformConfigurationExtensions
	{
		public static IPlatformElementConfiguration<CurrentPlatform, T> OnThisPlatform<T>(this T element)
			where T : Element, IElementConfiguration<T>
		{
			return (element).On<CurrentPlatform>();
		}
	}
}