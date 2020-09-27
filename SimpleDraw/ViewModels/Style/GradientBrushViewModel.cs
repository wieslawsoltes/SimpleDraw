using System.Collections.ObjectModel;
using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    public abstract class GradientBrushViewModel : BrushViewModel
    {
        private ObservableCollection<GradientStopViewModel> _gradientStops;
        private GradientSpreadMethod _spreadMethod;

        public ObservableCollection<GradientStopViewModel> GradientStops
        {
            get => _gradientStops;
            set => this.RaiseAndSetIfChanged(ref _gradientStops, value);
        }

        public GradientSpreadMethod SpreadMethod
        {
            get => _spreadMethod;
            set => this.RaiseAndSetIfChanged(ref _spreadMethod, value);
        }

        protected GradientBrushViewModel()
        {
        }

        protected GradientBrushViewModel(ObservableCollection<GradientStopViewModel> gradientStops, GradientSpreadMethod spreadMethod)
        {
            _gradientStops = gradientStops;
            _spreadMethod = spreadMethod;
        }
    }
}
