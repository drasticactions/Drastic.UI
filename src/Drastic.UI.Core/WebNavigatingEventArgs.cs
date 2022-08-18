namespace Drastic.UI
{
	public class WebNavigatingEventArgs : WebNavigationEventArgs
	{
		public WebNavigatingEventArgs(WebNavigationEvent navigationEvent, WebViewSource source, string url) : base(navigationEvent, source, url)
		{
		}

		public bool Cancel { get; set; }
	}
}