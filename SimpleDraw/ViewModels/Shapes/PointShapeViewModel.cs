using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    public class PointShapeViewModel : ShapeBaseViewModel
    {
        private double _x;
        private double _y;

        public double X
        {
            get => _x;
            set => this.RaiseAndSetIfChanged(ref _x, value);
        }

        public double Y
        {
            get => _y;
            set => this.RaiseAndSetIfChanged(ref _y, value);
        }

        public PointShapeViewModel()
        {
        }

        public PointShapeViewModel(double x, double y)
        {
            _x = x;
            _y = y;
        }
    }
}
