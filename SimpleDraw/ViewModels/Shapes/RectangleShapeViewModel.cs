using System.Collections.Generic;
using System.Runtime.Serialization;
using ReactiveUI;
using SimpleDraw.ViewModels.Primitives;

namespace SimpleDraw.ViewModels.Shapes;

[DataContract(IsReference = true)]
public class RectangleShapeViewModel : ShapeBaseViewModel
{
    private PointViewModel _topLeft;
    private PointViewModel _bottomRight;
    private bool _isStroked;
    private bool _isFilled;
    private double _radiusX;
    private double _radiusY;

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public PointViewModel TopLeft
    {
        get => _topLeft;
        set => this.RaiseAndSetIfChanged(ref _topLeft, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public PointViewModel BottomRight
    {
        get => _bottomRight;
        set => this.RaiseAndSetIfChanged(ref _bottomRight, value);
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
    public double RadiusX
    {
        get => _radiusX;
        set => this.RaiseAndSetIfChanged(ref _radiusX, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public double RadiusY
    {
        get => _radiusY;
        set => this.RaiseAndSetIfChanged(ref _radiusY, value);
    }

    public override ShapeBaseViewModel CloneSelf(Dictionary<ViewModelBase, ViewModelBase> shared)
    {
        if (shared.TryGetValue(this, out var value))
        {
            return value as RectangleShapeViewModel;
        }

        var copy = new RectangleShapeViewModel()
        {
            Brush = _brush?.CloneSelf(shared),
            Pen = _pen?.CloneSelf(shared),
            TopLeft = _topLeft?.CloneSelf(shared),
            BottomRight = _bottomRight?.CloneSelf(shared),
            IsStroked = _isStroked,
            IsFilled = _isFilled,
            RadiusX = _radiusX,
            RadiusY = _radiusY
        };

        shared[this] = copy;
        return copy;
    }

    public override ViewModelBase Clone(Dictionary<ViewModelBase, ViewModelBase> shared)
    {
        return CloneSelf(shared);
    }
}