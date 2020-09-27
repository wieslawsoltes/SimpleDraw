using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using SimpleDraw.ViewModels;

namespace SimpleDraw.Views
{
    public class SimpleCanvas : Canvas
    {
        public ToolPointerType ToToolPointerType(PointerUpdateKind pointerUpdateKind)
        {
            switch (pointerUpdateKind)
            {
                case PointerUpdateKind.LeftButtonPressed:
                case PointerUpdateKind.LeftButtonReleased:
                    return ToolPointerType.Left;
                case PointerUpdateKind.RightButtonPressed:
                case PointerUpdateKind.RightButtonReleased:
                    return ToolPointerType.Right;
                default:
                    return ToolPointerType.None;
            }
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            if (DataContext is CanvasViewModel canvas)
            {
                var point = e.GetCurrentPoint(this);
                var type = point.Properties.PointerUpdateKind;
                canvas.Tool?.Pressed(canvas, point.Position.X, point.Position.Y, ToToolPointerType(type));
                InvalidateVisual();
            }
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);

            if (DataContext is CanvasViewModel canvas)
            {
                var point = e.GetCurrentPoint(this);
                var type = point.Properties.PointerUpdateKind;
                canvas.Tool?.Released(canvas, point.Position.X, point.Position.Y, ToToolPointerType(type));
                InvalidateVisual();
            }
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            base.OnPointerMoved(e);

            if (DataContext is CanvasViewModel canvas)
            {
                var point = e.GetCurrentPoint(this);
                var type = point.Properties.PointerUpdateKind;
                canvas.Tool?.Moved(canvas, point.Position.X, point.Position.Y, ToToolPointerType(type));
                InvalidateVisual();
            }
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (DataContext is CanvasViewModel canvas)
            {
                AvaloniaRenderer.Render(context, canvas);
            }
        }
    }
}
