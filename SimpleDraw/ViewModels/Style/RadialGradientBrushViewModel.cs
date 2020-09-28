using System.Collections.Generic;
using System.Collections.ObjectModel;
using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    public class RadialGradientBrushViewModel : GradientBrushViewModel
    {
        private RelativePointViewModel _center;
        private RelativePointViewModel _gradientOrigin;
        private double _radius;

        public RelativePointViewModel Center
        {
            get => _center;
            set => this.RaiseAndSetIfChanged(ref _center, value);
        }

        public RelativePointViewModel GradientOrigin
        {
            get => _gradientOrigin;
            set => this.RaiseAndSetIfChanged(ref _gradientOrigin, value);
        }

        public double Radius
        {
            get => _radius;
            set => this.RaiseAndSetIfChanged(ref _radius, value);
        }

        public RadialGradientBrushViewModel()
            : base()
        {
        }

        public RadialGradientBrushViewModel(ObservableCollection<GradientStopViewModel> gradientStops, GradientSpreadMethod spreadMethod = GradientSpreadMethod.Pad, RelativePointViewModel center = null, RelativePointViewModel gradientOrigin = null, double radius = 0.5)
            : base(gradientStops, spreadMethod)
        {
            _center = center;
            _gradientOrigin = gradientOrigin;
            _radius = radius;
        }

        public override BrushViewModel Copy(Dictionary<ViewModelBase, ViewModelBase> shared)
        {
            if (shared.TryGetValue(this, out var value))
            {
                return value as RadialGradientBrushViewModel;
            }

            var gradientStops = new ObservableCollection<GradientStopViewModel>();

            foreach (var gradientStop in _gradientStops)
            {
                gradientStops.Add(gradientStop.Copy(shared));
            }

            var copy = new RadialGradientBrushViewModel()
            {
                GradientStops = gradientStops,
                SpreadMethod = _spreadMethod,
                Center = _center?.Copy(shared),
                GradientOrigin = _gradientOrigin?.Copy(shared),
                Radius = _radius
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
