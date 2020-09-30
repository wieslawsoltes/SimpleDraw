using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    public enum PathToolMode
    {
        Move = 0,
        Line = 1,
        CubicBezier = 2,
        QuadraticBezier
    }

    [DataContract(IsReference = true)]
    public class PathToolViewModel : ToolBaseViewModel
    {
        private enum State { None, Pressed }
        private State _state = State.None;
        private PathShapeViewModel _path;
        private FigureViewModel _figure;
        private BrushViewModel _brush;
        private PenViewModel _pen;
        private double _hitRadius;
        private bool _tryToConnect;
        private bool _isStroked;
        private bool _isFilled;
        private PathFillRule _fillRule;
        private bool _isClosed;
        private PathToolMode _mode;

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
        public PathFillRule FillRule
        {
            get => _fillRule;
            set => this.RaiseAndSetIfChanged(ref _fillRule, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool IsClosed
        {
            get => _isClosed;
            set => this.RaiseAndSetIfChanged(ref _isClosed, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public PathToolMode Mode
        {
            get => _mode;
            set => this.RaiseAndSetIfChanged(ref _mode, value);
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
        public override string Name => "Path";

        public override void Pressed(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers)
        {
            switch (_state)
            {
                case State.None:
                    {
                        if (pointerType == ToolPointerType.Left)
                        {
                            var shared = new Dictionary<ViewModelBase, ViewModelBase>();
                            var startPoint = default(PointViewModel);

                            if (_tryToConnect)
                            {
                                var result = HitTest.Contains(canvas.Items, x, y, _hitRadius);
                                if (result is PointViewModel point)
                                {
                                    startPoint = point;
                                }
                            }

                            _path = new PathShapeViewModel()
                            {
                                Figures = new ObservableCollection<FigureViewModel>(),
                                IsStroked = _isStroked,
                                IsFilled = _isFilled,
                                FillRule = _fillRule,
                                Brush = _brush.CloneSelf(shared),
                                Pen = _pen.CloneSelf(shared)
                            };

                            _figure = new FigureViewModel()
                            {
                                Segments = new ObservableCollection<ViewModelBase>(),
                                IsClosed = _isClosed
                            };

                            _path.Figures.Add(_figure);

                            switch (_mode)
                            {
                                case PathToolMode.Move:
                                    {
                                        // TODO: startPoint
                                    }
                                    break;
                                case PathToolMode.Line:
                                    {
                                        // TODO: startPoint
                                    }
                                    break;
                                case PathToolMode.CubicBezier:
                                    {
                                        // TODO: startPoint
                                    }
                                    break;
                                case PathToolMode.QuadraticBezier:
                                    {
                                        // TODO: startPoint
                                    }
                                    break;
                            }

                            canvas.Decorators.Add(_path);
                            canvas.Invalidate();
                            _state = State.Pressed;
                        }
                    }
                    break;
                case State.Pressed:
                    {
                        if (pointerType == ToolPointerType.Left)
                        {
                            var nextPoint = default(PointViewModel);

                            if (_tryToConnect)
                            {
                                var result = HitTest.Contains(canvas.Items, x, y, _hitRadius);
                                if (result is PointViewModel point)
                                {
                                    nextPoint = point;
                                }
                            }

                            if (nextPoint != null)
                            {
                                // TODO: nextPoint
                            }

                            canvas.Decorators.Remove(_path);
                            canvas.Items.Add(_path);
                            canvas.Invalidate();

                            _path = null;
                            _figure = null;
                            _state = State.None;
                        }

                        if (pointerType == ToolPointerType.Right)
                        {
                            canvas.Decorators.Remove(_path);
                            canvas.Invalidate();
                            _path = null;
                            _figure = null;
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
                case State.Pressed:
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
                case State.Pressed:
                    {
                        if (pointerType == ToolPointerType.None)
                        {
                            // TODO:
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
                return value as PathToolViewModel;
            }

            var copy = new PathToolViewModel()
            {
                Brush = _brush?.CloneSelf(shared),
                Pen = _pen?.CloneSelf(shared),
                IsStroked = _isStroked,
                IsFilled = _isFilled,
                FillRule = _fillRule
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
