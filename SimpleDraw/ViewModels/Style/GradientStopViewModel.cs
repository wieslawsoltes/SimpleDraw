using System.Collections.Generic;
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

        public GradientStopViewModel Copy(Dictionary<ViewModelBase, ViewModelBase> shared)
        {
            if (shared.TryGetValue(this, out var value))
            {
                return value as GradientStopViewModel;
            }

            var copy = new GradientStopViewModel()
            {
                Color = _color.Copy(shared),
                Offset = _offset
            };

            shared[this] = copy;
            return copy;
        }

        public override ViewModelBase Clone(Dictionary<ViewModelBase, ViewModelBase> shared)
        {
            return Copy(shared);
        }
    }
}
