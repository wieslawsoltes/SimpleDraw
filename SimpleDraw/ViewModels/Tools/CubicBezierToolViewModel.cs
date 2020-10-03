using System.Collections.Generic;
using System.Runtime.Serialization;
using ReactiveUI;
using SimpleDraw.ViewModels.Containers;
using SimpleDraw.ViewModels.Tools.Shape;

namespace SimpleDraw.ViewModels.Tools
{
    [DataContract(IsReference = true)]
    public class CubicBezierToolViewModel : ToolBaseViewModel
    {
        private ShapeToolViewModel _shapeTool;

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public ShapeToolViewModel ShapeTool
        {
            get => _shapeTool;
            set => this.RaiseAndSetIfChanged(ref _shapeTool, value);
        }

        [IgnoreDataMember]
        public override string Name => "CubicBezier";

        public override void Pressed(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers)
        {
            _shapeTool?.Pressed(canvas, x, y, pointerType, keyModifiers);
        }

        public override void Released(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers)
        {
            _shapeTool?.Released(canvas, x, y, pointerType, keyModifiers);
        }

        public override void Moved(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers)
        {
            _shapeTool?.Moved(canvas, x, y, pointerType, keyModifiers);
        }

        public override ToolBaseViewModel CloneSelf(Dictionary<ViewModelBase, ViewModelBase> shared)
        {
            if (shared.TryGetValue(this, out var value))
            {
                return value as CubicBezierToolViewModel;
            }

            var copy = new CubicBezierToolViewModel()
            {
                ShapeTool = _shapeTool?.CloneSelf(shared)
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
