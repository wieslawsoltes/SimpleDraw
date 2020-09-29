using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    [DataContract(IsReference = true)]
    public class FigureViewModel : ViewModelBase
    {
        private ObservableCollection<ViewModelBase> _segments;
        private bool _isClosed;

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public ObservableCollection<ViewModelBase> Segments
        {
            get => _segments;
            set => this.RaiseAndSetIfChanged(ref _segments, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool IsClosed
        {
            get => _isClosed;
            set => this.RaiseAndSetIfChanged(ref _isClosed, value);
        }

        public FigureViewModel CloneSelf(Dictionary<ViewModelBase, ViewModelBase> shared)
        {
            if (shared.TryGetValue(this, out var value))
            {
                return value as FigureViewModel;
            }

            var segments = new ObservableCollection<ViewModelBase>();

            foreach (var item in _segments)
            {
                segments.Add(item.Clone(shared));
            }

            var copy = new FigureViewModel()
            {
                Segments = segments,
                IsClosed = _isClosed
            };

            shared[this] = copy;
            return copy;
        }

        public override ViewModelBase Clone(Dictionary<ViewModelBase, ViewModelBase> shared)
        {
            return CloneSelf(shared);
        }
    }
}
