using System;

namespace Drastic.UI
{
	public interface IDispatcher
	{
		void BeginInvokeOnMainThread(Action action);
		bool IsInvokeRequired { get; }
	}
}
