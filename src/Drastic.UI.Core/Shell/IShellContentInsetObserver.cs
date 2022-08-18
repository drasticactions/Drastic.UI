namespace Drastic.UI
{
	public interface IShellContentInsetObserver
	{
		void OnInsetChanged(Thickness inset, double tabThickness);
	}
}