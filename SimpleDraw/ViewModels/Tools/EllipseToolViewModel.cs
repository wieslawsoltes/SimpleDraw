using System.Collections.Generic;
using System.Runtime.Serialization;
using ReactiveUI;
using SimpleDraw.Skia;
using SimpleDraw.ViewModels.Containers;
using SimpleDraw.ViewModels.Media;
using SimpleDraw.ViewModels.Primitives;
using SimpleDraw.ViewModels.Shapes;

namespace SimpleDraw.ViewModels.Tools;

[DataContract(IsReference = true)]
public class EllipseToolViewModel : ToolBaseViewModel
{
    private enum EllipseState { TopLeft, BottomRight }
    private EllipseState _state = EllipseState.TopLeft;
    private EllipseShapeViewModel _ellipse;
    private BrushViewModel _brush;
    private PenViewModel _pen;
    private double _hitRadius;
    private bool _tryToConnect;
    private bool _isStroked;
    private bool _isFilled;

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public BrushViewModel Brush
    {
        get => _brush;
        set => this.RaiseAndSetIfChanged(ref _brush, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public PenViewModel Pen
    {
        get => _pen;
        set => this.RaiseAndSetIfChanged(ref _pen, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public bool IsStroked
    {
        get => _isStroked;
        set => this.RaiseAndSetIfChanged(ref _isStroked, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public bool IsFilled
    {
        get => _isFilled;
        set => this.RaiseAndSetIfChanged(ref _isFilled, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public double HitRadius
    {
        get => _hitRadius;
        set => this.RaiseAndSetIfChanged(ref _hitRadius, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public bool TryToConnect
    {
        get => _tryToConnect;
        set => this.RaiseAndSetIfChanged(ref _tryToConnect, value);
    }

    [IgnoreDataMember]
    public override string Name => "Ellipse";

    private void TryToHover(IItemsCanvas canvas, double x, double y)
    {
        if (_tryToConnect)
        {
            var result = SkiaHitTest.Contains(canvas.Items, (_) => true, x, y, _hitRadius);
            if (result != null)
            {
                canvas.Hovered.Add(result);
            }
        }
    }

    private void ResetHover(IItemsCanvas canvas)
    {
        canvas.Hovered.Clear();
    }

    public override void Pressed(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers)
    {
        switch (_state)
        {
            case EllipseState.TopLeft:
            {
                if (pointerType == ToolPointerType.Left)
                {
                    var shared = new Dictionary<ViewModelBase, ViewModelBase>();
                    var topLeft = default(PointViewModel);

                    ResetHover(canvas);

                    if (_tryToConnect)
                    {
                        var result = SkiaHitTest.Contains(canvas.Items, (_) => true, x, y, _hitRadius);
                        if (result is PointViewModel point)
                        {
                            topLeft = point;
                        }
                    }

                    _ellipse = new EllipseShapeViewModel()
                    {
                        TopLeft = topLeft ?? new PointViewModel(x, y),
                        BottomRight = new PointViewModel(x, y),
                        IsStroked = _isStroked,
                        IsFilled = _isFilled,
                        Brush = _brush.CloneSelf(shared),
                        Pen = _pen.CloneSelf(shared)
                    };
                    canvas.Decorators.Add(_ellipse);
                    canvas.Invalidate();
                    _state = EllipseState.BottomRight;
                }
            }
                break;
            case EllipseState.BottomRight:
            {
                if (pointerType == ToolPointerType.Left)
                {
                    var bottomRight = default(PointViewModel);

                    ResetHover(canvas);

                    if (_tryToConnect)
                    {
                        var result = SkiaHitTest.Contains(canvas.Items, (_) => true, x, y, _hitRadius);
                        if (result is PointViewModel point)
                        {
                            bottomRight = point;
                        }
                    }

                    if (bottomRight != null)
                    {
                        _ellipse.BottomRight = bottomRight;
                    }

                    canvas.Decorators.Remove(_ellipse);
                    canvas.Items.Add(_ellipse);
                    canvas.Invalidate();

                    _ellipse = null;
                    _state = EllipseState.TopLeft;
                }

                if (pointerType == ToolPointerType.Right)
                {
                    ResetHover(canvas);
                    canvas.Decorators.Remove(_ellipse);
                    canvas.Invalidate();
                    _ellipse = null;
                    _state = EllipseState.TopLeft;
                }
            }
                break;
        }
    }

    public override void Released(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers)
    {
    }

    public override void Moved(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers)
    {
        switch (_state)
        {
            case EllipseState.TopLeft:
            {
                ResetHover(canvas);
                TryToHover(canvas, x, y);
                canvas.Invalidate();
            }
                break;
            case EllipseState.BottomRight:
            {
                if (pointerType == ToolPointerType.None)
                {
                    ResetHover(canvas);
                    TryToHover(canvas, x, y);
                    _ellipse.BottomRight.X = x;
                    _ellipse.BottomRight.Y = y;
                    canvas.Invalidate();
                }
            }
                break;
        }
    }

    public override ToolBaseViewModel CloneSelf(Dictionary<ViewModelBase, ViewModelBase> shared)
    {
        if (shared.TryGetValue(this, out var value))
        {
            return value as EllipseToolViewModel;
        }

        var copy = new EllipseToolViewModel()
        {
            Brush = _brush?.CloneSelf(shared),
            Pen = _pen?.CloneSelf(shared),
            IsStroked = _isStroked,
            IsFilled = _isFilled
        };

        shared[this] = copy;
        return copy;
    }

    public override ViewModelBase Clone(Dictionary<ViewModelBase, ViewModelBase> shared)
    {
        return CloneSelf(shared);
    }
}