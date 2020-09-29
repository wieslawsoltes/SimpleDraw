using System.Collections.Generic;
using System.Runtime.Serialization;
using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    [DataContract(IsReference = true)]
    public class LineShapeViewModel : ShapeBaseViewModel
    {
        private PointViewModel _start;
        private PointViewModel _end;
        private bool _isStroked;

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public PointViewModel Start
        {
            get => _start;
            set => this.RaiseAndSetIfChanged(ref _start, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public PointViewModel End
        {
            get => _end;
            set => this.RaiseAndSetIfChanged(ref _end, value);
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
                Start = _start?.CloneSelf(shared),
                End = _end?.CloneSelf(shared),
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
