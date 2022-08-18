using AppKit;
using Drastic.UI.Internals;

namespace Drastic.UI.Platform.MacOS
{
	internal static class FlowDirectionExtensions
	{
        internal static FlowDirection ToFlowDirection(this NSUserInterfaceLayoutDirection direction)
        {
            switch (direction)
            {
                case NSUserInterfaceLayoutDirection.LeftToRight:
                    return FlowDirection.LeftToRight;
                case NSUserInterfaceLayoutDirection.RightToLeft:
                    return FlowDirection.RightToLeft;
                default:
                    return FlowDirection.MatchParent;
            }
        }

        internal static void UpdateFlowDirection(this NSView view, IVisualElementController controller)
		{
			if (controller == null || view == null)
				return;

			if (controller.EffectiveFlowDirection.IsRightToLeft())
				view.UserInterfaceLayoutDirection = NSUserInterfaceLayoutDirection.RightToLeft;
			else if (controller.EffectiveFlowDirection.IsLeftToRight())
				view.UserInterfaceLayoutDirection = NSUserInterfaceLayoutDirection.LeftToRight;
		}

		internal static void UpdateFlowDirection(this NSTextField control, IVisualElementController controller)
		{
			if (controller == null || control == null)
				return;

			if (controller.EffectiveFlowDirection.IsRightToLeft())
			{
				control.Alignment = NSTextAlignment.Right;
			}
			else if (controller.EffectiveFlowDirection.IsLeftToRight())
			{
				control.Alignment = NSTextAlignment.Left;
			}
		}
	}
}