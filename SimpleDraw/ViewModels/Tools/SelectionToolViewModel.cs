using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ReactiveUI;
using SimpleDraw.Skia;
using SimpleDraw.ViewModels.Containers;
using SimpleDraw.ViewModels.Primitives;
using SimpleDraw.ViewModels.Shapes;

namespace SimpleDraw.ViewModels.Tools
{
    [DataContract(IsReference = true)]
    public class SelectionToolViewModel : ToolBaseViewModel
    {
        private enum State { None, Selected, Move }
        private State _state = State.None;
        private double _hitRadius;
        private double _disconnectRadius;
        private bool _tryToConnect;
        private double _pressedX = double.NaN;
        private double _pressedY = double.NaN;
        private double _previousX = double.NaN;
        private double _previousY = double.NaN;
        private bool _connected = false;
        private bool _disconnected = false;
        private List<ViewModelBase> _connectedItems = new List<ViewModelBase>();

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
        public double DisconnectRadius
        {
            get => _disconnectRadius;
            set => this.RaiseAndSetIfChanged(ref _disconnectRadius, value);
        }

        [IgnoreDataMember]
        public override string Name => "Selection";

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
                        _pressedX = x;
                        _pressedY = y;
                        _connected = false;
                        _disconnected = false;
                        ResetHover(canvas);

                        var result = SkiaHitTest.Contains(canvas.Items, (_) => true, x, y, _hitRadius);
                        if (result != null)
                        {
                            if (keyModifiers.HasFlag(ToolKeyModifiers.Control))
                            {
                                if (canvas.Selected.Contains(result))
                                {
                                    canvas.Selected.Remove(result);
                                    canvas.Invalidate();
                                }
                                else
                                {
                                    canvas.Selected.Add(result);
                                    canvas.Invalidate();
                                }
                            }
                            else
                            {
                                if (!canvas.Selected.Contains(result))
                                {
                                    canvas.Selected.Clear();
                                    canvas.Selected.Add(result);
                                    canvas.Invalidate();
                                }
                            }
                            canvas.UpdateSelectionBounds();
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
                            canvas.ShowSelectionDecorator(x, y);
                            canvas.UpdateSelectionBounds();
                            canvas.Invalidate();
                            _state = State.Move;
                        }
                    }
                    break;
                case State.Selected:
                    {
                        _pressedX = x;
                        _pressedY = y;
                        _connected = false;
                        _disconnected = false;
                        ResetHover(canvas);
                        canvas.Invalidate();
                        _state = State.None;
                    }
                    break;
                case State.Move:
                    {
                        _pressedX = x;
                        _pressedY = y;
                        _connected = false;
                        _disconnected = false;
                        ResetHover(canvas);
                        canvas.Invalidate();
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
                case State.Selected:
                    {
                        _state = State.None;
                    }
                    break;
                case State.Move:
                    {
                        ResetHover(canvas);
                        canvas.RemoveSelectionDecorator();
                        canvas.RemoveSelectionBounds();

                        var rect = SkiaRenderer.ToSKRect(_pressedX, _pressedY, x, y);
                        if (keyModifiers.HasFlag(ToolKeyModifiers.Control))
                        {
                            for (int i = canvas.Items.Count - 1; i >= 0; i--)
                            {
                                var shape = canvas.Items[i];
                                var result = SkiaHitTest.Intersects(shape, (_) => true, rect);
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

                            canvas.UpdateSelectionBounds();
                            canvas.Invalidate();
                        }
                        else
                        {
                            canvas.Selected.Clear();

                            for (int i = canvas.Items.Count - 1; i >= 0; i--)
                            {
                                var shape = canvas.Items[i];
                                var result = SkiaHitTest.Intersects(shape, (_) => true, rect);
                                if (result != null)
                                {
                                    canvas.Selected.Add(result);
                                }
                            }

                            canvas.UpdateSelectionBounds();
                            canvas.Invalidate();
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
                        ResetHover(canvas);
                        TryToHover(canvas, x, y);
                        canvas.Invalidate();
                    }
                    break;
                case State.Selected:
                    {
                        ResetHover(canvas);

                        if (keyModifiers == ToolKeyModifiers.Shift)
                        {
                            _connected = TryToConnectPoints(canvas, x, y);
                        }

                        if (keyModifiers == ToolKeyModifiers.Alt && _disconnected == false && Math.Abs(_pressedX - x) >= _disconnectRadius)
                        {
                            _disconnected = TryToDisconnectPoints(canvas, x, y);
                        }

                        if (!keyModifiers.HasFlag(ToolKeyModifiers.Alt) || _disconnected == true)
                        {
                            double deltaX = x - _previousX;
                            double deltaY = y - _previousY;
                            canvas.MoveSelected(deltaX, deltaY);
                            canvas.UpdateSelectionBounds();
                            canvas.Invalidate();
                            _previousX = x;
                            _previousY = y;
                        }
                    }
                    break;
                case State.Move:
                    {
                        ResetHover(canvas);
                        canvas.MoveSelectionDecorator(x, y);
                        canvas.Invalidate();
                    }
                    break;
            }
        }

        private bool TryToConnectPoints(CanvasViewModel canvas, double x, double y)
        {
            if (!(canvas.Selected.Count == 1 && canvas.Selected[0] is PointViewModel selectedPoint))
            {
                return false;
            }

            var result = SkiaHitTest.Contains(canvas.Items, (x) => x != selectedPoint, x, y, _hitRadius);
            if (result != null && result is PointViewModel hitTestPoint)
            {
                return ConnectPoint(canvas.Items, selectedPoint, hitTestPoint);
            }

            return false;
        }

        private bool ConnectPoint(ObservableCollection<ViewModelBase> items, PointViewModel selectedPoint, PointViewModel hitTestPoint)
        {
            foreach (var item in items)
            {
                switch (item)
                {
                    case GroupViewModel group:
                        {
                            if (ConnectPoint(group.Items, selectedPoint, hitTestPoint))
                            {
                                return true;
                            }
                        }
                        break;
                    case LineShapeViewModel lineShape:
                        {
                            if (lineShape.StartPoint == hitTestPoint 
                                && lineShape.Point != selectedPoint)
                            {
                                lineShape.StartPoint = selectedPoint;
                                return true;
                            }

                            if (lineShape.Point == hitTestPoint 
                                && lineShape.StartPoint != selectedPoint)
                            {
                                lineShape.Point = selectedPoint;
                                return true;
                            }
                        }
                        break;
                    case CubicBezierShapeViewModel cubicBezierShape:
                        {
                            if (cubicBezierShape.StartPoint == hitTestPoint
                                && cubicBezierShape.Point1 != selectedPoint
                                && cubicBezierShape.Point2 != selectedPoint
                                && cubicBezierShape.Point3 != selectedPoint)
                            {
                                cubicBezierShape.StartPoint = selectedPoint;
                                return true;
                            }

                            if (cubicBezierShape.Point1 == hitTestPoint
                                && cubicBezierShape.StartPoint != selectedPoint
                                && cubicBezierShape.Point2 != selectedPoint
                                && cubicBezierShape.Point3 != selectedPoint)
                            {
                                cubicBezierShape.Point1 = selectedPoint;
                                return true;
                            }

                            if (cubicBezierShape.Point2 == hitTestPoint
                                && cubicBezierShape.StartPoint != selectedPoint
                                && cubicBezierShape.Point1 != selectedPoint
                                && cubicBezierShape.Point3 != selectedPoint)
                            {
                                cubicBezierShape.Point2 = selectedPoint;
                                return true;
                            }

                            if (cubicBezierShape.Point3 == hitTestPoint
                                && cubicBezierShape.StartPoint != selectedPoint
                                && cubicBezierShape.Point1 != selectedPoint
                                && cubicBezierShape.Point2 != selectedPoint)
                            {
                                cubicBezierShape.Point3 = selectedPoint;
                                return true;
                            }
                        }
                        break;
                    case QuadraticBezierShapeViewModel quadraticBezierShape:
                        {
                            if (quadraticBezierShape.StartPoint == hitTestPoint
                                && quadraticBezierShape.Control != selectedPoint
                                && quadraticBezierShape.EndPoint != selectedPoint)
                            {
                                quadraticBezierShape.StartPoint = selectedPoint;
                                return true;
                            }

                            if (quadraticBezierShape.Control == hitTestPoint
                                && quadraticBezierShape.StartPoint != selectedPoint
                                && quadraticBezierShape.EndPoint != selectedPoint)
                            {
                                quadraticBezierShape.Control = selectedPoint;
                                return true;
                            }

                            if (quadraticBezierShape.EndPoint == hitTestPoint
                                && quadraticBezierShape.StartPoint != selectedPoint
                                && quadraticBezierShape.Control != selectedPoint)
                            {
                                quadraticBezierShape.EndPoint = selectedPoint;
                                return true;
                            }
                        }
                        break;
                    case PathShapeViewModel pathShape:
                        {
                            foreach (var figure in pathShape.Figures)
                            {
                                if (ConnectPoint(figure.Segments, selectedPoint, hitTestPoint))
                                {
                                    return true;
                                }
                            }
                        }
                        break;
                    case RectangleShapeViewModel rectangleShape:
                        {
                            if (rectangleShape.TopLeft == hitTestPoint
                                && rectangleShape.BottomRight != selectedPoint)
                            {
                                rectangleShape.TopLeft = selectedPoint;
                                return true;
                            }

                            if (rectangleShape.BottomRight == hitTestPoint
                                && rectangleShape.TopLeft != selectedPoint)
                            {
                                rectangleShape.BottomRight = selectedPoint;
                                return true;
                            }
                        }
                        break;
                    case EllipseShapeViewModel ellipseShape:
                        {
                            if (ellipseShape.TopLeft == hitTestPoint
                                && ellipseShape.BottomRight != selectedPoint)
                            {
                                ellipseShape.TopLeft = selectedPoint;
                                return true;
                            }

                            if (ellipseShape.BottomRight == hitTestPoint
                                && ellipseShape.TopLeft != selectedPoint)
                            {
                                ellipseShape.BottomRight = selectedPoint;
                                return true;
                            }
                        }
                        break;
                }
            }

            return false;
        }

        private bool TryToDisconnectPoints(CanvasViewModel canvas, double x, double y)
        {
            if (!(canvas.Selected.Count == 1 && canvas.Selected[0] is PointViewModel selectedPoint))
            {
                return false;
            }

            _connectedItems.Clear();
            GetConnected(canvas.Items, selectedPoint, _connectedItems);

            if (_connectedItems.Count <= 1)
            {
                return false;
            }

            var result = DisconnectPoint(canvas.Items, selectedPoint);
            if (result != null)
            {
                canvas.Selected[0] = result;
                return true;
            }

            return false;
        }

        private void GetConnected(ObservableCollection<ViewModelBase> items, PointViewModel selectedPoint, List<ViewModelBase> connected)
        {
            foreach (var item in items)
            {
                switch (item)
                {
                    case GroupViewModel group:
                        {
                            GetConnected(group.Items, selectedPoint, connected);
                        }
                        break;
                    case LineShapeViewModel lineShape:
                        {
                            if (lineShape.StartPoint == selectedPoint)
                            {
                                connected.Add(lineShape);
                                break;
                            }

                            if (lineShape.Point == selectedPoint)
                            {
                                connected.Add(lineShape);
                                break;
                            }
                        }
                        break;
                    case CubicBezierShapeViewModel cubicBezierShape:
                        {
                            if (cubicBezierShape.StartPoint == selectedPoint)
                            {
                                connected.Add(cubicBezierShape);
                                break;
                            }

                            if (cubicBezierShape.Point1 == selectedPoint)
                            {
                                connected.Add(cubicBezierShape);
                                break;
                            }

                            if (cubicBezierShape.Point2 == selectedPoint)
                            {
                                connected.Add(cubicBezierShape);
                                break;
                            }

                            if (cubicBezierShape.Point3 == selectedPoint)
                            {
                                connected.Add(cubicBezierShape);
                                break;
                            }
                        }
                        break;
                    case QuadraticBezierShapeViewModel quadraticBezierShape:
                        {
                            if (quadraticBezierShape.StartPoint == selectedPoint)
                            {
                                connected.Add(quadraticBezierShape);
                                break;
                            }

                            if (quadraticBezierShape.Control == selectedPoint)
                            {
                                connected.Add(quadraticBezierShape);
                                break;
                            }

                            if (quadraticBezierShape.EndPoint == selectedPoint)
                            {
                                connected.Add(quadraticBezierShape);
                                break;
                            }
                        }
                        break;
                    case PathShapeViewModel pathShape:
                        {
                            foreach (var figure in pathShape.Figures)
                            {
                                GetConnected(figure.Segments, selectedPoint, _connectedItems);
                            }
                        }
                        break;
                    case RectangleShapeViewModel rectangleShape:
                        {
                            if (rectangleShape.TopLeft == selectedPoint)
                            {
                                connected.Add(rectangleShape);
                                break;
                            }

                            if (rectangleShape.BottomRight == selectedPoint)
                            {
                                connected.Add(rectangleShape);
                                break;
                            }
                        }
                        break;
                    case EllipseShapeViewModel ellipseShape:
                        {
                            if (ellipseShape.TopLeft == selectedPoint)
                            {
                                connected.Add(ellipseShape);
                                break;
                            }

                            if (ellipseShape.BottomRight == selectedPoint)
                            {
                                connected.Add(ellipseShape);
                                break;
                            }
                        }
                        break;
                }
            }
        }

        private PointViewModel DisconnectPoint(ObservableCollection<ViewModelBase> items, PointViewModel selectedPoint)
        {
            foreach (var item in items)
            {
                switch (item)
                {
                    case GroupViewModel group:
                        {
                            var result = DisconnectPoint(group.Items, selectedPoint);
                            if (result != null)
                            {
                                return result;
                            }
                        }
                        break;
                    case LineShapeViewModel lineShape:
                        {
                            if (lineShape.StartPoint == selectedPoint)
                            {
                                lineShape.StartPoint = new PointViewModel(selectedPoint.X, selectedPoint.Y);
                                return lineShape.StartPoint;
                            }

                            if (lineShape.Point == selectedPoint)
                            {
                                lineShape.Point = new PointViewModel(selectedPoint.X, selectedPoint.Y);
                                return lineShape.Point;
                            }
                        }
                        break;
                    case CubicBezierShapeViewModel cubicBezierShape:
                        {
                            if (cubicBezierShape.StartPoint == selectedPoint)
                            {
                                cubicBezierShape.StartPoint = new PointViewModel(selectedPoint.X, selectedPoint.Y);
                                return cubicBezierShape.StartPoint;
                            }

                            if (cubicBezierShape.Point1 == selectedPoint)
                            {
                                cubicBezierShape.Point1 = new PointViewModel(selectedPoint.X, selectedPoint.Y);
                                return cubicBezierShape.Point1;
                            }

                            if (cubicBezierShape.Point2 == selectedPoint)
                            {
                                cubicBezierShape.Point2 = new PointViewModel(selectedPoint.X, selectedPoint.Y);
                                return cubicBezierShape.Point2;
                            }

                            if (cubicBezierShape.Point3 == selectedPoint)
                            {
                                cubicBezierShape.Point3 = new PointViewModel(selectedPoint.X, selectedPoint.Y);
                                return cubicBezierShape.Point3;
                            }
                        }
                        break;
                    case QuadraticBezierShapeViewModel quadraticBezierShape:
                        {
                            if (quadraticBezierShape.StartPoint == selectedPoint)
                            {
                                quadraticBezierShape.StartPoint = new PointViewModel(selectedPoint.X, selectedPoint.Y);
                                return quadraticBezierShape.StartPoint;
                            }

                            if (quadraticBezierShape.Control == selectedPoint)
                            {
                                quadraticBezierShape.Control = new PointViewModel(selectedPoint.X, selectedPoint.Y);
                                return quadraticBezierShape.Control;
                            }

                            if (quadraticBezierShape.EndPoint == selectedPoint)
                            {
                                quadraticBezierShape.EndPoint = new PointViewModel(selectedPoint.X, selectedPoint.Y);
                                return quadraticBezierShape.EndPoint;
                            }
                        }
                        break;
                    case PathShapeViewModel pathShape:
                        {
                            foreach (var figure in pathShape.Figures)
                            {
                                var result = DisconnectPoint(figure.Segments, selectedPoint);
                                {
                                    return result;
                                }
                            }
                        }
                        break;
                    case RectangleShapeViewModel rectangleShape:
                        {
                            if (rectangleShape.TopLeft == selectedPoint)
                            {
                                rectangleShape.TopLeft = new PointViewModel(selectedPoint.X, selectedPoint.Y);
                                return rectangleShape.TopLeft;
                            }

                            if (rectangleShape.BottomRight == selectedPoint)
                            {
                                rectangleShape.BottomRight = new PointViewModel(selectedPoint.X, selectedPoint.Y);
                                return rectangleShape.BottomRight;
                            }
                        }
                        break;
                    case EllipseShapeViewModel ellipseShape:
                        {
                            if (ellipseShape.TopLeft == selectedPoint)
                            {
                                ellipseShape.TopLeft = new PointViewModel(selectedPoint.X, selectedPoint.Y);
                                return ellipseShape.TopLeft;
                            }

                            if (ellipseShape.BottomRight == selectedPoint)
                            {
                                ellipseShape.BottomRight = new PointViewModel(selectedPoint.X, selectedPoint.Y);
                                return ellipseShape.BottomRight;
                            }
                        }
                        break;
                }
            }

            return null;
        }

        public override ToolBaseViewModel CloneSelf(Dictionary<ViewModelBase, ViewModelBase> shared)
        {
            if (shared.TryGetValue(this, out var value))
            {
                return value as SelectionToolViewModel;
            }

            var copy = new SelectionToolViewModel()
            {
                HitRadius = _hitRadius,
                TryToConnect = _tryToConnect
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
