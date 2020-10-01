using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    internal class ItemsCanvasAdapter : IItemsCanvas
    {
        public CanvasViewModel Canvas { get; set; }

        public PathShapeViewModel Path { get; set; }

        public FigureViewModel Figure { get; set; }

        public ObservableCollection<ViewModelBase> Items { get; set; }

        public ObservableCollection<ViewModelBase> Decorators { get; set; }

        public void Invalidate()
        {
            Canvas?.Invalidate();
        }
    }

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
        private enum PathState { StartPoint, NextPoint }
        private PathState _state = PathState.StartPoint;
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
        private PathToolMode _previousMode;
        private PathToolMode _mode;
        private ItemsCanvasAdapter _itemsCanvasAdapter;
        private ShapeToolViewModel _lineShapeTool;
        private ShapeToolViewModel _cubicBezierShapeTool;
        private ShapeToolViewModel _quadraticBezierShapeTool;

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
        public PathToolMode PreviousMode
        {
            get => _previousMode;
            set => this.RaiseAndSetIfChanged(ref _previousMode, value);
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

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public ShapeToolViewModel LineShapeTool
        {
            get => _lineShapeTool;
            set => this.RaiseAndSetIfChanged(ref _lineShapeTool, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public ShapeToolViewModel CubicBezierShapeTool
        {
            get => _cubicBezierShapeTool;
            set => this.RaiseAndSetIfChanged(ref _cubicBezierShapeTool, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public ShapeToolViewModel QuadraticBezierShapeTool
        {
            get => _quadraticBezierShapeTool;
            set => this.RaiseAndSetIfChanged(ref _quadraticBezierShapeTool, value);
        }

        [IgnoreDataMember]
        public override string Name => "Path";

        public PathToolViewModel()
        {
            _itemsCanvasAdapter = new ItemsCanvasAdapter();
        }

        public override void Pressed(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers)
        {
            switch (_state)
            {
                case PathState.StartPoint:
                    {
                        if (pointerType == ToolPointerType.Left)
                        {
                            var shared = new Dictionary<ViewModelBase, ViewModelBase>();

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

                            _itemsCanvasAdapter.Canvas = canvas;
                            _itemsCanvasAdapter.Path = _path;
                            _itemsCanvasAdapter.Figure = _figure;
                            _itemsCanvasAdapter.Items = _figure.Segments;
                            _itemsCanvasAdapter.Decorators = _figure.Segments;

                            switch (_mode)
                            {
                                case PathToolMode.Line:
                                    {
                                        _lineShapeTool?.Pressed(_itemsCanvasAdapter, x, y, pointerType, keyModifiers);
                                    }
                                    break;
                                case PathToolMode.CubicBezier:
                                    {
                                        _cubicBezierShapeTool?.Pressed(_itemsCanvasAdapter, x, y, pointerType, keyModifiers);
                                    }
                                    break;
                                case PathToolMode.QuadraticBezier:
                                    {
                                        _quadraticBezierShapeTool?.Pressed(_itemsCanvasAdapter, x, y, pointerType, keyModifiers);
                                    }
                                    break;
                            }

                            canvas.Decorators.Add(_path);
                            canvas.Invalidate();
                            _state = PathState.NextPoint;
                        }
                    }
                    break;
                case PathState.NextPoint:
                    {
                        if (pointerType == ToolPointerType.Left)
                        {
                            _itemsCanvasAdapter.Canvas = canvas;
                            _itemsCanvasAdapter.Path = _path;
                            _itemsCanvasAdapter.Figure = _figure;
                            _itemsCanvasAdapter.Items = _figure.Segments;
                            _itemsCanvasAdapter.Decorators = _figure.Segments;

                            if (_mode == PathToolMode.Move)
                            {
                                _figure = new FigureViewModel()
                                {
                                    Segments = new ObservableCollection<ViewModelBase>(),
                                    IsClosed = _isClosed
                                };

                                _path.Figures.Add(_figure);

                                _mode = _previousMode;
                            }

                            switch (_mode)
                            {
                                case PathToolMode.Line:
                                    {
                                        _lineShapeTool?.Pressed(_itemsCanvasAdapter, x, y, pointerType, keyModifiers);
                                        _lineShapeTool?.Pressed(_itemsCanvasAdapter, x, y, pointerType, keyModifiers);
                                    }
                                    break;
                                case PathToolMode.CubicBezier:
                                    {
                                        _cubicBezierShapeTool?.Pressed(_itemsCanvasAdapter, x, y, pointerType, keyModifiers);
                                        _cubicBezierShapeTool?.Pressed(_itemsCanvasAdapter, x, y, pointerType, keyModifiers);
                                    }
                                    break;
                                case PathToolMode.QuadraticBezier:
                                    {
                                        _quadraticBezierShapeTool?.Pressed(_itemsCanvasAdapter, x, y, pointerType, keyModifiers);
                                        _quadraticBezierShapeTool?.Pressed(_itemsCanvasAdapter, x, y, pointerType, keyModifiers);
                                    }
                                    break;
                            }
                        }

                        if (pointerType == ToolPointerType.Right)
                        {
                            switch (_mode)
                            {
                                case PathToolMode.Line:
                                    {
                                        _lineShapeTool?.Pressed(_itemsCanvasAdapter, x, y, pointerType, keyModifiers);
                                    }
                                    break;
                                case PathToolMode.CubicBezier:
                                    {
                                        _cubicBezierShapeTool?.Pressed(_itemsCanvasAdapter, x, y, pointerType, keyModifiers);
                                    }
                                    break;
                                case PathToolMode.QuadraticBezier:
                                    {
                                        _quadraticBezierShapeTool?.Pressed(_itemsCanvasAdapter, x, y, pointerType, keyModifiers);
                                    }
                                    break;
                            }

                            canvas.Decorators.Remove(_path);

                            if (_path.Figures[0].Segments.Count >= 0)
                            {
                                canvas.Items.Add(_path);
                            }
                            canvas.Invalidate();
                            _path = null;
                            _figure = null;
                            _state = PathState.StartPoint;
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
                case PathState.NextPoint:
                    {
                        if (pointerType == ToolPointerType.None)
                        {
                            _itemsCanvasAdapter.Canvas = canvas;
                            _itemsCanvasAdapter.Path = _path;
                            _itemsCanvasAdapter.Figure = _figure;
                            _itemsCanvasAdapter.Items = _figure.Segments;
                            _itemsCanvasAdapter.Decorators = _figure.Segments;

                            switch (_mode)
                            {
                                case PathToolMode.Line:
                                    {
                                        _lineShapeTool?.Moved(_itemsCanvasAdapter, x, y, pointerType, keyModifiers);
                                    }
                                    break;
                                case PathToolMode.CubicBezier:
                                    {
                                        _cubicBezierShapeTool?.Moved(_itemsCanvasAdapter, x, y, pointerType, keyModifiers);
                                    }
                                    break;
                                case PathToolMode.QuadraticBezier:
                                    {
                                        _quadraticBezierShapeTool?.Moved(_itemsCanvasAdapter, x, y, pointerType, keyModifiers);
                                    }
                                    break;
                            }
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
