using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using SimpleDraw.ViewModels;

namespace SimpleDraw.Views
{
    public class SimpleCanvas : Canvas
    {
        private ToolPointerType ToToolPointerType(PointerUpdateKind pointerUpdateKind)
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

        private ToolKeyModifiers ToToolKeyModifiers(KeyModifiers keyModifiers)
        {
            var result = ToolKeyModifiers.None;

            if (keyModifiers.HasFlag(KeyModifiers.Alt))
            {
                result |= ToolKeyModifiers.Alt;
            }

            if (keyModifiers.HasFlag(KeyModifiers.Control))
            {
                result |= ToolKeyModifiers.Control;
            }

            if (keyModifiers.HasFlag(KeyModifiers.Shift))
            {
                result |= ToolKeyModifiers.Shift;
            }

            if (keyModifiers.HasFlag(KeyModifiers.Meta))
            {
                result |= ToolKeyModifiers.Meta;
            }

            return result;
        }

        public SimpleCanvas()
        {
            Focusable = true;
            Focus();
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            if (!(DataContext is CanvasViewModel canvas))
            {
                return;
            }

            var point = e.GetCurrentPoint(this);
            var type = point.Properties.PointerUpdateKind;
            canvas.Tool?.Pressed(canvas, point.Position.X, point.Position.Y, ToToolPointerType(type), ToToolKeyModifiers(e.KeyModifiers));
            InvalidateVisual();
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);

            if (!(DataContext is CanvasViewModel canvas))
            {
                return;
            }

            var point = e.GetCurrentPoint(this);
            var type = point.Properties.PointerUpdateKind;
            canvas.Tool?.Released(canvas, point.Position.X, point.Position.Y, ToToolPointerType(type), ToToolKeyModifiers(e.KeyModifiers));
            InvalidateVisual();
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            base.OnPointerMoved(e);

            if (DataContext is CanvasViewModel canvas)
            {
                var point = e.GetCurrentPoint(this);
                var type = point.Properties.PointerUpdateKind;
                canvas.Tool?.Moved(canvas, point.Position.X, point.Position.Y, ToToolPointerType(type), ToToolKeyModifiers(e.KeyModifiers));
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
