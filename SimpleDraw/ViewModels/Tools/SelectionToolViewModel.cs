using System.Collections.Generic;
using System.Collections.ObjectModel;
using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    public class SelectionToolViewModel : ToolBaseViewModel
    {
        private enum State { None, Selected, Pressed }
        private State _state = State.None;
        private double _hitRadius;
        private double _pressedX = double.NaN;
        private double _pressedY = double.NaN;
        private double _previousX = double.NaN;
        private double _previousY = double.NaN;

        public double HitRadius
        {
            get => _hitRadius;
            set => this.RaiseAndSetIfChanged(ref _hitRadius, value);
        }

        public override string Name => "Selection";

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
                                }
                                else
                                {
                                    canvas.Selected.Add(result);
                                }
                            }
                            else
                            {
                                if (!canvas.Selected.Contains(result))
                                {
                                    canvas.Selected.Clear();
                                    canvas.Selected.Add(result);
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
                        var rect = HitTest.ToSKRect(_pressedX, _pressedY, x, y);
                        if (keyModifiers.HasFlag(ToolKeyModifiers.Control))
                        {
                            foreach (var shape in canvas.Items)
                            {
                                var result = HitTest.Intersects(shape, rect);
                                if (result != null)
                                {
                                    if (canvas.Selected.Contains(result))
                                    {
                                        canvas.Selected.Remove(result);
                                    }
                                    else
                                    {
                                        canvas.Selected.Add(result);
                                    }
                                }
                            }
                        }
                        else
                        {
                            canvas.Selected.Clear();

                            foreach (var shape in canvas.Items)
                            {
                                var result = HitTest.Intersects(shape, rect);
                                if (result != null)
                                {
                                    canvas.Selected.Add(result);
                                }
                            }
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

                        foreach (var item in canvas.Selected)
                        {
                            switch (item)
                            {
                                case PointViewModel point:
                                    {
                                        point.X += deltaX;
                                        point.Y += deltaY;
                                    }
                                    break;
                                case LineShapeViewModel lineShape:
                                    {
                                        lineShape.Start.X += deltaX;
                                        lineShape.Start.Y += deltaY;
                                        lineShape.End.X += deltaX;
                                        lineShape.End.Y += deltaY;
                                    }
                                    break;
                                case RectangleShapeViewModel rectangleShape:
                                    {
                                        rectangleShape.TopLeft.X += deltaX;
                                        rectangleShape.TopLeft.Y += deltaY;
                                        rectangleShape.BottomRight.X += deltaX;
                                        rectangleShape.BottomRight.Y += deltaY;
                                    }
                                    break;
                            }
                        }

                        _previousX = x;
                        _previousY = y;
                    }
                    break;
                case State.Pressed:
                    {
                        // TODO:
                    }
                    break;
            }
        }

        public override ToolBaseViewModel Copy(Dictionary<ViewModelBase, ViewModelBase> shared)
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
            return Copy(shared);
        }
    }
}
