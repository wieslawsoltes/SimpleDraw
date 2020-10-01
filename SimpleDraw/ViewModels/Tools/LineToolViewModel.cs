using System.Collections.Generic;
using System.Runtime.Serialization;
using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    [DataContract(IsReference = true)]
    public class LineToolViewModel : ToolBaseViewModel
    {
        private LineShapeToolViewModel _lineShapeTool;

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public LineShapeToolViewModel LineShapeTool
        {
            get => _lineShapeTool;
            set => this.RaiseAndSetIfChanged(ref _lineShapeTool, value);
        }

        [IgnoreDataMember]
        public override string Name => "Line";

        public override void Pressed(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers)
        {
            _lineShapeTool?.Pressed(canvas, x, y, pointerType, keyModifiers);
        }

        public override void Released(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers)
        {
            _lineShapeTool?.Released(canvas, x, y, pointerType, keyModifiers);
        }

        public override void Moved(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers)
        {
            _lineShapeTool?.Moved(canvas, x, y, pointerType, keyModifiers);
        }

        public override ToolBaseViewModel CloneSelf(Dictionary<ViewModelBase, ViewModelBase> shared)
        {
            if (shared.TryGetValue(this, out var value))
            {
                return value as LineToolViewModel;
            }

            var copy = new LineToolViewModel()
            {
                LineShapeTool = _lineShapeTool?.CloneSelf(shared)
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
