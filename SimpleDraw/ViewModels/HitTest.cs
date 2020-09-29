using System;
using System.Collections.Generic;
using SkiaSharp;

namespace SimpleDraw.ViewModels
{
    internal static class HitTest
    {
        //public static double PointBoundsRadius = 6;

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
                lineShape.StartPoint.X,
                lineShape.StartPoint.Y,
                lineShape.Point.X,
                lineShape.Point.Y);
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
            path.MoveTo(new SKPoint((float)lineShape.StartPoint.X, (float)lineShape.StartPoint.Y));
            path.LineTo(new SKPoint((float)lineShape.Point.X, (float)lineShape.Point.Y));
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

        public static SKRect GetBounds(IList<ViewModelBase> items)
        {
            var result = SKRect.Empty;
            var haveResult = false;

            for (int i = items.Count - 1; i >= 0; i--)
            {
                var item = items[i];

                switch (item)
                {
                    case PointViewModel point:
                        {
                            var bounds = SKRect.Create((float)point.X, (float)point.Y, 0, 0);
                            //var bounds = Expand(new SKPoint((float)point.X, (float)point.Y), PointBoundsRadius);
                            if (haveResult)
                            {
                                result.Union(bounds);
                            }
                            else
                            {
                                result = bounds;
                                haveResult = true;
                            }
                        }
                        break;
                    case GroupViewModel group:
                        {
                            var bounds = GetBounds(group.Items);
                            if (haveResult)
                            {
                                result.Union(bounds);
                            }
                            else
                            {
                                result = bounds;
                                haveResult = true;
                            }
                        }
                        break;
                    case LineShapeViewModel lineShape:
                        {
                            var bounds = GetBounds(lineShape);
                            if (haveResult)
                            {
                                result.Union(bounds);
                            }
                            else
                            {
                                result = bounds;
                                haveResult = true;
                            }
                        }
                        break;
                    case RectangleShapeViewModel rectangleShape:
                        {
                            var bounds = GetBounds(rectangleShape);
                            if (haveResult)
                            {
                                result.Union(bounds);
                            }
                            else
                            {
                                result = bounds;
                                haveResult = true;
                            }
                        }
                        break;
                }
            }

            return result;
        }

        public static ViewModelBase Contains(PointViewModel point, SKRect hitRect)
        {
            var result = hitRect.Contains((float)point.X, (float)point.Y);
            if (result)
            {
                return point;
            }
            return null;
        }

        public static ViewModelBase Contains(ViewModelBase item, SKRect hitRect)
        {
            switch (item)
            {
                case PointViewModel point:
                    {
                        var result = Contains(point, hitRect);
                        if (result != null)
                        {
                            return result;
                        }
                    }
                    break;
                case GroupViewModel group:
                    {
                        var bounds = GetBounds(group.Items);
                        var result = bounds.IntersectsWith(hitRect);
                        if (result)
                        {
                            return group;
                        }
                    }
                    break;
                case LineShapeViewModel lineShape:
                    {
                        var resultStart = Contains(lineShape.StartPoint, hitRect);
                        if (resultStart != null)
                        {
                            return resultStart;
                        }

                        var resultEnd = Contains(lineShape.Point, hitRect);
                        if (resultEnd != null)
                        {
                            return resultEnd;
                        }

                        var bounds = GetBounds(lineShape);
                        var result = bounds.IntersectsWith(hitRect);
                        if (result)
                        {
                            return lineShape;
                        }
                    }
                    break;
                case RectangleShapeViewModel rectangleShape:
                    {
                        var resultTopLeft = Contains(rectangleShape.TopLeft, hitRect);
                        if (resultTopLeft != null)
                        {
                            return resultTopLeft;
                        }

                        var resultBottomRight = Contains(rectangleShape.BottomRight, hitRect);
                        if (resultBottomRight != null)
                        {
                            return resultBottomRight;
                        }

                        var bounds = GetBounds(rectangleShape);
                        var result = bounds.IntersectsWith(hitRect);
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
            var hitRect = Expand(new SKPoint((float)x, (float)y), hitRadius);

            for (int i = items.Count - 1; i >= 0; i--)
            {
                var item = items[i];
                var result = Contains(item, hitRect);
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

        public static ViewModelBase Intersects(ViewModelBase item, SKRect rect)
        {
            switch (item)
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
                case GroupViewModel group:
                    {
                        var bounds = GetBounds(group.Items);
                        var result = rect.IntersectsWith(bounds);
                        if (result)
                        {
                            return group;
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
