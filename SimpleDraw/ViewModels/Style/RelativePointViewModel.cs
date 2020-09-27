using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    public class RelativePointViewModel : ViewModelBase
    {
        private PointViewModel _point;
        private RelativeUnit _unit;

        public PointViewModel Point
        {
            get => _point;
            set => this.RaiseAndSetIfChanged(ref _point, value);
        }

        public RelativeUnit Unit
        {
            get => _unit;
            set => this.RaiseAndSetIfChanged(ref _unit, value);
        }

        public RelativePointViewModel()
        {
        }

        public RelativePointViewModel(PointViewModel point, RelativeUnit unit)
        {
            _point = point;
            _unit = unit;
        }

        public RelativePointViewModel(double x, double y, RelativeUnit unit)
        {
            _point = new PointViewModel(x, y);
            _unit = unit;
        }
    }
}
