using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using SimpleDraw.ViewModels;

namespace SimpleDraw.Views
{
    public class SimpleCanvas : Canvas
    {
        private ObservableCollection<ViewModelBase> _copy = new ObservableCollection<ViewModelBase>();

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

            if (!(DataContext is CanvasViewModel canvas))
            {
                return;
            }

            var point = e.GetCurrentPoint(this);
            var type = point.Properties.PointerUpdateKind;
            canvas.Tool?.Moved(canvas, point.Position.X, point.Position.Y, ToToolPointerType(type), ToToolKeyModifiers(e.KeyModifiers));
            InvalidateVisual();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            
            if (!(DataContext is CanvasViewModel canvas))
            {
                return;
            }

            switch (e.Key)
            {
                case Key.X:
                    {
                        if (e.KeyModifiers == KeyModifiers.Control)
                        {
                            Cut(canvas);
                            InvalidateVisual();
                        }
                    }
                    break;
                case Key.C:
                    {
                        if (e.KeyModifiers == KeyModifiers.Control)
                        {
                            Copy(canvas);
                        }
                    }
                    break;
                case Key.V:
                    {
                        if (e.KeyModifiers == KeyModifiers.Control)
                        {
                            Paste(canvas);
                            InvalidateVisual();
                        }
                    }
                    break;
                case Key.Delete:
                    {
                        if (e.KeyModifiers == KeyModifiers.None)
                        {
                            Delete(canvas);
                            InvalidateVisual();
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
                case Key.A:
                    {
                        if (e.KeyModifiers == KeyModifiers.Control)
                        {
                            canvas.Selected.Clear();

                            foreach (var item in canvas.Items)
                            {
                                canvas.Selected.Add(item);
                            }
                        }
                    }
                    break;
                case Key.N:
                    {
                        if (e.KeyModifiers == KeyModifiers.None)
                        {
                            SetTool<NoneToolViewModel>(canvas);
                        }
                    }
                    break;
                case Key.S:
                    {
                        if (e.KeyModifiers == KeyModifiers.None)
                        {
                            SetTool<SelectionToolViewModel>(canvas);
                        }
                    }
                    break;
                case Key.L:
                    {
                        if (e.KeyModifiers == KeyModifiers.None)
                        {
                            SetTool<LineToolViewModel>(canvas);
                        }
                    }
                    break;
                case Key.R:
                    {
                        if (e.KeyModifiers == KeyModifiers.None)
                        {
                            SetTool<RectangleToolViewModel>(canvas);
                        }
                    }
                    break;
            }
        }

        private void SetTool<T>(CanvasViewModel canvas) where T : ToolBaseViewModel
        {
            foreach (var tool in canvas.Tools)
            {
                if (tool is T toolType)
                {
                    canvas.Tool = toolType;
                }
            }
        }

        private void Cut(CanvasViewModel canvas)
        {
            var shared = new Dictionary<ViewModelBase, ViewModelBase>();

            _copy.Clear();

            foreach (var item in canvas.Selected)
            {
                _copy.Add(item.Clone(shared));
            }

            foreach (var item in canvas.Selected)
            {
                canvas.Items.Remove(item);
            }

            canvas.Selected.Clear();
        }

        private void Copy(CanvasViewModel canvas)
        {
            var shared = new Dictionary<ViewModelBase, ViewModelBase>();

            _copy.Clear();

            foreach (var item in canvas.Selected)
            {
                _copy.Add(item.Clone(shared));
            }
        }

        private void Paste(CanvasViewModel canvas)
        {
            var shared = new Dictionary<ViewModelBase, ViewModelBase>();

            canvas.Selected.Clear();

            foreach (var item in _copy)
            {
                var clone = item.Clone(shared);
                canvas.Items.Add(clone);
                canvas.Selected.Add(clone);
            }
        }

        private void Delete(CanvasViewModel canvas)
        {
            foreach (var item in canvas.Selected)
            {
                canvas.Items.Remove(item);
            }

            canvas.Selected.Clear();
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
