using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using SimpleDraw.Avalonia;
using SimpleDraw.Skia;
using SimpleDraw.ViewModels.Containers;
using SimpleDraw.ViewModels.Tools;
using SkiaSharp;

namespace SimpleDraw.Controls;

public class SimpleCanvas : Canvas
{
    public static ISimpleDrawApplication App { get; set; }

    public static readonly StyledProperty<IInputElement> InputSourceProperty =
        AvaloniaProperty.Register<SimpleCanvas, IInputElement>(nameof(InputSource));

    public static readonly StyledProperty<bool> CustomDrawProperty =
        AvaloniaProperty.Register<SimpleCanvas, bool>(nameof(CustomDraw));

    public IInputElement InputSource
    {
        get => GetValue(InputSourceProperty);
        set => SetValue(InputSourceProperty, value);
    }

    public bool CustomDraw
    {
        get => GetValue(CustomDrawProperty);
        set => SetValue(CustomDrawProperty, value);
    }

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

    private void Canvas_Invalidate(object sender, EventArgs e)
    {
        InvalidateVisual();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        var inputSource = InputSource ?? this;
        if (inputSource != null)
        {
            inputSource.PointerPressed += InputSource_PointerPressed;
            inputSource.PointerReleased += InputSource_PointerReleased;
            inputSource.PointerMoved += InputSource_PointerMoved;
            inputSource.KeyDown += InputSource_KeyDown;
        }

        if (!(DataContext is CanvasViewModel canvas))
        {
            return;
        }

        canvas.InvalidateCanvas += Canvas_Invalidate;
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);

        var inputSource = InputSource ?? this;
        if (inputSource != null)
        {
            inputSource.PointerPressed -= InputSource_PointerPressed;
            inputSource.PointerReleased -= InputSource_PointerReleased;
            inputSource.PointerMoved -= InputSource_PointerMoved;
            inputSource.KeyDown -= InputSource_KeyDown;
        }

        if (!(DataContext is CanvasViewModel canvas))
        {
            return;
        }

        canvas.InvalidateCanvas -= Canvas_Invalidate;
    }

    private void InputSource_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        if (!(DataContext is CanvasViewModel canvas))
        {
            return;
        }

        var point = e.GetCurrentPoint(this);
        var type = point.Properties.PointerUpdateKind;
        var pointerType = ToToolPointerType(type);
        var keyModifiers = ToToolKeyModifiers(e.KeyModifiers);
        canvas.Tool?.Pressed(canvas, point.Position.X, point.Position.Y, pointerType, keyModifiers);
    }

    private void InputSource_PointerReleased(object sender, PointerReleasedEventArgs e)
    {
        if (!(DataContext is CanvasViewModel canvas))
        {
            return;
        }

        var point = e.GetCurrentPoint(this);
        var type = point.Properties.PointerUpdateKind;
        var pointerType = ToToolPointerType(type);
        var keyModifiers = ToToolKeyModifiers(e.KeyModifiers);
        canvas.Tool?.Released(canvas, point.Position.X, point.Position.Y, pointerType, keyModifiers);
    }

    private void InputSource_PointerMoved(object sender, PointerEventArgs e)
    {
        if (!(DataContext is CanvasViewModel canvas))
        {
            return;
        }

        var point = e.GetCurrentPoint(this);
        var type = point.Properties.PointerUpdateKind;
        var pointerType = ToToolPointerType(type);
        var keyModifiers = ToToolKeyModifiers(e.KeyModifiers);
        canvas.Tool?.Moved(canvas, point.Position.X, point.Position.Y, pointerType, keyModifiers);
    }

    private async void InputSource_KeyDown(object sender, KeyEventArgs e)
    {
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
                if (e.KeyModifiers == KeyModifiers.None)
                {
                    if (canvas.Tool is PathToolViewModel pathTool && pathTool.Mode != PathToolMode.CubicBezier)
                    {
                        pathTool.PreviousMode = pathTool.Mode;
                        pathTool.Mode = PathToolMode.CubicBezier;
                    }
                    else
                    {
                        canvas.SetTool("CubicBezier");
                    }
                }

                if (e.KeyModifiers == KeyModifiers.Control)
                {
                    canvas.Copy();
                }
            }
                break;
            case Key.E:
            {
                if (e.KeyModifiers == KeyModifiers.None)
                {
                    canvas.SetTool("Ellipse");
                }

                if (e.KeyModifiers == KeyModifiers.Control)
                {
                    await Export(canvas);
                }
            }
                break;
            case Key.G:
            {
                if (e.KeyModifiers == KeyModifiers.Control)
                {
                    canvas.Group();
                }
            }
                break;
            case Key.H:
            {
                if (e.KeyModifiers == KeyModifiers.None)
                {
                    canvas.SetTool("Path");
                }
            }
                break;
            case Key.L:
            {
                if (e.KeyModifiers == KeyModifiers.None)
                {
                    if (canvas.Tool is PathToolViewModel pathTool && pathTool.Mode != PathToolMode.Line)
                    {
                        pathTool.PreviousMode = pathTool.Mode;
                        pathTool.Mode = PathToolMode.Line;
                    }
                    else
                    {
                        canvas.SetTool("Line");
                    }
                }
            }
                break;
            case Key.M:
            {
                if (e.KeyModifiers == KeyModifiers.None)
                {
                    if (canvas.Tool is PathToolViewModel pathTool)
                    {
                        pathTool.PreviousMode = pathTool.Mode;
                        pathTool.Mode = PathToolMode.Move;
                    }
                }
            }
                break;
            case Key.N:
            {
                if (e.KeyModifiers == KeyModifiers.None)
                {
                    canvas.SetTool("None");
                }

                if (e.KeyModifiers == KeyModifiers.Control)
                {
                    New();
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
            case Key.Q:
            {
                if (e.KeyModifiers == KeyModifiers.None)
                {
                    if (canvas.Tool is PathToolViewModel pathTool && pathTool.Mode != PathToolMode.QuadraticBezier)
                    {
                        pathTool.PreviousMode = pathTool.Mode;
                        pathTool.Mode = PathToolMode.QuadraticBezier;
                    }
                    else
                    {
                        canvas.SetTool("QuadraticBezier");
                    }
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
            case Key.U:
            {
                if (e.KeyModifiers == KeyModifiers.Control)
                {
                    canvas.Ungroup();
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

    private void Load(Window window, CanvasViewModel canvasOpen)
    {
        if (window.DataContext is CanvasViewModel canvasOld)
        {
            canvasOld.InvalidateCanvas -= Canvas_Invalidate;
        }
        window.DataContext = canvasOpen;
        canvasOpen.InvalidateCanvas += Canvas_Invalidate;
        canvasOpen.Invalidate();
    }

    public void New()
    {
        var window = this.VisualRoot as Window;
        var canvasNew = App.New();
        Load(window, canvasNew);
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
                    Load(window, canvasOpen);
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

    public async Task Export(CanvasViewModel canvas)
    {
        var dlg = new SaveFileDialog() { Title = "Save" };
        dlg.Filters.Add(new FileDialogFilter() { Name = "Skp", Extensions = { "skp" } });
        dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
        dlg.InitialFileName = "canvas";
        dlg.DefaultExtension = "skp";

        var window = this.VisualRoot as Window;
        var path = await dlg.ShowAsync(window);
        if (path != null)
        {
            using var skPictureRecorder = new SKPictureRecorder();
            var cullRect = SKRect.Create((float)this.Bounds.X, (float)this.Bounds.Y, (float)this.Bounds.Width, (float)this.Bounds.Height);
            using var skCanvas = skPictureRecorder.BeginRecording(cullRect);
            SkiaRenderer.Render(skCanvas, canvas);
            using var skPicture = skPictureRecorder.EndRecording();
            using var stream = File.Create(path);
            skPicture.Serialize(stream);
        }
    }

    public void Exit()
    {
        var window = this.VisualRoot as Window;
        window.Close();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (!(DataContext is CanvasViewModel canvas))
        {
            return;
        }

        if (CustomDraw)
        {
            if (InputSource != null)
            {
                var translated = this.TranslatePoint(new Point(0, 0), InputSource);
                var position = new Point(-translated.Value.X, -translated.Value.Y);
                var bounds = new Rect(position, InputSource.Bounds.Size);
                context.Custom(new SkiaCustomDraw(canvas, bounds));
            }
            else
            {
                var bounds = new Rect(new Point(0, 0), this.Bounds.Size);
                context.Custom(new SkiaCustomDraw(canvas, bounds));
            }
        }
        else
        {
            AvaloniaRenderer.Render(context, canvas);
        }
    }

    private class SkiaCustomDraw : ICustomDrawOperation
    {
        private readonly Rect _bounds;
        private readonly CanvasViewModel _canvas;

        public SkiaCustomDraw(CanvasViewModel canvas, Rect bounds)
        {
            _canvas = canvas;
            _bounds = bounds;
        }

        public Rect Bounds => _bounds;

        public void Dispose()
        {
        }

        public bool Equals(ICustomDrawOperation other) => false;

        public bool HitTest(Point p) => false;

        public void Render(IDrawingContextImpl context)
        {
            var skCanvas = (context as ISkiaDrawingContextImpl)?.SkCanvas;
            if (skCanvas == null)
            {
                return;
            }
            SkiaRenderer.Render(skCanvas, _canvas);
        }
    }
}