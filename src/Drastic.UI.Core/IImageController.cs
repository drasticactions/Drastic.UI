using System;

namespace Drastic.UI
{
	public interface IImageController : IViewController
	{
		void SetIsLoading(bool isLoading);
		bool GetLoadAsAnimation();
	}
}