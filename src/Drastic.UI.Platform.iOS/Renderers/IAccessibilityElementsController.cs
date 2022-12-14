using Foundation;
using System;
using System.Collections.Generic;

namespace Drastic.UI.Platform.iOS
{
	internal interface IAccessibilityElementsController
	{
		List<NSObject> GetAccessibilityElements();
	}
}