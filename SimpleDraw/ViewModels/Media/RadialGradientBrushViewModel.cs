using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ReactiveUI;

namespace SimpleDraw.ViewModels.Media;

[DataContract(IsReference = true)]
public class RadialGradientBrushViewModel : GradientBrushViewModel
{
    private RelativePointViewModel _center;
    private RelativePointViewModel _gradientOrigin;
    private double _radius;

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public RelativePointViewModel Center
    {
        get => _center;
        set => this.RaiseAndSetIfChanged(ref _center, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public RelativePointViewModel GradientOrigin
    {
        get => _gradientOrigin;
        set => this.RaiseAndSetIfChanged(ref _gradientOrigin, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public double Radius
    {
        get => _radius;
        set => this.RaiseAndSetIfChanged(ref _radius, value);
    }

    public RadialGradientBrushViewModel()
        : base()
    {
    }

    public RadialGradientBrushViewModel(ObservableCollection<GradientStopViewModel> gradientStops, GradientSpreadMethod spreadMethod = GradientSpreadMethod.Pad, RelativePointViewModel center = null, RelativePointViewModel gradientOrigin = null, double radius = 0.5)
        : base(gradientStops, spreadMethod)
    {
        _center = center;
        _gradientOrigin = gradientOrigin;
        _radius = radius;
    }

    public override BrushViewModel CloneSelf(Dictionary<ViewModelBase, ViewModelBase> shared)
    {
        if (shared.TryGetValue(this, out var value))
        {
            return value as RadialGradientBrushViewModel;
        }

        var gradientStops = new ObservableCollection<GradientStopViewModel>();

        foreach (var gradientStop in _gradientStops)
        {
            gradientStops.Add(gradientStop.CloneSelf(shared));
        }

        var copy = new RadialGradientBrushViewModel()
        {
            GradientStops = gradientStops,
            SpreadMethod = _spreadMethod,
            Center = _center?.CloneSelf(shared),
            GradientOrigin = _gradientOrigin?.CloneSelf(shared),
            Radius = _radius
        };

        shared[this] = copy;
        return copy;
    }

    public override ViewModelBase Clone(Dictionary<ViewModelBase, ViewModelBase> shared)
    {
        return CloneSelf(shared);
    }
}