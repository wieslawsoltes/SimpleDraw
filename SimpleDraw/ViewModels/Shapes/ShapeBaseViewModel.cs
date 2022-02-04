using System.Collections.Generic;
using System.Runtime.Serialization;
using ReactiveUI;
using SimpleDraw.ViewModels.Media;

namespace SimpleDraw.ViewModels.Shapes;

[DataContract(IsReference = true)]
public abstract class ShapeBaseViewModel : ViewModelBase
{
    protected BrushViewModel _brush;
    protected PenViewModel _pen;

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

    public abstract ShapeBaseViewModel CloneSelf(Dictionary<ViewModelBase, ViewModelBase> shared);
}