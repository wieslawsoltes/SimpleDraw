using System.Collections.Generic;
using System.Runtime.Serialization;
using ReactiveUI;

namespace SimpleDraw.ViewModels.Media;

[DataContract(IsReference = true)]
public class SolidColorBrushViewModel : BrushViewModel
{
    private ColorViewModel _color;

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public ColorViewModel Color
    {
        get => _color;
        set => this.RaiseAndSetIfChanged(ref _color, value);
    }

    public SolidColorBrushViewModel()
    {
    }

    public SolidColorBrushViewModel(ColorViewModel color)
    {
        _color = color;
    }

    public override BrushViewModel CloneSelf(Dictionary<ViewModelBase, ViewModelBase> shared)
    {
        if (shared.TryGetValue(this, out var value))
        {
            return value as SolidColorBrushViewModel;
        }

        var copy = new SolidColorBrushViewModel()
        {
            Color = _color?.CloneSelf(shared)
        };

        shared[this] = copy;
        return copy;
    }

    public override ViewModelBase Clone(Dictionary<ViewModelBase, ViewModelBase> shared)
    {
        return CloneSelf(shared);
    }
}