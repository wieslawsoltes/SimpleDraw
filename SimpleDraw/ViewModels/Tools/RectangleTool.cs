using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    public class RectangleTool : ToolBase
    {
        private enum State { None, Pressed }
        private State _state = State.None;
        private RectangleShapeViewModel _rectangle;
        private BrushViewModel _brush;
        private PenViewModel _pen;
        private bool _isStroked;
        private bool _isFilled;
        private double _radiusX;
        private double _radiusY;

        public BrushViewModel Brush
        {
            get => _brush;
            set => this.RaiseAndSetIfChanged(ref _brush, value);
        }

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

        public bool IsFilled
        {
            get => _isFilled;
            set => this.RaiseAndSetIfChanged(ref _isFilled, value);
        }

        public double RadiusX
        {
            get => _radiusX;
            set => this.RaiseAndSetIfChanged(ref _radiusX, value);
        }

        public double RadiusY
        {
            get => _radiusY;
            set => this.RaiseAndSetIfChanged(ref _radiusY, value);
        }

        public override string Name => "Rectangle";

        public override void Pressed(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType)
        {
            switch (_state)
            {
                case State.None:
                    {
                        if (pointerType == ToolPointerType.Left)
                        {
                            _rectangle = new RectangleShapeViewModel()
                            {
                                TopLeft = new PointShapeViewModel(x, y),
                                BottomRight = new PointShapeViewModel(x, y),
                                IsStroked = _isStroked,
                                IsFilled = _isFilled,
                                RadiusX = _radiusX,
                                RadiusY = _radiusY,
                                Brush = _brush,
                                Pen = _pen
                            };
                            canvas.Shapes.Add(_rectangle);
                            _state = State.Pressed;
                        }
                    }
                    break;
                case State.Pressed:
                    {
                        if (pointerType == ToolPointerType.Left)
                        {
                            _rectangle = null;
                            _state = State.None;
                        }

                        if (pointerType == ToolPointerType.Right)
                        {
                            canvas.Shapes.Remove(_rectangle);
                            _rectangle = null;
                            _state = State.None;
                        }
                    }
                    break;
            }
        }

        public override void Released(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType)
        {
            switch (_state)
            {
                case State.None:
                    {
                    }
                    break;
                case State.Pressed:
                    {
                    }
                    break;
            }
        }

        public override void Moved(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType)
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
                            _rectangle.BottomRight.X = x;
                            _rectangle.BottomRight.Y = y;
                        }
                    }
                    break;
            }
        }
    }
}
