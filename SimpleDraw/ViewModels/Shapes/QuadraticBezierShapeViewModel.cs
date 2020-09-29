using System.Collections.Generic;
using System.Runtime.Serialization;
using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    [DataContract(IsReference = true)]
    public class QuadraticBezierShapeViewModel : ShapeBaseViewModel
    {
        private PointViewModel _startPoint;
        private PointViewModel _control;
        private PointViewModel _endPoint;
        private bool _isStroked;
        private bool _isFilled;

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public PointViewModel StartPoint
        {
            get => _startPoint;
            set => this.RaiseAndSetIfChanged(ref _startPoint, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public PointViewModel Control
        {
            get => _control;
            set => this.RaiseAndSetIfChanged(ref _control, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public PointViewModel EndPoint
        {
            get => _endPoint;
            set => this.RaiseAndSetIfChanged(ref _endPoint, value);
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
                return value as QuadraticBezierShapeViewModel;
            }

            var copy = new QuadraticBezierShapeViewModel()
            {
                Brush = _brush?.CloneSelf(shared),
                Pen = _pen?.CloneSelf(shared),
                StartPoint = _startPoint?.CloneSelf(shared),
                Control = _control?.CloneSelf(shared),
                EndPoint = _endPoint?.CloneSelf(shared),
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
