namespace SimpleDraw.ViewModels
{
    public abstract class ToolBase : ViewModelBase
    {
        public abstract string Name { get; }
        public abstract void Pressed(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType);
        public abstract void Released(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType);
        public abstract void Moved(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType);
    }
}
