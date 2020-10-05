using System.Collections.Generic;
using System.Runtime.Serialization;
using ReactiveUI;
using SimpleDraw.Skia;
using SimpleDraw.ViewModels.Containers;
using SimpleDraw.ViewModels.Media;
using SimpleDraw.ViewModels.Primitives;
using SimpleDraw.ViewModels.Shapes;

namespace SimpleDraw.ViewModels.Tools.Shape
{
    [DataContract(IsReference = true)]
    public class CubicBezierShapeToolViewModel : ShapeToolViewModel
    {
        private enum CubicBezierState { StartPoint, Point1, Point2, Point3 }
        private CubicBezierState _state = CubicBezierState.StartPoint;
        private CubicBezierShapeViewModel _cubicBezier;
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

        public override void Pressed(IItemsCanvas canvas, double x, double y, ToolPointerType pointerType, ToolKeyModifiers keyModifiers)
        {
            switch (_state)
            {
                case CubicBezierState.StartPoint:
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

                            _cubicBezier = new CubicBezierShapeViewModel()
                            {
                                StartPoint = topLeft ?? new PointViewModel(x, y),
                                Point1 = new PointViewModel(x, y),
                                Point2 = new PointViewModel(x, y),
                                Point3 = new PointViewModel(x, y),
                                IsStroked = _isStroked,
                                IsFilled = _isFilled,
                                Brush = _brush.CloneSelf(shared),
                                Pen = _pen.CloneSelf(shared)
                            };
                            canvas.Decorators.Add(_cubicBezier);
                            canvas.Invalidate();
                            _state = CubicBezierState.Point3;
                        }
                    }
                    break;
                case CubicBezierState.Point3:
                    {
                        if (pointerType == ToolPointerType.Left)
                        {
                            var point3 = default(PointViewModel);

                            ResetHover(canvas);

                            if (_tryToConnect)
                            {
                                var result = SkiaHitTest.Contains(canvas.Items, x, y, _hitRadius);
                                if (result is PointViewModel point)
                                {
                                    point3 = point;
                                }
                            }

                            if (point3 != null)
                            {
                                _cubicBezier.Point3 = point3;
                            }

                            canvas.Invalidate();

                            _state = CubicBezierState.Point2;
                        }

                        if (pointerType == ToolPointerType.Right)
                        {
                            canvas.Decorators.Remove(_cubicBezier);
                            canvas.Invalidate();
                            _cubicBezier = null;
                            _state = CubicBezierState.StartPoint;
                        }
                    }
                    break;
                case CubicBezierState.Point2:
                    {
                        if (pointerType == ToolPointerType.Left)
                        {
                            var point2 = default(PointViewModel);

                            ResetHover(canvas);

                            if (_tryToConnect)
                            {
                                var result = SkiaHitTest.Contains(canvas.Items, x, y, _hitRadius);
                                if (result is PointViewModel point)
                                {
                                    point2 = point;
                                }
                            }

                            if (point2 != null)
                            {
                                _cubicBezier.Point2 = point2;
                            }

                            canvas.Invalidate();

                            _state = CubicBezierState.Point1;
                        }

                        if (pointerType == ToolPointerType.Right)
                        {
                            ResetHover(canvas);
                            canvas.Decorators.Remove(_cubicBezier);
                            canvas.Invalidate();
                            _cubicBezier = null;
                            _state = CubicBezierState.StartPoint;
                        }
                    }
                    break;
                case CubicBezierState.Point1:
                    {
                        if (pointerType == ToolPointerType.Left)
                        {
                            var point1 = default(PointViewModel);

                            ResetHover(canvas);

                            if (_tryToConnect)
                            {
                                var result = SkiaHitTest.Contains(canvas.Items, x, y, _hitRadius);
                                if (result is PointViewModel point)
                                {
                                    point1 = point;
                                }
                            }

                            if (point1 != null)
                            {
                                _cubicBezier.Point1 = point1;
                            }

                            canvas.Decorators.Remove(_cubicBezier);
                            canvas.Items.Add(_cubicBezier);
                            canvas.Invalidate();

                            _cubicBezier = null;
                            _state = CubicBezierState.StartPoint;
                        }

                        if (pointerType == ToolPointerType.Right)
                        {
                            ResetHover(canvas);
                            canvas.Decorators.Remove(_cubicBezier);
                            canvas.Invalidate();
                            _cubicBezier = null;
                            _state = CubicBezierState.StartPoint;
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
                case CubicBezierState.StartPoint:
                    {
                        ResetHover(canvas);
                        TryToHover(canvas, x, y);
                        canvas.Invalidate();
                    }
                    break;
                case CubicBezierState.Point3:
                    {
                        if (pointerType == ToolPointerType.None)
                        {
                            ResetHover(canvas);
                            TryToHover(canvas, x, y);
                            _cubicBezier.Point2.X = x;
                            _cubicBezier.Point2.Y = y;
                            _cubicBezier.Point3.X = x;
                            _cubicBezier.Point3.Y = y;
                            canvas.Invalidate();
                        }
                    }
                    break;
                case CubicBezierState.Point2:
                    {
                        if (pointerType == ToolPointerType.None)
                        {
                            ResetHover(canvas);
                            TryToHover(canvas, x, y);
                            _cubicBezier.Point2.X = x;
                            _cubicBezier.Point2.Y = y;
                            _cubicBezier.Point1.X = x;
                            _cubicBezier.Point1.Y = y;
                            canvas.Invalidate();
                        }
                    }
                    break;
                case CubicBezierState.Point1:
                    {
                        if (pointerType == ToolPointerType.None)
                        {
                            ResetHover(canvas);
                            TryToHover(canvas, x, y);
                            _cubicBezier.Point1.X = x;
                            _cubicBezier.Point1.Y = y;
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
                return value as CubicBezierShapeToolViewModel;
            }

            var copy = new CubicBezierShapeToolViewModel()
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
