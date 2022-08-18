using System;

namespace Drastic.UI
{
	public class ControlTemplate : ElementTemplate
	{
		public ControlTemplate()
		{
		}

		public ControlTemplate(Type type) : base(type)
		{
		}

		public ControlTemplate(Func<object> createTemplate) : base(createTemplate)
		{
		}
	}
}