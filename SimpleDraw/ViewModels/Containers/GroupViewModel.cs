using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ReactiveUI;

namespace SimpleDraw.ViewModels.Containers;

[DataContract(IsReference = true)]
public class GroupViewModel : ViewModelBase
{
    private ObservableCollection<ViewModelBase> _items;

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public ObservableCollection<ViewModelBase> Items
    {
        get => _items;
        set => this.RaiseAndSetIfChanged(ref _items, value);
    }

    public GroupViewModel CloneSelf(Dictionary<ViewModelBase, ViewModelBase> shared)
    {
        if (shared.TryGetValue(this, out var value))
        {
            return value as GroupViewModel;
        }

        var items = new ObservableCollection<ViewModelBase>();

        foreach (var item in _items)
        {
            items.Add(item.Clone(shared));
        }

        var copy = new GroupViewModel()
        {
            Items = items
        };

        shared[this] = copy;
        return copy;
    }

    public override ViewModelBase Clone(Dictionary<ViewModelBase, ViewModelBase> shared)
    {
        return CloneSelf(shared);
    }
}