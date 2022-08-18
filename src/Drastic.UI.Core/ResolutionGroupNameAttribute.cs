using System;

namespace Drastic.UI
{
	[AttributeUsage(AttributeTargets.Assembly)]
	public sealed class ResolutionGroupNameAttribute : Attribute
	{
		public ResolutionGroupNameAttribute(string name)
		{
			ShortName = name;
		}

		internal string ShortName { get; private set; }
	}
}