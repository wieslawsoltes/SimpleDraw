using System.Collections.Generic;
using System.Runtime.Serialization;
using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    [DataContract(IsReference = true)]
    public class QuadraticBezierShapeToolViewModel : ShapeToolViewModel
    {
        private enum QuadraticBezierState { StartPoint, Control, EndPoint }
        private QuadraticBezierState _state = QuadraticBezierState.StartPoint;
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

        public override void Pressed(IItemsCanvas canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers)
        {
            switch (_state)
            {
                case QuadraticBezierState.StartPoint:
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
                            _state = QuadraticBezierState.EndPoint;
                        }
                    }
                    break;
                case QuadraticBezierState.EndPoint:
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

                            _state = QuadraticBezierState.Control;
                        }

                        if (pointerType == ToolPointerType.Right)
                        {
                            canvas.Decorators.Remove(_quadraticBezier);
                            canvas.Invalidate();
                            _quadraticBezier = null;
                            _state = QuadraticBezierState.StartPoint;
                        }
                    }
                    break;
                case QuadraticBezierState.Control:
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
                            _state = QuadraticBezierState.StartPoint;
                        }

                        if (pointerType == ToolPointerType.Right)
                        {
                            canvas.Decorators.Remove(_quadraticBezier);
                            canvas.Invalidate();
                            _quadraticBezier = null;
                            _state = QuadraticBezierState.StartPoint;
                        }
                    }
                    break;
            }
        }

        public override void Released(IItemsCanvas canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers)
        {
        }

        public override void Moved(IItemsCanvas canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers)
        {
            switch (_state)
            {
                case QuadraticBezierState.StartPoint:
                    {
                    }
                    break;
                case QuadraticBezierState.EndPoint:
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
                case QuadraticBezierState.Control:
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

        public override ShapeToolViewModel CloneSelf(Dictionary<ViewModelBase, ViewModelBase> shared)
        {
            if (shared.TryGetValue(this, out var value))
            {
                return value as QuadraticBezierShapeToolViewModel;
            }

            var copy = new QuadraticBezierShapeToolViewModel()
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
