using System.Collections.Generic;

namespace SimpleDraw.ViewModels
{
    public abstract class ToolBaseViewModel : ViewModelBase
    {
        public abstract string Name { get; }
        public abstract void Pressed(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers);
        
        public abstract void Released(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers);
        
        public abstract void Moved(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers);
        
        public abstract ToolBaseViewModel Copy(Dictionary<ViewModelBase, ViewModelBase> shared);
    }
}
