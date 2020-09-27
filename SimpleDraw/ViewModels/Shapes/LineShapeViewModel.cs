using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    public class LineShapeViewModel : ShapeBaseViewModel
    {
        private PointViewModel _start;
        private PointViewModel _end;
        private bool _isStroked;

        public PointViewModel Start
        {
            get => _start;
            set => this.RaiseAndSetIfChanged(ref _start, value);
        }

        public PointViewModel End
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
