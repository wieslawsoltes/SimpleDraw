using System.Collections.Generic;
using System.Runtime.Serialization;
using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    [DataContract(IsReference = true)]
    public class QuadraticBezierToolViewModel : ToolBaseViewModel
    {
        private enum State { None, EndPoint, Control }
        private State _state = State.None;
        private QuadraticBezierShapeViewModel _quadraticBezier;
        private BrushViewModel _brush;
        private PenViewModel _pen;
        private double _hitRadius;
        private bool _tryToConnect;
        private bool _isStroked;
        private bool _isFilled;

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public BrushViewModel Brush
        {
            get => _brush;
            set => this.RaiseAndSetIfChanged(ref _brush, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public PenViewModel Pen
        {
            get => _pen;
            set => this.RaiseAndSetIfChanged(ref _pen, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool IsStroked
        {
            get => _isStroked;
            set => this.RaiseAndSetIfChanged(ref _isStroked, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool IsFilled
        {
            get => _isFilled;
            set => this.RaiseAndSetIfChanged(ref _isFilled, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public double HitRadius
        {
            get => _hitRadius;
            set => this.RaiseAndSetIfChanged(ref _hitRadius, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool TryToConnect
        {
            get => _tryToConnect;
            set => this.RaiseAndSetIfChanged(ref _tryToConnect, value);
        }

        [IgnoreDataMember]
        public override string Name => "QuadraticBezier";

        public override void Pressed(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers)
        {
            switch (_state)
            {
                case State.None:
                    {
                        if (pointerType == ToolPointerType.Left)
                        {
                            var shared = new Dictionary<ViewModelBase, ViewModelBase>();
                            var topLeft = default(PointViewModel);

                            if (_tryToConnect)
                            {
                                var result = HitTest.Contains(canvas.Items, x, y, _hitRadius);
                                if (result is PointViewModel point)
                                {
                                    topLeft = point;
                                }
                            }

                            _quadraticBezier = new QuadraticBezierShapeViewModel()
                            {
                                StartPoint = topLeft ?? new PointViewModel(x, y),
                                Control = new PointViewModel(x, y),
                                EndPoint = new PointViewModel(x, y),
                                IsStroked = _isStroked,
                                IsFilled = _isFilled,
                                Brush = _brush.CloneSelf(shared),
                                Pen = _pen.CloneSelf(shared)
                            };
                            canvas.Decorators.Add(_quadraticBezier);
                            canvas.Invalidate();
                            _state = State.EndPoint;
                        }
                    }
                    break;
                case State.EndPoint:
                    {
                        if (pointerType == ToolPointerType.Left)
                        {
                            var endPoint = default(PointViewModel);

                            if (_tryToConnect)
                            {
                                var result = HitTest.Contains(canvas.Items, x, y, _hitRadius);
                                if (result is PointViewModel point)
                                {
                                    endPoint = point;
                                }
                            }

                            if (endPoint != null)
                            {
                                _quadraticBezier.EndPoint = endPoint;
                            }

                            canvas.Invalidate();

                            _state = State.Control;
                        }

                        if (pointerType == ToolPointerType.Right)
                        {
                            canvas.Decorators.Remove(_quadraticBezier);
                            canvas.Invalidate();
                            _quadraticBezier = null;
                            _state = State.None;
                        }
                    }
                    break;
                case State.Control:
                    {
                        if (pointerType == ToolPointerType.Left)
                        {
                            var control = default(PointViewModel);

                            if (_tryToConnect)
                            {
                                var result = HitTest.Contains(canvas.Items, x, y, _hitRadius);
                                if (result is PointViewModel point)
                                {
                                    control = point;
                                }
                            }

                            if (control != null)
                            {
                                _quadraticBezier.Control = control;
                            }

                            canvas.Decorators.Remove(_quadraticBezier);
                            canvas.Items.Add(_quadraticBezier);
                            canvas.Invalidate();

                            _quadraticBezier = null;
                            _state = State.None;
                        }

                        if (pointerType == ToolPointerType.Right)
                        {
                            canvas.Decorators.Remove(_quadraticBezier);
                            canvas.Invalidate();
                            _quadraticBezier = null;
                            _state = State.None;
                        }
                    }
                    break;
            }
        }

        public override void Released(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers)
        {
            switch (_state)
            {
                case State.None:
                    {
                    }
                    break;
                case State.EndPoint:
                    {
                    }
                    break;
                case State.Control:
                    {
                    }
                    break;
            }
        }

        public override void Moved(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers)
        {
            switch (_state)
            {
                case State.None:
                    {
                    }
                    break;
                case State.EndPoint:
                    {
                        if (pointerType == ToolPointerType.None)
                        {
                            _quadraticBezier.Control.X = x;
                            _quadraticBezier.Control.Y = y;
                            _quadraticBezier.EndPoint.X = x;
                            _quadraticBezier.EndPoint.Y = y;
                            canvas.Invalidate();
                        }
                    }
                    break;
                case State.Control:
                    {
                        if (pointerType == ToolPointerType.None)
                        {
                            _quadraticBezier.Control.X = x;
                            _quadraticBezier.Control.Y = y;
                            canvas.Invalidate();
                        }
                    }
                    break;
            }
        }

        public override ToolBaseViewModel CloneSelf(Dictionary<ViewModelBase, ViewModelBase> shared)
        {
            if (shared.TryGetValue(this, out var value))
            {
                return value as QuadraticBezierToolViewModel;
            }

            var copy = new QuadraticBezierToolViewModel()
            {
                Brush = _brush?.CloneSelf(shared),
                Pen = _pen?.CloneSelf(shared),
                IsStroked = _isStroked,
                IsFilled = _isFilled
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
