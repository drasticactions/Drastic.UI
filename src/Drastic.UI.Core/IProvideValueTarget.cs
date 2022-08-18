namespace Drastic.UI.Xaml
{
	public interface IProvideValueTarget
	{
		object TargetObject { get; }
		object TargetProperty { get; }
	}
}