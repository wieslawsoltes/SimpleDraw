using System.Collections.Generic;
using System.Runtime.Serialization;
using SimpleDraw.ViewModels.Containers;

namespace SimpleDraw.ViewModels.Tools
{
    [DataContract(IsReference = true)]
    public abstract class ToolBaseViewModel : ViewModelBase
    {
        public abstract string Name { get; }

        public abstract void Pressed(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers);
        
        public abstract void Released(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers);
        
        public abstract void Moved(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers);
        
        public abstract ToolBaseViewModel CloneSelf(Dictionary<ViewModelBase, ViewModelBase> shared);
    }
}
