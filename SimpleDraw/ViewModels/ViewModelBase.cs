using System.Collections.Generic;
using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    public abstract class ViewModelBase : ReactiveObject
    {
        public abstract ViewModelBase Clone(Dictionary<ViewModelBase, ViewModelBase> shared);
    }
}
