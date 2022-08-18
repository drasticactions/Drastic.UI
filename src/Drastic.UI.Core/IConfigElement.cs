
namespace Drastic.UI
{
	public interface IConfigElement<out T> where T : Element
	{
		T Element { get; }
	}
}
