namespace Drastic.UI
{
	internal interface IAttachedObject
	{
		void AttachTo(BindableObject bindable);
		void DetachFrom(BindableObject bindable);
	}
}