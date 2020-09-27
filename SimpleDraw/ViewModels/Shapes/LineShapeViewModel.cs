using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    public class LineShapeViewModel : ShapeBaseViewModel
    {
        private PointShapeViewModel _start;
        private PointShapeViewModel _end;
        private bool _isStroked;

        public PointShapeViewModel Start
        {
            get => _start;
            set => this.RaiseAndSetIfChanged(ref _start, value);
        }

        public PointShapeViewModel End
        {
            get => _end;
            set => this.RaiseAndSetIfChanged(ref _end, value);
        }

        public bool IsStroked
        {
            get => _isStroked;
            set => this.RaiseAndSetIfChanged(ref _isStroked, value);
        }
    }
}
