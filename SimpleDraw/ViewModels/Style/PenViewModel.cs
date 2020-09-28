using System.Collections.Generic;
using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    public class PenViewModel : ViewModelBase
    {
        private BrushViewModel _brush;
        private double _thickness;
        private DashStyleViewModel _dashStyle;
        private PenLineCap _lineCap;
        private PenLineJoin _lineJoin;
        private double _miterLimit;

        public BrushViewModel Brush
        {
            get => _brush;
            set => this.RaiseAndSetIfChanged(ref _brush, value);
        }

        public double Thickness
        {
            get => _thickness;
            set => this.RaiseAndSetIfChanged(ref _thickness, value);
        }

        public DashStyleViewModel DashStyle
        {
            get => _dashStyle;
            set => this.RaiseAndSetIfChanged(ref _dashStyle, value);
        }

        public PenLineCap LineCap
        {
            get => _lineCap;
            set => this.RaiseAndSetIfChanged(ref _lineCap, value);
        }

        public PenLineJoin LineJoin
        {
            get => _lineJoin;
            set => this.RaiseAndSetIfChanged(ref _lineJoin, value);
        }

        public double MiterLimit
        {
            get => _miterLimit;
            set => this.RaiseAndSetIfChanged(ref _miterLimit, value);
        }

        public PenViewModel()
        {
        }

        public PenViewModel(BrushViewModel brush, double thickness = 1, DashStyleViewModel dashStyle = null, PenLineCap lineCap = PenLineCap.Flat, PenLineJoin lineJoin = PenLineJoin.Miter, double miterLimit = 10)
        {
            _brush = brush;
            _dashStyle = dashStyle;
            _lineCap = lineCap;
            _lineJoin = lineJoin;
            _miterLimit = miterLimit;
            _thickness = thickness;
        }

        public PenViewModel Copy(Dictionary<ViewModelBase, ViewModelBase> shared)
        {
            if (shared.TryGetValue(this, out var value))
            {
                return value as PenViewModel;
            }

            var copy = new PenViewModel()
            {
                Brush = _brush.Copy(shared),
                Thickness = _thickness,
                DashStyle = _dashStyle.Copy(shared),
                LineCap = _lineCap,
                LineJoin = _lineJoin,
                MiterLimit = _miterLimit
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
