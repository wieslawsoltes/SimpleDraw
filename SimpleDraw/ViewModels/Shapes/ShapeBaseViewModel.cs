using System.Collections.Generic;
using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    public abstract class ShapeBaseViewModel : ViewModelBase
    {
        protected BrushViewModel _brush;
        protected PenViewModel _pen;

        public BrushViewModel Brush
        {
            get => _brush;
            set => this.RaiseAndSetIfChanged(ref _brush, value);
        }

        public PenViewModel Pen
        {
            get => _pen;
            set => this.RaiseAndSetIfChanged(ref _pen, value);
        }

        public abstract ShapeBaseViewModel Copy(Dictionary<ViewModelBase, ViewModelBase> shared);
    }
}
