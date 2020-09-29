using System.Linq;
using System.Threading.Tasks;
using Avalonia;
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

        private void Canvas_Invalidate(object sender, System.EventArgs e)
        {
            InvalidateVisual();
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            if (!(DataContext is CanvasViewModel canvas))
            {
                return;
            }

            canvas.InvalidateCanvas += Canvas_Invalidate;
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);

            if (!(DataContext is CanvasViewModel canvas))
            {
                return;
            }

            canvas.InvalidateCanvas -= Canvas_Invalidate;
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
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            base.OnPointerMoved(e);

            if (!(DataContext is CanvasViewModel canvas))
            {
                return;
            }

            var point = e.GetCurrentPoint(this);
            var type = point.Properties.PointerUpdateKind;
            canvas.Tool?.Moved(canvas, point.Position.X, point.Position.Y, ToToolPointerType(type), ToToolKeyModifiers(e.KeyModifiers));
        }

        protected override async void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (!(DataContext is CanvasViewModel canvas))
            {
                return;
            }

            switch (e.Key)
            {
                case Key.A:
                    {
                        if (e.KeyModifiers == KeyModifiers.Control)
                        {
                            canvas.SelectAll();
                        }
                    }
                    break;
                case Key.C:
                    {
                        if (e.KeyModifiers == KeyModifiers.Control)
                        {
                            canvas.Copy();
                        }
                    }
                    break;
                case Key.L:
                    {
                        if (e.KeyModifiers == KeyModifiers.None)
                        {
                            canvas.SetTool("Line");
                        }
                    }
                    break;
                case Key.N:
                    {
                        if (e.KeyModifiers == KeyModifiers.None)
                        {
                            canvas.SetTool("None");
                        }
                    }
                    break;
                case Key.O:
                    {
                        if (e.KeyModifiers == KeyModifiers.Control)
                        {
                            await Open();
                        }
                    }
                    break;
                case Key.R:
                    {
                        if (e.KeyModifiers == KeyModifiers.None)
                        {
                            canvas.SetTool("Rectangle");
                        }
                    }
                    break;
                case Key.S:
                    {
                        if (e.KeyModifiers == KeyModifiers.None)
                        {
                            canvas.SetTool("Selection");
                        }

                        if (e.KeyModifiers == KeyModifiers.Control)
                        {
                            await Save(canvas);
                        }
                    }
                    break;
                case Key.V:
                    {
                        if (e.KeyModifiers == KeyModifiers.Control)
                        {
                            canvas.Paste();
                        }
                    }
                    break;
                case Key.X:
                    {
                        if (e.KeyModifiers == KeyModifiers.Control)
                        {
                            canvas.Cut();
                        }
                    }
                    break;
                case Key.Delete:
                    {
                        if (e.KeyModifiers == KeyModifiers.None)
                        {
                            canvas.Delete();
                        }
                    }
                    break;
                case Key.Escape:
                    {
                        if (e.KeyModifiers == KeyModifiers.None)
                        {
                            canvas.Selected.Clear();
                        }
                    }
                    break;
            }
        }

        public async Task Open()
        {
            var dlg = new OpenFileDialog() { Title = "Open" };
            dlg.Filters.Add(new FileDialogFilter() { Name = "Json", Extensions = { "json" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });

            var window = this.VisualRoot as Window;
            var result = await dlg.ShowAsync(window);
            if (result != null)
            {
                var path = result.FirstOrDefault();
                if (path != null)
                {
                    var canvasOpen = App.Open(path);
                    if (canvasOpen != null)
                    {
                        if (DataContext is CanvasViewModel canvasOld)
                        {
                            canvasOld.InvalidateCanvas -= Canvas_Invalidate;
                        }
                        window.DataContext = canvasOpen;
                        canvasOpen.InvalidateCanvas += Canvas_Invalidate;
                        canvasOpen.Invalidate();
                    }
                }
            }
        }

        public async Task Save(CanvasViewModel canvas)
        {
            var dlg = new SaveFileDialog() { Title = "Save" };
            dlg.Filters.Add(new FileDialogFilter() { Name = "Json", Extensions = { "json" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
            dlg.InitialFileName = "canvas";
            dlg.DefaultExtension = "json";

            var window = this.VisualRoot as Window;
            var path = await dlg.ShowAsync(window);
            if (path != null)
            {
                App.Save(path, canvas);
            }
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (!(DataContext is CanvasViewModel canvas))
            {
                return;
            }

            AvaloniaRenderer.Render(context, canvas);
        }
    }
}
