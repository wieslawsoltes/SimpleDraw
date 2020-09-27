using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    public class ColorViewModel : ViewModelBase
    {
        private byte _a;
        private byte _r;
        private byte _g;
        private byte _b;

        public byte A
        {
            get => _a;
            set => this.RaiseAndSetIfChanged(ref _a, value);
        }

        public byte R
        {
            get => _r;
            set => this.RaiseAndSetIfChanged(ref _r, value);
        }

        public byte G
        {
            get => _g;
            set => this.RaiseAndSetIfChanged(ref _g, value);
        }

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
    }
}
