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
    }
}
