using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    public class RectangleShapeViewModel : ShapeBaseViewModel
    {
        private PointShapeViewModel _topLeft;
        private PointShapeViewModel _bottomRight;
        private bool _isStroked;
        private bool _isFilled;
        private double _radiusX;
        private double _radiusY;

        public PointShapeViewModel TopLeft
        {
            get => _topLeft;
            set => this.RaiseAndSetIfChanged(ref _topLeft, value);
        }

        public PointShapeViewModel BottomRight
        {
            get => _bottomRight;
            set => this.RaiseAndSetIfChanged(ref _bottomRight, value);
        }

        public bool IsStroked
        {
            get => _isStroked;
            set => this.RaiseAndSetIfChanged(ref _isStroked, value);
        }

        public bool IsFilled
        {
            get => _isFilled;
            set => this.RaiseAndSetIfChanged(ref _isFilled, value);
        }

        public double RadiusX
        {
            get => _radiusX;
            set => this.RaiseAndSetIfChanged(ref _radiusX, value);
        }

        public double RadiusY
        {
            get => _radiusY;
            set => this.RaiseAndSetIfChanged(ref _radiusY, value);
        }
    }
}
