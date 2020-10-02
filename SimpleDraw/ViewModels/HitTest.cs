﻿using System;
using System.Collections.Generic;
using SkiaSharp;

namespace SimpleDraw.ViewModels
{
    internal static class HitTest
    {
        public static SKRect Expand(SKPoint point, double radius)
        {
            return SKRect.Create(
                (float)(point.X - radius),
                (float)(point.Y - radius),
                (float)(radius + radius),
                (float)(radius + radius));
        }

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

        public static SKRect ToSKRect(EllipseShapeViewModel ellipseShape)
        {
            return ToSKRect(
                ellipseShape.TopLeft.X,
                ellipseShape.TopLeft.Y,
                ellipseShape.BottomRight.X,
                ellipseShape.BottomRight.Y);
        }

        public static SKPath ToPath(LineShapeViewModel lineShape)
        {
            var path = new SKPath() { FillType = SKPathFillType.Winding };
            path.MoveTo(new SKPoint((float)lineShape.StartPoint.X, (float)lineShape.StartPoint.Y));
            path.LineTo(new SKPoint((float)lineShape.Point.X, (float)lineShape.Point.Y));
            return path;
        }

        public static SKPath ToPath(CubicBezierShapeViewModel cubicBezierShape)
        {
            var path = new SKPath() { FillType = SKPathFillType.Winding };
            path.MoveTo(new SKPoint((float)cubicBezierShape.StartPoint.X, (float)cubicBezierShape.StartPoint.Y));
            path.CubicTo(
                new SKPoint((float)cubicBezierShape.Point1.X, (float)cubicBezierShape.Point1.Y),
                new SKPoint((float)cubicBezierShape.Point2.X, (float)cubicBezierShape.Point2.Y),
                new SKPoint((float)cubicBezierShape.Point3.X, (float)cubicBezierShape.Point3.Y));
            return path;
        }

        public static SKPath ToPath(QuadraticBezierShapeViewModel quadraticBezierShape)
        {
            var path = new SKPath() { FillType = SKPathFillType.Winding };
            path.MoveTo(new SKPoint((float)quadraticBezierShape.StartPoint.X, (float)quadraticBezierShape.StartPoint.Y));
            path.QuadTo(
                new SKPoint((float)quadraticBezierShape.Control.X, (float)quadraticBezierShape.Control.Y),
                new SKPoint((float)quadraticBezierShape.EndPoint.X, (float)quadraticBezierShape.EndPoint.Y));
            return path;
        }

        public static SKPath ToPath(PathShapeViewModel pathShape)
        {
            var path = new SKPath() { FillType = pathShape.FillRule == PathFillRule.EvenOdd ? SKPathFillType.EvenOdd : SKPathFillType.Winding };

            foreach (var figure in pathShape.Figures)
            {
                if (figure.Segments.Count > 0)
                {
                    for (int i = 0; i < figure.Segments.Count; i++)
                    {
                        var segment = figure.Segments[i];

                        switch (segment)
                        {
                            case LineShapeViewModel lineShape:
                                {
                                    if (i == 0)
                                    {
                                        path.MoveTo(new SKPoint((float)lineShape.StartPoint.X, (float)lineShape.StartPoint.Y));
                                    }
                                    path.LineTo(new SKPoint((float)lineShape.Point.X, (float)lineShape.Point.Y));
                                }
                                break;
                            case CubicBezierShapeViewModel cubicBezierShape:
                                {
                                    if (i == 0)
                                    {
                                        path.MoveTo(new SKPoint((float)cubicBezierShape.StartPoint.X, (float)cubicBezierShape.StartPoint.Y));
                                    }
                                    path.CubicTo(
                                        new SKPoint((float)cubicBezierShape.Point1.X, (float)cubicBezierShape.Point1.Y),
                                        new SKPoint((float)cubicBezierShape.Point2.X, (float)cubicBezierShape.Point2.Y),
                                        new SKPoint((float)cubicBezierShape.Point3.X, (float)cubicBezierShape.Point3.Y));
                                }
                                break;
                            case QuadraticBezierShapeViewModel quadraticBezierShape:
                                {
                                    if (i == 0)
                                    {
                                        path.MoveTo(new SKPoint((float)quadraticBezierShape.StartPoint.X, (float)quadraticBezierShape.StartPoint.Y));
                                    }
                                    path.QuadTo(
                                        new SKPoint((float)quadraticBezierShape.Control.X, (float)quadraticBezierShape.Control.Y),
                                        new SKPoint((float)quadraticBezierShape.EndPoint.X, (float)quadraticBezierShape.EndPoint.Y));
                                }
                                break;
                        }
                    }

                    if (figure.IsClosed)
                    {
                        path.Close();
                    }
                }
            }

            return path;
        }

        public static SKPath ToPath(RectangleShapeViewModel rectangleShape)
        {
            var rect = ToSKRect(rectangleShape);
            var path = new SKPath() { FillType = SKPathFillType.Winding };
            path.AddRect(rect);
            return path;
        }

        public static SKPath ToPath(EllipseShapeViewModel ellipseShape)
        {
            var rect = ToSKRect(ellipseShape);
            var path = new SKPath() { FillType = SKPathFillType.Winding };
            path.AddOval(rect);
            return path;
        }

        public static SKRect GetBounds(LineShapeViewModel lineShape)
        {
            var path = ToPath(lineShape);
            var bounds = path.ComputeTightBounds();
            return bounds;
        }

        public static SKRect GetBounds(CubicBezierShapeViewModel cubicBezierShape)
        {
            var path = ToPath(cubicBezierShape);
            var bounds = path.ComputeTightBounds();
            return bounds;
        }

        public static SKRect GetBounds(QuadraticBezierShapeViewModel quadraticBezierShape)
        {
            var path = ToPath(quadraticBezierShape);
            var bounds = path.ComputeTightBounds();
            return bounds;
        }

        public static SKRect GetBounds(PathShapeViewModel pathShape)
        {
            var path = ToPath(pathShape);
            var bounds = path.ComputeTightBounds();
            return bounds;
        }

        public static SKRect GetBounds(RectangleShapeViewModel rectangleShape)
        {
            var path = ToPath(rectangleShape);
            var bounds = path.ComputeTightBounds();
            return bounds;
        }

        public static SKRect GetBounds(EllipseShapeViewModel ellipseShape)
        {
            var path = ToPath(ellipseShape);
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
                    case CubicBezierShapeViewModel cubicBezierShape:
                        {
                            var bounds = GetBounds(cubicBezierShape);
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
                    case QuadraticBezierShapeViewModel quadraticBezierShape:
                        {
                            var bounds = GetBounds(quadraticBezierShape);
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
                    case PathShapeViewModel pathShape:
                        {
                            var bounds = GetBounds(pathShape);
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
                    case EllipseShapeViewModel ellipseShape:
                        {
                            var bounds = GetBounds(ellipseShape);
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
                        var resultStartPoint = Contains(lineShape.StartPoint, hitRect);
                        if (resultStartPoint != null)
                        {
                            return resultStartPoint;
                        }

                        var resultPoint = Contains(lineShape.Point, hitRect);
                        if (resultPoint != null)
                        {
                            return resultPoint;
                        }

                        var bounds = GetBounds(lineShape);
                        var result = bounds.IntersectsWith(hitRect);
                        if (result)
                        {
                            return lineShape;
                        }
                    }
                    break;
                case CubicBezierShapeViewModel cubicBezierShape:
                    {
                        var resultStartPoint = Contains(cubicBezierShape.StartPoint, hitRect);
                        if (resultStartPoint != null)
                        {
                            return resultStartPoint;
                        }

                        var resultPoint1 = Contains(cubicBezierShape.Point1, hitRect);
                        if (resultPoint1 != null)
                        {
                            return resultPoint1;
                        }

                        var resultPoint2 = Contains(cubicBezierShape.Point2, hitRect);
                        if (resultPoint2 != null)
                        {
                            return resultPoint2;
                        }

                        var resultPoint3 = Contains(cubicBezierShape.Point3, hitRect);
                        if (resultPoint3 != null)
                        {
                            return resultPoint3;
                        }

                        var bounds = GetBounds(cubicBezierShape);
                        var result = bounds.IntersectsWith(hitRect);
                        if (result)
                        {
                            return cubicBezierShape;
                        }
                    }
                    break;
                case QuadraticBezierShapeViewModel quadraticBezierShape:
                    {
                        var resultStartPoint = Contains(quadraticBezierShape.StartPoint, hitRect);
                        if (resultStartPoint != null)
                        {
                            return resultStartPoint;
                        }

                        var resultControl = Contains(quadraticBezierShape.Control, hitRect);
                        if (resultControl != null)
                        {
                            return resultControl;
                        }

                        var resultEndPoint = Contains(quadraticBezierShape.EndPoint, hitRect);
                        if (resultEndPoint != null)
                        {
                            return resultEndPoint;
                        }

                        var bounds = GetBounds(quadraticBezierShape);
                        var result = bounds.IntersectsWith(hitRect);
                        if (result)
                        {
                            return quadraticBezierShape;
                        }
                    }
                    break;
                case PathShapeViewModel pathShape:
                    {
                        foreach (var figure in pathShape.Figures)
                        {
                            foreach (var segment in figure.Segments)
                            {
                                var segmentResult = Contains(segment, hitRect);
                                if (segmentResult is PointViewModel resultPoint)
                                {
                                    return resultPoint;
                                }
#if false
                                else if (segmentResult != null)
                                {
                                    return segmentResult;
                                }
#endif
                            }
                        }

                        var bounds = GetBounds(pathShape);
                        var result = bounds.IntersectsWith(hitRect);
                        if (result)
                        {
                            return pathShape;
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
                case EllipseShapeViewModel ellipseShape:
                    {
                        var resultTopLeft = Contains(ellipseShape.TopLeft, hitRect);
                        if (resultTopLeft != null)
                        {
                            return resultTopLeft;
                        }

                        var resultBottomRight = Contains(ellipseShape.BottomRight, hitRect);
                        if (resultBottomRight != null)
                        {
                            return resultBottomRight;
                        }

                        var bounds = GetBounds(ellipseShape);
                        var result = bounds.IntersectsWith(hitRect);
                        if (result)
                        {
                            return ellipseShape;
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
                case CubicBezierShapeViewModel cubicBezierShape:
                    {
                        var bounds = GetBounds(cubicBezierShape);
                        var result = rect.IntersectsWith(bounds);
                        if (result)
                        {
                            return cubicBezierShape;
                        }
                    }
                    break;
                case QuadraticBezierShapeViewModel quadraticBezierShape:
                    {
                        var bounds = GetBounds(quadraticBezierShape);
                        var result = rect.IntersectsWith(bounds);
                        if (result)
                        {
                            return quadraticBezierShape;
                        }
                    }
                    break;
                case PathShapeViewModel pathShape:
                    {
                        var bounds = GetBounds(pathShape);
                        var result = rect.IntersectsWith(bounds);
                        if (result)
                        {
                            return pathShape;
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
                case EllipseShapeViewModel ellipseShape:
                    {
                        var bounds = GetBounds(ellipseShape);
                        var result = rect.IntersectsWith(bounds);
                        if (result)
                        {
                            return ellipseShape;
                        }
                    }
                    break;
            }

            return null;
        }
    }
}
