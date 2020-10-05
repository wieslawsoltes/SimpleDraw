﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using ReactiveUI;
using SimpleDraw.Skia;
using SimpleDraw.ViewModels.Containers;
using SimpleDraw.ViewModels.Primitives;

namespace SimpleDraw.ViewModels.Tools
{
    [DataContract(IsReference = true)]
    public class SelectionToolViewModel : ToolBaseViewModel
    {
        private enum State { None, Selected, Move }
        private State _state = State.None;
        private double _hitRadius;
        private bool _tryToConnect;
        private double _pressedX = double.NaN;
        private double _pressedY = double.NaN;
        private double _previousX = double.NaN;
        private double _previousY = double.NaN;

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
        public override string Name => "Selection";

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
            if (canvas.Selected == null)
            {
                return;
            }

            switch (_state)
            {
                case State.None:
                    {
                        ResetHover(canvas);

                        var result = SkiaHitTest.Contains(canvas.Items, x, y, _hitRadius);
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
                            _pressedX = x;
                            _pressedY = y;
                            _state = State.Move;
                        }
                    }
                    break;
                case State.Selected:
                    {
                        ResetHover(canvas);
                        canvas.Invalidate();
                        _state = State.None;
                    }
                    break;
                case State.Move:
                    {
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
                                var result = SkiaHitTest.Intersects(shape, rect);
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
                                var result = SkiaHitTest.Intersects(shape, rect);
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
                        double deltaX = x - _previousX;
                        double deltaY = y - _previousY;
                        canvas.MoveSelected(deltaX, deltaY);
                        canvas.UpdateSelectionBounds();
                        canvas.Invalidate();
                        _previousX = x;
                        _previousY = y;
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
