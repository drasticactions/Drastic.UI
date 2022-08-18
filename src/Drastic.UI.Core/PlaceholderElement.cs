﻿namespace Drastic.UI
{
	static class PlaceholderElement
	{
		public static readonly BindableProperty PlaceholderProperty =
			BindableProperty.Create(nameof(IPlaceholderElement.Placeholder), typeof(string), typeof(IPlaceholderElement), default(string));

		public static readonly BindableProperty PlaceholderColorProperty =
			BindableProperty.Create(nameof(IPlaceholderElement.PlaceholderColor), typeof(Color), typeof(IPlaceholderElement), default(Color));
	}
}