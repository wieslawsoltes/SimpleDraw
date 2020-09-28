using System.Collections.Generic;

namespace SimpleDraw.ViewModels
{
    public abstract class BrushViewModel : ViewModelBase
    {
        public abstract BrushViewModel Copy(Dictionary<ViewModelBase, ViewModelBase> shared);
    }
}
