using System.Collections.Generic;
using System.Runtime.Serialization;
using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    [DataContract(IsReference = true)]
    public class CubicBezierShapeViewModel : ShapeBaseViewModel
    {
        private PointViewModel _startPoint;
        private PointViewModel _point1;
        private PointViewModel _point2;
        private PointViewModel _point3;
        private bool _isStroked;
        private bool _isFilled;

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public PointViewModel StartPoint
        {
            get => _startPoint;
            set => this.RaiseAndSetIfChanged(ref _startPoint, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public PointViewModel Point1
        {
            get => _point1;
            set => this.RaiseAndSetIfChanged(ref _point1, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public PointViewModel Point2
        {
            get => _point2;
            set => this.RaiseAndSetIfChanged(ref _point2, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public PointViewModel Point3
        {
            get => _point3;
            set => this.RaiseAndSetIfChanged(ref _point3, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool IsStroked
        {
            get => _isStroked;
            set => this.RaiseAndSetIfChanged(ref _isStroked, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool IsFilled
        {
            get => _isFilled;
            set => this.RaiseAndSetIfChanged(ref _isFilled, value);
        }

        public override ShapeBaseViewModel CloneSelf(Dictionary<ViewModelBase, ViewModelBase> shared)
        {
            if (shared.TryGetValue(this, out var value))
            {
                return value as CubicBezierShapeViewModel;
            }

            var copy = new CubicBezierShapeViewModel()
            {
                Brush = _brush?.CloneSelf(shared),
                Pen = _pen?.CloneSelf(shared),
                StartPoint = _startPoint?.CloneSelf(shared),
                Point1 = _point1?.CloneSelf(shared),
                Point2 = _point2?.CloneSelf(shared),
                Point3 = _point3?.CloneSelf(shared),
                IsStroked = _isStroked,
                IsFilled = _isFilled
            };

            shared[this] = copy;
            return copy;
        }

        public override ViewModelBase Clone(Dictionary<ViewModelBase, ViewModelBase> shared)
        {
            return CloneSelf(shared);
        }
    }
}
