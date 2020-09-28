using System;
using System.Collections.Generic;
using SkiaSharp;

namespace SimpleDraw.ViewModels
{
    internal static class HitTest
    {
        public static SKRect ToSKRect(double x1, double y1, double x2, double y2)
        {
            var x = Math.Min(x1, x2);
            var y = Math.Min(y1, y2);
            var width = Math.Abs(x1 - x2);
            var height = Math.Abs(y1 - y2);
            return SKRect.Create((float)x, (float)y, (float)width, (float)height);
        }

        public static SKRect ToSKRect(PointViewModel p1, PointViewModel p2)
        {
            return ToSKRect(p1.X, p1.Y, p2.X, p2.Y);
        }

        public static SKRect ToSKRect(LineShapeViewModel lineShape)
        {
            return ToSKRect(
                lineShape.Start.X,
                lineShape.Start.Y,
                lineShape.End.X,
                lineShape.End.Y);
        }

        public static SKRect ToSKRect(RectangleShapeViewModel rectangleShape)
        {
            return ToSKRect(
                rectangleShape.TopLeft.X,
                rectangleShape.TopLeft.Y,
                rectangleShape.BottomRight.X,
                rectangleShape.BottomRight.Y);
        }

        public static SKRect Expand(SKPoint point, double radius)
        {
            return SKRect.Create(
                (float)(point.X - radius),
                (float)(point.Y - radius),
                (float)(radius + radius),
                (float)(radius + radius));
        }

        public static SKRect GetBounds(LineShapeViewModel lineShape)
        {
            var path = new SKPath() { FillType = SKPathFillType.Winding };
            path.MoveTo(new SKPoint((float)lineShape.Start.X, (float)lineShape.Start.Y));
            path.LineTo(new SKPoint((float)lineShape.End.X, (float)lineShape.End.Y));
            var bounds = path.ComputeTightBounds();
            return bounds;
        }

        public static SKRect GetBounds(RectangleShapeViewModel rectangleShape)
        {
            var rect = ToSKRect(rectangleShape);
            var path = new SKPath() { FillType = SKPathFillType.Winding };
            path.AddRect(rect);
            var bounds = path.ComputeTightBounds();
            return bounds;
        }

        public static ViewModelBase Contains(PointViewModel point, double x, double y, double hitRadius)
        {
            var rect = Expand(new SKPoint((float)point.X, (float)point.Y), hitRadius);
            var result = rect.Contains((float)x, (float)y);
            if (result)
            {
                return point;
            }
            return null;
        }

        public static ViewModelBase Contains(ViewModelBase item, double x, double y, double hitRadius)
        {
            switch (item)
            {
                case PointViewModel point:
                    {
                        var result = Contains(point, x, y, hitRadius);
                        if (result != null)
                        {
                            return result;
                        }
                    }
                    break;
                case LineShapeViewModel lineShape:
                    {
                        var resultStart = Contains(lineShape.Start, x, y, hitRadius);
                        if (resultStart != null)
                        {
                            return resultStart;
                        }

                        var resultEnd = Contains(lineShape.End, x, y, hitRadius);
                        if (resultEnd != null)
                        {
                            return resultEnd;
                        }

                        var bounds = GetBounds(lineShape);
                        var result = bounds.Contains((float)x, (float)y);
                        if (result)
                        {
                            return lineShape;
                        }
                    }
                    break;
                case RectangleShapeViewModel rectangleShape:
                    {
                        var resultTopLeft = Contains(rectangleShape.TopLeft, x, y, hitRadius);
                        if (resultTopLeft != null)
                        {
                            return resultTopLeft;
                        }

                        var resultBottomRight = Contains(rectangleShape.BottomRight, x, y, hitRadius);
                        if (resultBottomRight != null)
                        {
                            return resultBottomRight;
                        }

                        var bounds = GetBounds(rectangleShape);
                        var result = bounds.Contains((float)x, (float)y);
                        if (result)
                        {
                            return rectangleShape;
                        }
                    }
                    break;
            }

            return null;
        }

        public static ViewModelBase Contains(IList<ViewModelBase> items, double x, double y, double hitRadius)
        {
            foreach (var item in items)
            {
                var result = Contains(item, x, y, hitRadius);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        public static ViewModelBase Intersects(PointViewModel point, SKRect rect)
        {
            var result = rect.Contains(new SKPoint((float)point.X, (float)point.Y));
            if (result)
            {
                return point;
            }
            return null;
        }

        public static ViewModelBase Intersects(ViewModelBase shape, SKRect rect)
        {
            switch (shape)
            {
                case PointViewModel point:
                    {
                        var result = Intersects(point, rect);
                        if (result != null)
                        {
                            return result;
                        }
                    }
                    break;
                case LineShapeViewModel lineShape:
                    {
                        var bounds = GetBounds(lineShape);
                        var result = rect.IntersectsWith(bounds);
                        if (result)
                        {
                            return lineShape;
                        }
                    }
                    break;
                case RectangleShapeViewModel rectangleShape:
                    {
                        var bounds = GetBounds(rectangleShape);
                        var result = rect.IntersectsWith(bounds);
                        if (result)
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
