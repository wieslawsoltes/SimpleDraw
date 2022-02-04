using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SimpleDraw.ViewModels.Media;

[DataContract(IsReference = true)]
public abstract class BrushViewModel : ViewModelBase
{
    public abstract BrushViewModel CloneSelf(Dictionary<ViewModelBase, ViewModelBase> shared);
}