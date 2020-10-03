using System.Collections.Generic;
using System.Runtime.Serialization;
using ReactiveUI;
using SimpleDraw.ViewModels.Primitives;

namespace SimpleDraw.ViewModels.Shapes
{
    [DataContract(IsReference = true)]
    public class LineShapeViewModel : ShapeBaseViewModel
    {
        private PointViewModel _startPoint;
        private PointViewModel _point;
        private bool _isStroked;

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public PointViewModel StartPoint
        {
            get => _startPoint;
            set => this.RaiseAndSetIfChanged(ref _startPoint, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public PointViewModel Point
        {
            get => _point;
            set => this.RaiseAndSetIfChanged(ref _point, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool IsStroked
        {
            get => _isStroked;
            set => this.RaiseAndSetIfChanged(ref _isStroked, value);
        }

        public override ShapeBaseViewModel CloneSelf(Dictionary<ViewModelBase, ViewModelBase> shared)
        {
            if (shared.TryGetValue(this, out var value))
            {
                return value as LineShapeViewModel;
            }

            var copy = new LineShapeViewModel()
            {
                Brush = _brush?.CloneSelf(shared),
                Pen = _pen?.CloneSelf(shared),
                StartPoint = _startPoint?.CloneSelf(shared),
                Point = _point?.CloneSelf(shared),
                IsStroked = _isStroked
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
