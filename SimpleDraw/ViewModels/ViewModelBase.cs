using System.Collections.Generic;
using System.Runtime.Serialization;
using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    [DataContract(IsReference = true)]
    public abstract class ViewModelBase : ReactiveObject
    {
        public abstract ViewModelBase Clone(Dictionary<ViewModelBase, ViewModelBase> shared);
    }
}
