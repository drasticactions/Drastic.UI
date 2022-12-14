﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

namespace Drastic.UI.Platform.iOS
{
	internal static class NSObjectExtensions
	{
		public static void QueueForLater(this NSObject nsObject, Action action) =>
			nsObject.BeginInvokeOnMainThread(action);
	}
}