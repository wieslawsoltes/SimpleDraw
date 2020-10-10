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
    public class LineShapeToolViewModel : ShapeToolViewModel
    {
        private enum LineState { StartPoint, Point }
        private LineState _state = LineState.StartPoint;
        private LineShapeViewModel _line = null;
        private PenViewModel _pen;
        private bool _isStroked;
        private double _hitRadius;
        private bool _tryToConnect;

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
                var result = SkiaHitTest.Contains(canvas.Items, (_) => true, x, y, _hitRadius);
                if (result != null)
                {
                    canvas.Hovered.Add(result);
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
                case LineState.StartPoint:
                    {
                        if (pointerType == ToolPointerType.Left)
                        {
                            var shared = new Dictionary<ViewModelBase, ViewModelBase>();
                            var start = default(PointViewModel);

                            ResetHover(canvas);

                            if (_tryToConnect)
                            {
                                var result = SkiaHitTest.Contains(canvas.Items, (_) => true, x, y, _hitRadius);
                                if (result is PointViewModel point)
                                {
                                    start = point;
                                }
                            }

                            _line = new LineShapeViewModel()
                            {
                                StartPoint = start ?? new PointViewModel(x, y),
                                Point = new PointViewModel(x, y),
                                IsStroked = _isStroked,
                                Pen = _pen.CloneSelf(shared)
                            };
                            canvas.Decorators.Add(_line);
                            canvas.Invalidate();
                            _state = LineState.Point;
                        }
                    }
                    break;
                case LineState.Point:
                    {
                        if (pointerType == ToolPointerType.Left)
                        {
                            var end = default(PointViewModel);

                            ResetHover(canvas);

                            if (_tryToConnect)
                            {
                                var result = SkiaHitTest.Contains(canvas.Items, (_) => true, x, y, _hitRadius);
                                if (result is PointViewModel point)
                                {
                                    end = point;
                                }
                            }

                            if (end != null)
                            {
                                _line.Point = end;
                            }

                            canvas.Decorators.Remove(_line);
                            canvas.Items.Add(_line);
                            canvas.Invalidate();

                            _line = null;
                            _state = LineState.StartPoint;
                        }

                        if (pointerType == ToolPointerType.Right)
                        {
                            ResetHover(canvas);
                            canvas.Decorators.Remove(_line);
                            canvas.Invalidate();
                            _line = null;
                            _state = LineState.StartPoint;
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
                case LineState.StartPoint:
                    {
                        ResetHover(canvas);
                        TryToHover(canvas, x, y);
                        canvas.Invalidate();
                    }
                    break;
                case LineState.Point:
                    {
                        if (pointerType == ToolPointerType.None)
                        {
                            ResetHover(canvas);
                            TryToHover(canvas, x, y);
                            _line.Point.X = x;
                            _line.Point.Y = y;
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
                return value as LineShapeToolViewModel;
            }

            var copy = new LineShapeToolViewModel()
            {
                Pen = _pen?.CloneSelf(shared),
                IsStroked = _isStroked
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
