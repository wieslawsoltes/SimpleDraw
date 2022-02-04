using System.Collections.Generic;
using System.Runtime.Serialization;
using ReactiveUI;

namespace SimpleDraw.ViewModels.Media;

[DataContract(IsReference = true)]
public class ColorViewModel : ViewModelBase
{
    private byte _a;
    private byte _r;
    private byte _g;
    private byte _b;

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public byte A
    {
        get => _a;
        set => this.RaiseAndSetIfChanged(ref _a, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public byte R
    {
        get => _r;
        set => this.RaiseAndSetIfChanged(ref _r, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public byte G
    {
        get => _g;
        set => this.RaiseAndSetIfChanged(ref _g, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public byte B
    {
        get => _b;
        set => this.RaiseAndSetIfChanged(ref _b, value);
    }

    public ColorViewModel()
    {
    }

    public ColorViewModel(byte a, byte r, byte g, byte b)
    {
        _a = a;
        _r = r;
        _g = g;
        _b = b;
    }

    public ColorViewModel CloneSelf(Dictionary<ViewModelBase, ViewModelBase> shared)
    {
        if (shared.TryGetValue(this, out var value))
        {
            return value as ColorViewModel;
        }

        var copy = new ColorViewModel()
        {
            A = _a,
            R = _r,
            G = _g,
            B = _b
        };

        shared[this] = copy;
        return copy;
    }

    public override ViewModelBase Clone(Dictionary<ViewModelBase, ViewModelBase> shared)
    {
        return CloneSelf(shared);
    }
}