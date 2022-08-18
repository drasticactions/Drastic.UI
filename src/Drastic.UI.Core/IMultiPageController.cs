namespace Drastic.UI
{
	public interface IMultiPageController<T>
	{
		T GetPageByIndex(int index);
	}
}