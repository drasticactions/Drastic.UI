
#if __MOBILE__
namespace Drastic.UI.Platform.iOS
#else

namespace Drastic.UI.Platform.MacOS
#endif
{
	internal static class EffectUtilities
	{
		public static void RegisterEffectControlProvider(IEffectControlProvider self, IElementController oldElement,
			IElementController newElement)
		{
			IElementController controller = oldElement;
			if (controller != null && controller.EffectControlProvider == self)
				controller.EffectControlProvider = null;

			controller = newElement;
			if (controller != null)
				controller.EffectControlProvider = self;
		}
	}
}