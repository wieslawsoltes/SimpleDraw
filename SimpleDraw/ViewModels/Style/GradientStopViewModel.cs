using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    public class GradientStopViewModel : ViewModelBase
    {
        private ColorViewModel _color;
        private double _offset;

        public ColorViewModel Color
        {
            get => _color;
            set => this.RaiseAndSetIfChanged(ref _color, value);
        }

        public double Offset
        {
            get => _offset;
            set => this.RaiseAndSetIfChanged(ref _offset, value);
        }

        public GradientStopViewModel()
        {
        }

        public GradientStopViewModel(ColorViewModel color, double offset)
        {
            _color = color;
            _offset = offset;
        }
    }
}
