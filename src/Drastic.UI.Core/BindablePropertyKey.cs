using System;

namespace Drastic.UI
{
	public sealed class BindablePropertyKey
	{
		internal BindablePropertyKey(BindableProperty property)
		{
			if (property == null)
				throw new ArgumentNullException("property");

			BindableProperty = property;
		}

		public BindableProperty BindableProperty { get; private set; }
	}
}