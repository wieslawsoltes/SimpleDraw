using System.Collections.Generic;
using System.Runtime.Serialization;
using ReactiveUI;
using SimpleDraw.ViewModels.Primitives;

namespace SimpleDraw.ViewModels.Media
{
    [DataContract(IsReference = true)]
    public class RelativePointViewModel : ViewModelBase
    {
        private PointViewModel _point;
        private RelativeUnit _unit;

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public PointViewModel Point
        {
            get => _point;
            set => this.RaiseAndSetIfChanged(ref _point, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public RelativeUnit Unit
        {
            get => _unit;
            set => this.RaiseAndSetIfChanged(ref _unit, value);
        }

        public RelativePointViewModel()
        {
        }

        public RelativePointViewModel(PointViewModel point, RelativeUnit unit)
        {
            _point = point;
            _unit = unit;
        }

        public RelativePointViewModel(double x, double y, RelativeUnit unit)
        {
            _point = new PointViewModel(x, y);
            _unit = unit;
        }

        public RelativePointViewModel CloneSelf(Dictionary<ViewModelBase, ViewModelBase> shared)
        {
            if (shared.TryGetValue(this, out var value))
            {
                return value as RelativePointViewModel;
            }

            var copy = new RelativePointViewModel()
            {
                Point = _point?.CloneSelf(shared),
                Unit = _unit
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
