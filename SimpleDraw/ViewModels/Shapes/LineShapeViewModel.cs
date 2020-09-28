using System.Collections.Generic;
using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    public class LineShapeViewModel : ShapeBaseViewModel
    {
        private PointViewModel _start;
        private PointViewModel _end;
        private bool _isStroked;

        public PointViewModel Start
        {
            get => _start;
            set => this.RaiseAndSetIfChanged(ref _start, value);
        }

        public PointViewModel End
        {
            get => _end;
            set => this.RaiseAndSetIfChanged(ref _end, value);
        }

        public bool IsStroked
        {
            get => _isStroked;
            set => this.RaiseAndSetIfChanged(ref _isStroked, value);
        }

        public override ShapeBaseViewModel Copy(Dictionary<ViewModelBase, ViewModelBase> shared)
        {
            if (shared.TryGetValue(this, out var value))
            {
                return value as LineShapeViewModel;
            }

            var copy = new LineShapeViewModel()
            {
                Brush = _brush?.Copy(shared),
                Pen = _pen?.Copy(shared),
                Start = _start,
                End = _end,
                IsStroked = _isStroked
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
