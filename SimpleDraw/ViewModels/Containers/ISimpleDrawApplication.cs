namespace SimpleDraw.ViewModels.Containers
{
    public interface ISimpleDrawApplication
    {
        CanvasViewModel New();
        CanvasViewModel Open(string path);
        void Save(string path, CanvasViewModel canvas);
    }
}
