namespace Drastic.UI
{
	internal interface IVisualController
	{
		IVisual EffectiveVisual { get; set; }
		IVisual Visual { get; }
	}
}