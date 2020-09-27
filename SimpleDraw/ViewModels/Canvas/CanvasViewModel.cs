using System.Collections.ObjectModel;
using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    public class CanvasViewModel : ViewModelBase
    {
        private double _width;
        private double _height;
        private ObservableCollection<ShapeBaseViewModel> _shapes;
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

        public ObservableCollection<ShapeBaseViewModel> Shapes
        {
            get => _shapes;
            set => this.RaiseAndSetIfChanged(ref _shapes, value);
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
    }
}
