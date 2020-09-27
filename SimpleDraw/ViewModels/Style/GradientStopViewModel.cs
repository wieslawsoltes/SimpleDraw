using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    public class GradientStopViewModel : ViewModelBase
    {
        private double _offset;
        private ColorViewModel _color;

        public double Offset
        {
            get => _offset;
            set => this.RaiseAndSetIfChanged(ref _offset, value);
        }

        public ColorViewModel Color
        {
            get => _color;
            set => this.RaiseAndSetIfChanged(ref _color, value);
        }

        public GradientStopViewModel()
        {
        }

        public GradientStopViewModel(double offset, ColorViewModel color)
        {
            _offset = offset;
            _color = color;
        }
    }
}
