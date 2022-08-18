using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Drastic.UI;
using Drastic.UI.Internals;

namespace Drastic.UI.DualScreen
{
	internal interface IDualScreenService
	{
		event EventHandler OnScreenChanged;
		bool IsSpanned { get; }
		bool IsLandscape { get; }
		Rectangle GetHinge();
		Size ScaledScreenSize { get; }
		Point? GetLocationOnScreen(VisualElement visualElement);
		object WatchForChangesOnLayout(VisualElement visualElement, Action action);
		void StopWatchingForChangesOnLayout(VisualElement visualElement, object handle);
		Task<int> GetHingeAngleAsync();
	}
}
