using System;

namespace Drastic.UI
{
	public enum ShellNavigationSource
	{
		Unknown = 0,
		Push,
		Pop,
		PopToRoot,
		Insert,
		Remove,
		ShellItemChanged,
		ShellSectionChanged,
		ShellContentChanged,
	}
}