using System.Collections.Generic;
using System.Runtime.Serialization;
using ReactiveUI;

namespace SimpleDraw.ViewModels.Primitives;

[DataContract(IsReference = true)]
public class PointViewModel : ViewModelBase
{
    private double _x;
    private double _y;

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public double X
    {
        get => _x;
        set => this.RaiseAndSetIfChanged(ref _x, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public double Y
    {
        get => _y;
        set => this.RaiseAndSetIfChanged(ref _y, value);
    }

    public PointViewModel()
    {
    }

    public PointViewModel(double x, double y)
    {
        _x = x;
        _y = y;
    }

    public PointViewModel CloneSelf(Dictionary<ViewModelBase, ViewModelBase> shared)
    {
        if (shared.TryGetValue(this, out var value))
        {
            return value as PointViewModel;
        }

        var copy = new PointViewModel()
        {
            X = _x,
            Y = _y
        };

        shared[this] = copy;
        return copy;
    }

    public override ViewModelBase Clone(Dictionary<ViewModelBase, ViewModelBase> shared)
    {
        return CloneSelf(shared);
    }
}