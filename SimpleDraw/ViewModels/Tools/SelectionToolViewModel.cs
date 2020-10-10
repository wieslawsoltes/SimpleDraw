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
                                if (keyModifiers == ToolKeyModifiers.Shift)
                                {
                                    if (result is PointViewModel hitTestPoint)
                                    {
                                        if (canvas.Selected.Count == 1 && canvas.Selected[0] is PointViewModel selectedPoint && selectedPoint != hitTestPoint)
                                        {
                                            var connected = canvas.ConnectPoint(canvas.Items, selectedPoint, hitTestPoint);
                                            if (connected)
                                            {
                                                canvas.Selected.Clear();
                                                canvas.UpdateSelectionBounds();
                                                canvas.Invalidate();
                                                break;
                                            }
                                        }
                                    }
                                }
#if false
                                if (keyModifiers == ToolKeyModifiers.Alt)
                                {
                                    if (canvas.Selected.Count == 0 && result is PointViewModel hitTestPoint)
                                    {
                                        var resultDisconnect = canvas.DisconnectPoint(canvas.Items, hitTestPoint);
                                        if (resultDisconnect != null)
                                        {
                                            canvas.Selected.Clear();
                                            canvas.UpdateSelectionBounds();
                                            canvas.Invalidate();
                                            break;
                                        }
                                    }

                                    if (canvas.Selected.Count == 1 && canvas.Selected[0] is PointViewModel selectedPoint && selectedPoint == result)
                                    {
                                        var resultDisconnect = canvas.DisconnectPoint(canvas.Items, selectedPoint);
                                        if (resultDisconnect != null)
                                        {
                                            canvas.Selected.Clear();
                                            canvas.UpdateSelectionBounds();
                                            canvas.Invalidate();
                                            break;
                                        }
                                    }
                                }
#endif

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
                            _disconnected = TryToDisconnectPoints(canvas);
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
                return canvas.ConnectPoint(canvas.Items, selectedPoint, hitTestPoint);
            }

            return false;
        }

        private bool TryToDisconnectPoints(CanvasViewModel canvas)
        {
            if (!(canvas.Selected.Count == 1 && canvas.Selected[0] is PointViewModel selectedPoint))
            {
                return false;
            }

            _connectedItems.Clear();
            canvas.GetConnected(canvas.Items, selectedPoint, _connectedItems);

            if (_connectedItems.Count <= 1)
            {
                return false;
            }

            var result = canvas.DisconnectPoint(canvas.Items, selectedPoint);
            if (result != null)
            {
                canvas.Selected[0] = result;
                return true;
            }

            return false;
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
