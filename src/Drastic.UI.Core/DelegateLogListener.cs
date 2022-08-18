using System;
using System.ComponentModel;
using Drastic.UI.Internals;

namespace Drastic.UI.Internals
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class DelegateLogListener : LogListener
	{
		readonly Action<string, string> _log;

		public DelegateLogListener(Action<string, string> log)
		{
			if (log == null)
				throw new ArgumentNullException("log");

			_log = log;
		}

		public override void Warning(string category, string message)
		{
			_log(category, message);
		}
	}
}