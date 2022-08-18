using System;

namespace Drastic.UI
{
	[Flags]
	public enum ListViewCachingStrategy
	{
		RetainElement = 0,
		RecycleElement = 1 << 0,
		RecycleElementAndDataTemplate = RecycleElement | 1 << 1,
	}
}