using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    public class SolidColorBrushViewModel : BrushViewModel
    {
        private ColorViewModel _color;

        public ColorViewModel Color
        {
            get => _color;
            set => this.RaiseAndSetIfChanged(ref _color, value);
        }

        public SolidColorBrushViewModel()
        {
        }

        public SolidColorBrushViewModel(ColorViewModel color)
        {
            _color = color;
        }
    }
}
