using System.ComponentModel;

namespace Drastic.UI.Internals
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	interface ILineHeightElement
	{
		double LineHeight { get; }

		void OnLineHeightChanged(double oldValue, double newValue);
	}
}