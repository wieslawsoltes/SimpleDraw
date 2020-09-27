using System.Collections.ObjectModel;
using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    public class LinearGradientBrushViewModel : GradientBrushViewModel
    {
        private RelativePointViewModel _startPoint;
        private RelativePointViewModel _endPoint;

        public RelativePointViewModel StartPoint
        {
            get => _startPoint;
            set => this.RaiseAndSetIfChanged(ref _startPoint, value);
        }

        public RelativePointViewModel EndPoint
        {
            get => _endPoint;
            set => this.RaiseAndSetIfChanged(ref _endPoint, value);
        }

        public LinearGradientBrushViewModel()
            : base()
        {
        }

        public LinearGradientBrushViewModel(ObservableCollection<GradientStopViewModel> gradientStops, GradientSpreadMethod spreadMethod = GradientSpreadMethod.Pad, RelativePointViewModel startPoint = null, RelativePointViewModel endPoint = null)
            : base(gradientStops, spreadMethod)
        {
            _startPoint = startPoint;
            _endPoint = endPoint;
        }
    }
}
