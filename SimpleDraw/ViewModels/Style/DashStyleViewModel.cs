using System.Collections.ObjectModel;
using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    public class DashStyleViewModel : ViewModelBase
    {
        private ObservableCollection<double> _dashes;
        private double _offset;

        public ObservableCollection<double> Dashes
        {
            get => _dashes;
            set => this.RaiseAndSetIfChanged(ref _dashes, value);
        }

        public double Offset
        {
            get => _offset;
            set => this.RaiseAndSetIfChanged(ref _offset, value);
        }

        public DashStyleViewModel()
        {
        }

        public DashStyleViewModel(ObservableCollection<double> dashes, double offset)
        {
            _dashes = dashes;
            _offset = offset;
        }
    }
}
