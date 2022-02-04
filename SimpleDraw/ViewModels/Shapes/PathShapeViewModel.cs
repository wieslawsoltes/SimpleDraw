using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ReactiveUI;
using SimpleDraw.ViewModels.Containers;

namespace SimpleDraw.ViewModels.Shapes;

public enum FillRule
{
    EvenOdd = 0,
    NonZero = 1
}

[DataContract(IsReference = true)]
public class PathShapeViewModel : ShapeBaseViewModel
{
    private ObservableCollection<FigureViewModel> _figures;
    private bool _isStroked;
    private bool _isFilled;
    private FillRule _fillRule;

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public ObservableCollection<FigureViewModel> Figures
    {
        get => _figures;
        set => this.RaiseAndSetIfChanged(ref _figures, value);
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
    public FillRule FillRule
    {
        get => _fillRule;
        set => this.RaiseAndSetIfChanged(ref _fillRule, value);
    }

    public override ShapeBaseViewModel CloneSelf(Dictionary<ViewModelBase, ViewModelBase> shared)
    {
        if (shared.TryGetValue(this, out var value))
        {
            return value as PathShapeViewModel;
        }

        var figures = new ObservableCollection<FigureViewModel>();

        foreach (var item in _figures)
        {
            figures.Add(item.CloneSelf(shared));
        }

        var copy = new PathShapeViewModel()
        {
            Brush = _brush?.CloneSelf(shared),
            Pen = _pen?.CloneSelf(shared),
            Figures = figures,
            IsStroked = _isStroked,
            IsFilled = _isFilled,
            FillRule = _fillRule
        };

        shared[this] = copy;
        return copy;
    }

    public override ViewModelBase Clone(Dictionary<ViewModelBase, ViewModelBase> shared)
    {
        return CloneSelf(shared);
    }
}