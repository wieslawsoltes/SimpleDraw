using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    [DataContract(IsReference = true)]
    public class LinearGradientBrushViewModel : GradientBrushViewModel
    {
        private RelativePointViewModel _startPoint;
        private RelativePointViewModel _endPoint;

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public RelativePointViewModel StartPoint
        {
            get => _startPoint;
            set => this.RaiseAndSetIfChanged(ref _startPoint, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
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

        public override BrushViewModel Copy(Dictionary<ViewModelBase, ViewModelBase> shared)
        {
            if (shared.TryGetValue(this, out var value))
            {
                return value as LinearGradientBrushViewModel;
            }

            var gradientStops = new ObservableCollection<GradientStopViewModel>();

            foreach (var gradientStop in _gradientStops)
            {
                gradientStops.Add(gradientStop.Copy(shared));
            }

            var copy = new LinearGradientBrushViewModel()
            {
                GradientStops = gradientStops,
                SpreadMethod = _spreadMethod,
                StartPoint = _startPoint?.Copy(shared),
                EndPoint = _endPoint?.Copy(shared)
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
