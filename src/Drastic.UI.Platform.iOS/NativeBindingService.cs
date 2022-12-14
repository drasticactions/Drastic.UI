using System;
using UIKit;
using Drastic.UI.Internals;
using Drastic.UI.Xaml.Internals;

[assembly: Drastic.UI.Dependency(typeof(Drastic.UI.Platform.iOS.NativeBindingService))]

namespace Drastic.UI.Platform.iOS
{
	[Preserve(AllMembers = true)]
	class NativeBindingService : INativeBindingService
	{
		public bool TrySetBinding(object target, string propertyName, BindingBase binding)
		{
			var view = target as UIView;
			if (view == null)
				return false;
			if (target.GetType().GetProperty(propertyName)?.GetMethod == null)
				return false;
			view.SetBinding(propertyName, binding);
			return true;
		}

		public bool TrySetBinding(object target, BindableProperty property, BindingBase binding)
		{
			var view = target as UIView;
			if (view == null)
				return false;
			view.SetBinding(property, binding);
			return true;
		}

		public bool TrySetValue(object target, BindableProperty property, object value)
		{
			var view = target as UIView;
			if (view == null)
				return false;
			view.SetValue(property, value);
			return true;
		}
	}
}