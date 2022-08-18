using System;

namespace Drastic.UI
{
	public interface IAppLinks
	{
		void DeregisterLink(IAppLinkEntry appLink);
		void DeregisterLink(Uri appLinkUri);
		void RegisterLink(IAppLinkEntry appLink);
	}
}