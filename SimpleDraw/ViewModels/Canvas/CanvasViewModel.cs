using System.Collections.Generic;
using System.Collections.ObjectModel;
using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    public class CanvasViewModel : ViewModelBase
    {
        private double _width;
        private double _height;
        private ObservableCollection<ViewModelBase> _items;
        private ObservableCollection<ViewModelBase> _selected;
        private ToolBaseViewModel _tool;
        private ObservableCollection<ToolBaseViewModel> _tools;

        public double Width
        {
            get => _width;
            set => this.RaiseAndSetIfChanged(ref _width, value);
        }

        public double Height
        {
            get => _height;
            set => this.RaiseAndSetIfChanged(ref _height, value);
        }

        public ObservableCollection<ViewModelBase> Items
        {
            get => _items;
            set => this.RaiseAndSetIfChanged(ref _items, value);
        }

        public ObservableCollection<ViewModelBase> Selected
        {
            get => _selected;
            set => this.RaiseAndSetIfChanged(ref _selected, value);
        }

        public ToolBaseViewModel Tool
        {
            get => _tool;
            set => this.RaiseAndSetIfChanged(ref _tool, value);
        }

        public ObservableCollection<ToolBaseViewModel> Tools
        {
            get => _tools;
            set => this.RaiseAndSetIfChanged(ref _tools, value);
        }

        public CanvasViewModel Copy(Dictionary<ViewModelBase, ViewModelBase> shared)
        {
            if (shared.TryGetValue(this, out var value))
            {
                return value as CanvasViewModel;
            }

            var items = new ObservableCollection<ViewModelBase>();
            
            foreach (var item in _items)
            {
                items.Add(item.Clone(shared));
            }

            var selected = new ObservableCollection<ViewModelBase>();

            foreach (var item in _selected)
            {
                selected.Add(item.Clone(shared));
            }

            var tools = new ObservableCollection<ToolBaseViewModel>();

            foreach (var item in _tools)
            {
                tools.Add(item.Copy(shared));
            }

            var copy = new CanvasViewModel()
            {
                Width = _width,
                Height = _height,
                Items = items,
                Selected = selected,
                Tool = _tool?.Copy(shared),
                Tools = tools
            };

            shared[this] = copy;
            return copy;
        }

        public override ViewModelBase Clone(Dictionary<ViewModelBase, ViewModelBase> shared)
        {
            return Copy(shared);
        }
    }
}
