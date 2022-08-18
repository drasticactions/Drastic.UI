using System;
using Drastic.UI.Internals;
using static Drastic.UI.VisualElement;

namespace Drastic.UI
{
	public interface IVisualElementController : IElementController
	{
		void NativeSizeChanged();
		void InvalidateMeasure(InvalidationTrigger trigger);
		bool Batched { get; }
		bool DisableLayout { get; set; }
		EffectiveFlowDirection EffectiveFlowDirection { get; }
		bool IsInNativeLayout { get; set; }
		bool IsNativeStateConsistent { get; set; }
		bool IsPlatformEnabled { get; set; }
		NavigationProxy NavigationProxy { get; }
		event EventHandler<EventArg<VisualElement>> BatchCommitted;
		event EventHandler<FocusRequestArgs> FocusChangeRequested;
	}
}