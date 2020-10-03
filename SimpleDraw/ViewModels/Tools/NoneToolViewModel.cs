using System.Collections.Generic;
using System.Runtime.Serialization;
using SimpleDraw.ViewModels.Containers;

namespace SimpleDraw.ViewModels.Tools
{
    [DataContract(IsReference = true)]
    public class NoneToolViewModel : ToolBaseViewModel
    {
        [IgnoreDataMember]
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

        public override ToolBaseViewModel CloneSelf(Dictionary<ViewModelBase, ViewModelBase> shared)
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
            return CloneSelf(shared);
        }
    }
}
