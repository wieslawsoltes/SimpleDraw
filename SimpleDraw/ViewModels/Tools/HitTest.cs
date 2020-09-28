using System;
using SkiaSharp;

namespace SimpleDraw.ViewModels
{
    internal static class HitTest
    {
        private static SKRect ToSKRect(RectangleShapeViewModel rectangleShape)
        {
            var x = Math.Min(rectangleShape.TopLeft.X, rectangleShape.BottomRight.X);
            var y = Math.Min(rectangleShape.TopLeft.Y, rectangleShape.BottomRight.Y);
            var width = Math.Abs(rectangleShape.TopLeft.X - rectangleShape.BottomRight.X);
            var height = Math.Abs(rectangleShape.TopLeft.Y - rectangleShape.BottomRight.Y);
            return SKRect.Create((float)x, (float)y, (float)width, (float)height);
        }

        private static SKRect Expand(SKPoint point, double radius)
        {
            return SKRect.Create(
                (float)(point.X - radius),
                (float)(point.Y - radius),
                (float)(radius + radius),
                (float)(radius + radius));
        }

        public static ViewModelBase Contains(CanvasViewModel canvas, double x, double y, double hitRadius)
        {
            foreach (var shape in canvas.Shapes)
            {
                var result = Contains(x, y, hitRadius, shape);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        private static ViewModelBase Contains(double x, double y, double hitRadius, ShapeBaseViewModel shape)
        {
            switch (shape)
            {
                case LineShapeViewModel lineShape:
                    {
                        var p1 = new SKPoint((float)lineShape.Start.X, (float)lineShape.Start.Y);
                        var p2 = new SKPoint((float)lineShape.End.X, (float)lineShape.End.Y);

                        var containsStart = Expand(p1, hitRadius).Contains((float)x, (float)y);
                        if (containsStart)
                        {
                            return lineShape.Start;
                        }

                        var containsEnd = Expand(p2, hitRadius).Contains((float)x, (float)y);
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

                        var containsTopLeft = Expand(tl, hitRadius).Contains((float)x, (float)y);
                        if (containsTopLeft)
                        {
                            return rectangleShape.TopLeft;
                        }

                        var containsBottomRight = Expand(br, hitRadius).Contains((float)x, (float)y);
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
            }

            return null;
        }
    }
}
