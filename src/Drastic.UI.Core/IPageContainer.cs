namespace Drastic.UI
{
	public interface IPageContainer<out T> where T : Page
	{
		T CurrentPage { get; }
	}
}