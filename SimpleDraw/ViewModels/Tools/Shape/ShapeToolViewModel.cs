using System.Collections.Generic;
using System.Runtime.Serialization;
using SimpleDraw.ViewModels.Containers;

namespace SimpleDraw.ViewModels.Tools.Shape
{
    [DataContract(IsReference = true)]
    public abstract class ShapeToolViewModel : ViewModelBase
    {
        public abstract void Pressed(IItemsCanvas canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers);

        public abstract void Released(IItemsCanvas canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers);

        public abstract void Moved(IItemsCanvas canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers);

        public abstract ShapeToolViewModel CloneSelf(Dictionary<ViewModelBase, ViewModelBase> shared);
    }
}
