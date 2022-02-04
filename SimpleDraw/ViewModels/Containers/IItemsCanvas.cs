﻿using System.Collections.ObjectModel;

namespace SimpleDraw.ViewModels.Containers;

public interface IItemsCanvas
{
    ObservableCollection<ViewModelBase> Items { get; set; }
    ObservableCollection<ViewModelBase> Hovered { get; set; }
    ObservableCollection<ViewModelBase> Selected { get; set; }
    ObservableCollection<ViewModelBase> Decorators { get; set; }
    void Invalidate();
}