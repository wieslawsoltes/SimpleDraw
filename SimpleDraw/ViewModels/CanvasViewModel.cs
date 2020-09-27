using System.Collections.ObjectModel;
using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    public class CanvasViewModel : ViewModelBase
    {
        private double _width;
        private double _height;
        private ObservableCollection<ShapeBaseViewModel> _shapes;
        private ToolBase _tool;
        private ObservableCollection<ToolBase> _tools;

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

        public ObservableCollection<ShapeBaseViewModel> Shapes
        {
            get => _shapes;
            set => this.RaiseAndSetIfChanged(ref _shapes, value);
        }

        public ToolBase Tool
        {
            get => _tool;
            set => this.RaiseAndSetIfChanged(ref _tool, value);
        }

        public ObservableCollection<ToolBase> Tools
        {
            get => _tools;
            set => this.RaiseAndSetIfChanged(ref _tools, value);
        }
    }
}
