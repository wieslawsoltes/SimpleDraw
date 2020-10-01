using System.Collections.ObjectModel;

namespace SimpleDraw.ViewModels
{
    public interface IItemsCanvas
    {
        ObservableCollection<ViewModelBase> Items { get; set; }
        ObservableCollection<ViewModelBase> Decorators { get; set; }
        void Invalidate();
    }
}
