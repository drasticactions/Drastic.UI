namespace Drastic.UI.Platform.iOS
{
    public interface ILoopItemsViewSource : IItemsViewSource
    {
        bool Loop { get; set; }

        int LoopCount { get; }
    }
}