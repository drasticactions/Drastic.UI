namespace Drastic.UI
{
	interface IResourcesProvider
	{
		bool IsResourcesCreated { get; }
		ResourceDictionary Resources { get; set; }
	}
}