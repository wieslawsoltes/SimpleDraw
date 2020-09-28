using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    [DataContract(IsReference = true)]
    public class CanvasViewModel : ViewModelBase
    {
        private double _width;
        private double _height;
        private ObservableCollection<ViewModelBase> _items;
        private ObservableCollection<ViewModelBase> _selected;
        private ObservableCollection<ViewModelBase> _decorators;
        private ToolBaseViewModel _tool;
        private ObservableCollection<ToolBaseViewModel> _tools;

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public double Width
        {
            get => _width;
            set => this.RaiseAndSetIfChanged(ref _width, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public double Height
        {
            get => _height;
            set => this.RaiseAndSetIfChanged(ref _height, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public ObservableCollection<ViewModelBase> Items
        {
            get => _items;
            set => this.RaiseAndSetIfChanged(ref _items, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public ObservableCollection<ViewModelBase> Selected
        {
            get => _selected;
            set => this.RaiseAndSetIfChanged(ref _selected, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public ObservableCollection<ViewModelBase> Decorators
        {
            get => _decorators;
            set => this.RaiseAndSetIfChanged(ref _decorators, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public ToolBaseViewModel Tool
        {
            get => _tool;
            set => this.RaiseAndSetIfChanged(ref _tool, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
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

            var decorators = new ObservableCollection<ViewModelBase>();

            foreach (var item in _decorators)
            {
                decorators.Add(item.Clone(shared));
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
                Decorators = decorators,
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
