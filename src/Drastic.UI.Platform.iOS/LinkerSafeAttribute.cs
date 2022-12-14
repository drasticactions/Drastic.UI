using System;
using System.ComponentModel;

namespace Drastic.UI.Internals
{
	[AttributeUsage(AttributeTargets.All)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	class LinkerSafeAttribute : Attribute
	{
		public LinkerSafeAttribute()
		{
		}
	}
}