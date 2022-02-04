using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ReactiveUI;

namespace SimpleDraw.ViewModels.Media;

[DataContract(IsReference = true)]
public abstract class GradientBrushViewModel : BrushViewModel
{
    protected ObservableCollection<GradientStopViewModel> _gradientStops;
    protected GradientSpreadMethod _spreadMethod;

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public ObservableCollection<GradientStopViewModel> GradientStops
    {
        get => _gradientStops;
        set => this.RaiseAndSetIfChanged(ref _gradientStops, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public GradientSpreadMethod SpreadMethod
    {
        get => _spreadMethod;
        set => this.RaiseAndSetIfChanged(ref _spreadMethod, value);
    }

    protected GradientBrushViewModel()
    {
    }

    protected GradientBrushViewModel(ObservableCollection<GradientStopViewModel> gradientStops, GradientSpreadMethod spreadMethod)
    {
        _gradientStops = gradientStops;
        _spreadMethod = spreadMethod;
    }
}