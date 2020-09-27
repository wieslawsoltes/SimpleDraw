using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    public abstract class ShapeBaseViewModel : ViewModelBase
    {
        private BrushViewModel _brush;
        private PenViewModel _pen;

        public BrushViewModel Brush
        {
            get => _brush;
            set => this.RaiseAndSetIfChanged(ref _brush, value);
        }

        public PenViewModel Pen
        {
            get => _pen;
            set => this.RaiseAndSetIfChanged(ref _pen, value);
        }
    }
}
