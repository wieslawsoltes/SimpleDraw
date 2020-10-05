using System.Collections.Generic;
using System.Runtime.Serialization;
using ReactiveUI;
using SimpleDraw.Skia;
using SimpleDraw.ViewModels.Containers;
using SimpleDraw.ViewModels.Media;
using SimpleDraw.ViewModels.Primitives;
using SimpleDraw.ViewModels.Shapes;

namespace SimpleDraw.ViewModels.Tools
{
    [DataContract(IsReference = true)]
    public class RectangleToolViewModel : ToolBaseViewModel
    {
        private enum RectangleState { TopLeft, BottomRight }
        private RectangleState _state = RectangleState.TopLeft;
        private RectangleShapeViewModel _rectangle;
        private BrushViewModel _brush;
        private PenViewModel _pen;
        private double _hitRadius;
        private bool _tryToConnect;
        private bool _isStroked;
        private bool _isFilled;
        private double _radiusX;
        private double _radiusY;

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
        public double RadiusX
        {
            get => _radiusX;
            set => this.RaiseAndSetIfChanged(ref _radiusX, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public double RadiusY
        {
            get => _radiusY;
            set => this.RaiseAndSetIfChanged(ref _radiusY, value);
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
        public override string Name => "Rectangle";

        private void TryToHover(IItemsCanvas canvas, double x, double y)
        {
            if (_tryToConnect)
            {
                var result = SkiaHitTest.Contains(canvas.Items, x, y, _hitRadius);
                if (result is PointViewModel point)
                {
                    canvas.Hovered.Add(point);
                }
            }
        }

        private void ResetHover(IItemsCanvas canvas)
        {
            canvas.Hovered.Clear();
        }

        public override void Pressed(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers)
        {
            switch (_state)
            {
                case RectangleState.TopLeft:
                    {
                        if (pointerType == ToolPointerType.Left)
                        {
                            var shared = new Dictionary<ViewModelBase, ViewModelBase>();
                            var topLeft = default(PointViewModel);

                            ResetHover(canvas);

                            if (_tryToConnect)
                            {
                                var result = SkiaHitTest.Contains(canvas.Items, x, y, _hitRadius);
                                if (result is PointViewModel point)
                                {
                                    topLeft = point;
                                }
                            }

                            _rectangle = new RectangleShapeViewModel()
                            {
                                TopLeft = topLeft ?? new PointViewModel(x, y),
                                BottomRight = new PointViewModel(x, y),
                                IsStroked = _isStroked,
                                IsFilled = _isFilled,
                                RadiusX = _radiusX,
                                RadiusY = _radiusY,
                                Brush = _brush.CloneSelf(shared),
                                Pen = _pen.CloneSelf(shared)
                            };
                            canvas.Decorators.Add(_rectangle);
                            canvas.Invalidate();
                            _state = RectangleState.BottomRight;
                        }
                    }
                    break;
                case RectangleState.BottomRight:
                    {
                        if (pointerType == ToolPointerType.Left)
                        {
                            var bottomRight = default(PointViewModel);

                            ResetHover(canvas);

                            if (_tryToConnect)
                            {
                                var result = SkiaHitTest.Contains(canvas.Items, x, y, _hitRadius);
                                if (result is PointViewModel point)
                                {
                                    bottomRight = point;
                                }
                            }

                            if (bottomRight != null)
                            {
                                _rectangle.BottomRight = bottomRight;
                            }

                            canvas.Decorators.Remove(_rectangle);
                            canvas.Items.Add(_rectangle);
                            canvas.Invalidate();

                            _rectangle = null;
                            _state = RectangleState.TopLeft;
                        }

                        if (pointerType == ToolPointerType.Right)
                        {
                            ResetHover(canvas);
                            canvas.Decorators.Remove(_rectangle);
                            canvas.Invalidate();
                            _rectangle = null;
                            _state = RectangleState.TopLeft;
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
                case RectangleState.TopLeft:
                    {
                        ResetHover(canvas);
                        TryToHover(canvas, x, y);
                        canvas.Invalidate();
                    }
                    break;
                case RectangleState.BottomRight:
                    {
                        if (pointerType == ToolPointerType.None)
                        {
                            ResetHover(canvas);
                            TryToHover(canvas, x, y);
                            _rectangle.BottomRight.X = x;
                            _rectangle.BottomRight.Y = y;
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
                return value as RectangleToolViewModel;
            }

            var copy = new RectangleToolViewModel()
            {
                Brush = _brush?.CloneSelf(shared),
                Pen = _pen?.CloneSelf(shared),
                IsStroked = _isStroked,
                IsFilled = _isFilled,
                RadiusX = _radiusX,
                RadiusY = _radiusY
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
