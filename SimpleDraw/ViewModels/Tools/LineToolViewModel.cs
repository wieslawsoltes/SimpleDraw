using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    public class LineToolViewModel : ToolBaseViewModel
    {
        private enum State { None, Pressed }
        private State _state = State.None;
        private LineShapeViewModel _line = null;
        private PenViewModel _pen;
        private bool _isStroked;

        public PenViewModel Pen
        {
            get => _pen;
            set => this.RaiseAndSetIfChanged(ref _pen, value);
        }

        public bool IsStroked
        {
            get => _isStroked;
            set => this.RaiseAndSetIfChanged(ref _isStroked, value);
        }

        public override string Name => "Line";

        public override void Pressed(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers)
        {
            switch (_state)
            {
                case State.None:
                    {
                        if (pointerType == ToolPointerType.Left)
                        {
                            _line = new LineShapeViewModel()
                            {
                                Start = new PointViewModel(x, y),
                                End = new PointViewModel(x, y),
                                IsStroked = _isStroked,
                                Pen = _pen
                            };
                            canvas.Shapes.Add(_line);
                            _state = State.Pressed;
                        }
                    }
                    break;
                case State.Pressed:
                    {
                        if (pointerType == ToolPointerType.Left)
                        {
                            _line = null;
                            _state = State.None;
                        }

                        if (pointerType == ToolPointerType.Right)
                        {
                            canvas.Shapes.Remove(_line);
                            _line = null;
                            _state = State.None;
                        }
                    }
                    break;
            }
        }

        public override void Released(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers)
        {
        }

        public override void Moved(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers)
        {
            switch (_state)
            {
                case State.None:
                    {
                    }
                    break;
                case State.Pressed:
                    {
                        if (pointerType == ToolPointerType.None)
                        {
                            _line.End.X = x;
                            _line.End.Y = y;
                        }
                    }
                    break;
            }
        }
    }
}
