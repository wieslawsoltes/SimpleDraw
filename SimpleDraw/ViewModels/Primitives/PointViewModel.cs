using System.Collections.Generic;
using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    public class PointViewModel : ViewModelBase
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

        public PointViewModel()
        {
        }

        public PointViewModel(double x, double y)
        {
            _x = x;
            _y = y;
        }

        public PointViewModel Copy(Dictionary<ViewModelBase, ViewModelBase> shared)
        {
            if (shared.TryGetValue(this, out var value))
            {
                return value as PointViewModel;
            }

            var copy = new PointViewModel()
            {
                X = _x,
                Y = _y
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
