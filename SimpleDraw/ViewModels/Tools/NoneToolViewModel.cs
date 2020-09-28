using System.Collections.Generic;

namespace SimpleDraw.ViewModels
{
    public class NoneToolViewModel : ToolBaseViewModel
    {
        public override string Name => "None";

        public override void Pressed(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers)
        {
        }

        public override void Released(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers)
        {
        }

        public override void Moved(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers)
        {
        }

        public override ToolBaseViewModel Copy(Dictionary<ViewModelBase, ViewModelBase> shared)
        {
            if (shared.TryGetValue(this, out var value))
            {
                return value as NoneToolViewModel;
            }

            var copy = new NoneToolViewModel()
            {
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
