using System;
using System.Diagnostics;
using ReactiveUI;
using SkiaSharp;

namespace SimpleDraw.ViewModels
{
    public class SelectionToolViewModel : ToolBaseViewModel
    {
        private enum State { None, Pressed }
        private State _state = State.None;
        private double _hitRadius = 6;
        private ViewModelBase _selected = null;

        public double HitRadius
        {
            get => _hitRadius;
            set => this.RaiseAndSetIfChanged(ref _hitRadius, value);
        }

        public override string Name => "Selection";

        public override void Pressed(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType)
        {
            switch (_state)
            {
                case State.None:
                    {
                        var result = HitTest(canvas, x, y);
                        if (result != null)
                        {
                            // TODO:
                            Debug.WriteLine($"{result}");
                        }
                    }
                    break;
                case State.Pressed:
                    {
                        // TODO:
                    }
                    break;
            }
        }

        public override void Released(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType)
        {
            switch (_state)
            {
                case State.None:
                    {
                        // TODO:
                    }
                    break;
                case State.Pressed:
                    {
                        // TODO:
                    }
                    break;
            }
        }

        public override void Moved(CanvasViewModel canvas, double x, double y, ToolPointerType pointerType)
        {
            switch (_state)
            {
                case State.None:
                    {
                        var result = HitTest(canvas, x, y);
                        if (result != null)
                        {
                            // TODO:
                            Debug.WriteLine($"{result}");
                        }
                    }
                    break;
                case State.Pressed:
                    {
                        // TODO:
                    }
                    break;
            }
        }

        public static SKRect ToSKRect(RectangleShapeViewModel rectangleShape)
        {
            var x = Math.Min(rectangleShape.TopLeft.X, rectangleShape.BottomRight.X);
            var y = Math.Min(rectangleShape.TopLeft.Y, rectangleShape.BottomRight.Y);
            var width = Math.Abs(rectangleShape.TopLeft.X - rectangleShape.BottomRight.X);
            var height = Math.Abs(rectangleShape.TopLeft.Y - rectangleShape.BottomRight.Y);
            return SKRect.Create((float)x, (float)y, (float)width, (float)height);
        }

        private SKRect Expand(SKPoint point, double radius)
        {
            return SKRect.Create(
                (float)(point.X - radius), 
                (float)(point.Y - radius), 
                (float)(radius + radius), 
                (float)(radius + radius));
        }

        private ViewModelBase HitTest(CanvasViewModel canvas, double x, double y)
        {
            foreach (var shape in canvas.Shapes)
            {
                switch (shape)
                {
                    case LineShapeViewModel lineShape:
                        {
                            var p1 = new SKPoint((float)lineShape.Start.X, (float)lineShape.Start.Y);
                            var p2 = new SKPoint((float)lineShape.End.X, (float)lineShape.End.Y);

                            var containsStart = Expand(p1, _hitRadius).Contains((float)x, (float)y);
                            if (containsStart)
                            {
                                return lineShape.Start;
                            }

                            var containsEnd = Expand(p2, _hitRadius).Contains((float)x, (float)y);
                            if (containsEnd)
                            {
                                return lineShape.End;
                            }

                            var path = new SKPath() { FillType = SKPathFillType.Winding };
                            path.MoveTo(p1);
                            path.LineTo(p2);

                            var bounds = path.ComputeTightBounds();

                            var contains = bounds.Contains((float)x, (float)y);
                            if (contains)
                            {
                                return lineShape;
                            }
                        }
                        break;
                    case RectangleShapeViewModel rectangleShape:
                        {
                            var tl = new SKPoint((float)rectangleShape.TopLeft.X, (float)rectangleShape.TopLeft.Y);
                            var br = new SKPoint((float)rectangleShape.BottomRight.X, (float)rectangleShape.BottomRight.Y);

                            var containsTopLeft = Expand(tl, _hitRadius).Contains((float)x, (float)y);
                            if (containsTopLeft)
                            {
                                return rectangleShape.TopLeft;
                            }

                            var containsBottomRight = Expand(br, _hitRadius).Contains((float)x, (float)y);
                            if (containsBottomRight)
                            {
                                return rectangleShape.BottomRight;
                            }

                            var rect = ToSKRect(rectangleShape);

                            var path = new SKPath() { FillType = SKPathFillType.Winding };
                            path.AddRect(rect);

                            var bounds = path.ComputeTightBounds();

                            var contains = bounds.Contains((float)x, (float)y);
                            if (contains)
                            {
                                return rectangleShape;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            return null;
        }
    }
}
