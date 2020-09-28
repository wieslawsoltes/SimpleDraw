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
        private ObservableCollection<ViewModelBase> _selected;
        private double _pressedX = double.NaN;
        private double _pressedY = double.NaN;
        private double _previousX = double.NaN;
        private double _previousY = double.NaN;

        public double HitRadius
        {
            get => _hitRadius;
            set => this.RaiseAndSetIfChanged(ref _hitRadius, value);
        }

        public ObservableCollection<ViewModelBase> Selected
        {
            get => _selected;
            set => this.RaiseAndSetIfChanged(ref _selected, value);
        }

        public override string Name => "Selection";

        public override void Pressed(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers)
        {
            if (_selected == null)
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
                                if (_selected.Contains(result))
                                {
                                    _selected.Remove(result);
                                }
                                else
                                {
                                    _selected.Add(result);
                                }
                            }
                            else
                            {
                                if (!_selected.Contains(result))
                                {
                                    _selected.Clear();
                                    _selected.Add(result);
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
                                _selected.Clear();
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
            if (_selected == null)
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
                                    if (_selected.Contains(result))
                                    {
                                        _selected.Remove(result);
                                    }
                                    else
                                    {
                                        _selected.Add(result);
                                    }
                                }
                            }
                        }
                        else
                        {
                            _selected.Clear();

                            foreach (var shape in canvas.Items)
                            {
                                var result = HitTest.Intersects(shape, rect);
                                if (result != null)
                                {
                                    _selected.Add(result);
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
            if (_selected == null)
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

                        foreach (var item in _selected)
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

            var selected = new ObservableCollection<ViewModelBase>();

            foreach (var item in _selected)
            {
                selected.Add(item.Clone(shared));
            }

            var copy = new SelectionToolViewModel()
            {
                HitRadius = _hitRadius,
                Selected = selected
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
