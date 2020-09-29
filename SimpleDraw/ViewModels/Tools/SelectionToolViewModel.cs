using System.Collections.Generic;
using System.Runtime.Serialization;
using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    [DataContract(IsReference = true)]
    public class SelectionToolViewModel : ToolBaseViewModel
    {
        private enum State { None, Selected, Pressed }
        private State _state = State.None;
        private double _hitRadius;
        private double _pressedX = double.NaN;
        private double _pressedY = double.NaN;
        private double _previousX = double.NaN;
        private double _previousY = double.NaN;
        private RectangleShapeViewModel _rectangle;

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public double HitRadius
        {
            get => _hitRadius;
            set => this.RaiseAndSetIfChanged(ref _hitRadius, value);
        }

        [IgnoreDataMember]
        public override string Name => "Selection";

        public SelectionToolViewModel()
        {
            _rectangle = new RectangleShapeViewModel()
            {
                TopLeft = new PointViewModel(0, 0),
                BottomRight = new PointViewModel(0, 0),
                IsStroked = true,
                IsFilled = true,
                RadiusX = 0,
                RadiusY = 0,
                Brush = new SolidColorBrushViewModel(new ColorViewModel(80, 0, 0, 255)),
                Pen = new PenViewModel(new SolidColorBrushViewModel(new ColorViewModel(160, 0, 0, 255)), 2)
            };
        }

        public override void Pressed(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers)
        {
            if (canvas.Selected == null)
            {
                return;
            }

            switch (_state)
            {
                case State.None:
                    {
                        var result = HitTest.Contains(canvas.Items, x, y, _hitRadius);
                        if (result != null)
                        {
                            if (keyModifiers.HasFlag(ToolKeyModifiers.Control))
                            {
                                if (canvas.Selected.Contains(result))
                                {
                                    canvas.Selected.Remove(result);
                                    canvas.Invalidate();
                                }
                                else
                                {
                                    canvas.Selected.Add(result);
                                    canvas.Invalidate();
                                }
                            }
                            else
                            {
                                if (!canvas.Selected.Contains(result))
                                {
                                    canvas.Selected.Clear();
                                    canvas.Selected.Add(result);
                                    canvas.Invalidate();
                                }
                            }
                            _previousX = x;
                            _previousY = y;
                            _state = State.Selected;
                        }
                        else
                        {
                            if (!keyModifiers.HasFlag(ToolKeyModifiers.Control))
                            {
                                canvas.Selected.Clear();
                            }
                            canvas.Decorators.Add(_rectangle);
                            _rectangle.TopLeft.X = x;
                            _rectangle.TopLeft.Y = y;
                            _rectangle.BottomRight.X = x;
                            _rectangle.BottomRight.Y = y;
                            canvas.Invalidate();
                            _pressedX = x;
                            _pressedY = y;
                            _state = State.Pressed;
                        }
                    }
                    break;
                case State.Selected:
                    {
                        _state = State.None;
                    }
                    break;
                case State.Pressed:
                    {
                        _state = State.None;
                    }
                    break;
            }
        }

        public override void Released(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers)
        {
            if (canvas.Selected == null)
            {
                return;
            }

            switch (_state)
            {
                case State.None:
                    {
                    }
                    break;
                case State.Selected:
                    {
                        _state = State.None;
                    }
                    break;
                case State.Pressed:
                    {
                        canvas.Decorators.Remove(_rectangle);

                        var rect = HitTest.ToSKRect(_pressedX, _pressedY, x, y);
                        if (keyModifiers.HasFlag(ToolKeyModifiers.Control))
                        {
                            for (int i = canvas.Items.Count - 1; i >= 0; i--)
                            {
                                var shape = canvas.Items[i];
                                var result = HitTest.Intersects(shape, rect);
                                if (result != null)
                                {
                                    if (canvas.Selected.Contains(result))
                                    {
                                        canvas.Selected.Remove(result);
                                        canvas.Invalidate();
                                    }
                                    else
                                    {
                                        canvas.Selected.Add(result);
                                        canvas.Invalidate();
                                    }
                                }
                            }
                        }
                        else
                        {
                            canvas.Selected.Clear();

                            for (int i = canvas.Items.Count - 1; i >= 0; i--)
                            {
                                var shape = canvas.Items[i];
                                var result = HitTest.Intersects(shape, rect);
                                if (result != null)
                                {
                                    canvas.Selected.Add(result);
                                }
                            }

                            canvas.Invalidate();
                        }
                        _state = State.None;
                    }
                    break;
            }
        }

        public override void Moved(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers)
        {
            if (canvas.Selected == null)
            {
                return;
            }

            switch (_state)
            {
                case State.None:
                    {
                    }
                    break;
                case State.Selected:
                    {
                        double deltaX = x - _previousX;
                        double deltaY = y - _previousY;

                        Move(canvas, deltaX, deltaY);
                        canvas.Invalidate();

                        _previousX = x;
                        _previousY = y;
                    }
                    break;
                case State.Pressed:
                    {
                        _rectangle.BottomRight.X = x;
                        _rectangle.BottomRight.Y = y;
                        canvas.Invalidate();
                    }
                    break;
            }
        }

        private void Move(CanvasViewModel canvas, double deltaX, double deltaY)
        {
            var points = new HashSet<PointViewModel>();

            foreach (var item in canvas.Selected)
            {
                switch (item)
                {
                    case PointViewModel point:
                        {
                            points.Add(point);
                            points.Add(point);
                        }
                        break;
                    case LineShapeViewModel lineShape:
                        {
                            points.Add(lineShape.Start);
                            points.Add(lineShape.End);
                        }
                        break;
                    case RectangleShapeViewModel rectangleShape:
                        {
                            points.Add(rectangleShape.TopLeft);
                            points.Add(rectangleShape.BottomRight);
                        }
                        break;
                }
            }

            foreach (var point in points)
            {
                point.X += deltaX;
                point.Y += deltaY;
            }

            canvas.Invalidate();
        }

        public override ToolBaseViewModel CloneSelf(Dictionary<ViewModelBase, ViewModelBase> shared)
        {
            if (shared.TryGetValue(this, out var value))
            {
                return value as SelectionToolViewModel;
            }

            var copy = new SelectionToolViewModel()
            {
                HitRadius = _hitRadius
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
