﻿using System.Collections.Generic;
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
        private LineShapeToolViewModel _lineShapeTool;

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
        public LineShapeToolViewModel LineShapeTool
        {
            get => _lineShapeTool;
            set => this.RaiseAndSetIfChanged(ref _lineShapeTool, value);
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
                                case PathToolMode.Move:
                                    {
                                        // TODO: startPoint
                                    }
                                    break;
                                case PathToolMode.Line:
                                    {
                                        _lineShapeTool?.Pressed(_itemsCanvasAdapter, x, y, pointerType, keyModifiers);
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
                                        // TODO:
                                    }
                                    break;
                                case PathToolMode.QuadraticBezier:
                                    {
                                        // TODO:
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
                                        // TODO:
                                    }
                                    break;
                                case PathToolMode.QuadraticBezier:
                                    {
                                        // TODO:
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
            switch (_state)
            {
                case PathState.StartPoint:
                    {
                    }
                    break;
                case PathState.NextPoint:
                    {
                    }
                    break;
            }
        }

        public override void Moved(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers)
        {
            switch (_state)
            {
                case PathState.StartPoint:
                    {
                    }
                    break;
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
                                case PathToolMode.Move:
                                    {
                                        // TODO: startPoint
                                    }
                                    break;
                                case PathToolMode.Line:
                                    {
                                        _lineShapeTool?.Moved(_itemsCanvasAdapter, x, y, pointerType, keyModifiers);
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
