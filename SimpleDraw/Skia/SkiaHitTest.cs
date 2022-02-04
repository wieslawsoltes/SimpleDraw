using System;
using System.Collections.Generic;
using SimpleDraw.ViewModels;
using SimpleDraw.ViewModels.Containers;
using SimpleDraw.ViewModels.Primitives;
using SimpleDraw.ViewModels.Shapes;
using SkiaSharp;

namespace SimpleDraw.Skia;

internal static class SkiaHitTest
{
    public static SKRect GetBounds(LineShapeViewModel lineShape)
    {
        var path = SkiaRenderer.ToSKPath(lineShape);
        var bounds = path.ComputeTightBounds();
        return bounds;
    }

    public static SKRect GetBounds(CubicBezierShapeViewModel cubicBezierShape)
    {
        var path = SkiaRenderer.ToSKPath(cubicBezierShape);
        var bounds = path.ComputeTightBounds();
        return bounds;
    }

    public static SKRect GetBounds(QuadraticBezierShapeViewModel quadraticBezierShape)
    {
        var path = SkiaRenderer.ToSKPath(quadraticBezierShape);
        var bounds = path.ComputeTightBounds();
        return bounds;
    }

    public static SKRect GetBounds(PathShapeViewModel pathShape)
    {
        var path = SkiaRenderer.ToSKPath(pathShape);
        var bounds = path.ComputeTightBounds();
        return bounds;
    }

    public static SKRect GetBounds(RectangleShapeViewModel rectangleShape)
    {
        var path = SkiaRenderer.ToSKPath(rectangleShape);
        var bounds = path.ComputeTightBounds();
        return bounds;
    }

    public static SKRect GetBounds(EllipseShapeViewModel ellipseShape)
    {
        var path = SkiaRenderer.ToSKPath(ellipseShape);
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

    public static SKRect Expand(SKPoint point, double radius)
    {
        return SKRect.Create(
            (float)(point.X - radius),
            (float)(point.Y - radius),
            (float)(radius + radius),
            (float)(radius + radius));
    }

    private static ViewModelBase Contains(PointViewModel point, SKRect hitRect)
    {
        var result = hitRect.Contains((float)point.X, (float)point.Y);
        if (result)
        {
            return point;
        }
        return null;
    }

    private static ViewModelBase Contains(ViewModelBase item, Predicate<ViewModelBase> filter, SKRect hitRect)
    {
        if (!filter(item))
        {
            return null;
        }

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
                        var segmentResult = Contains(segment, filter, hitRect);
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

    public static ViewModelBase Contains(IList<ViewModelBase> items, Predicate<ViewModelBase> filter, double x, double y, double hitRadius)
    {
        var hitRect = Expand(new SKPoint((float)x, (float)y), hitRadius);

        for (int i = items.Count - 1; i >= 0; i--)
        {
            var item = items[i];
            var result = Contains(item, filter, hitRect);
            if (result != null && filter(result))
            {
                return result;
            }
        }
        return null;
    }

    private static bool Intersects(PointViewModel point, SKRect rect)
    {
        var result = rect.Contains(new SKPoint((float)point.X, (float)point.Y));
        if (result)
        {
            return true;
        }
        return false;
    }

    public static ViewModelBase Intersects(ViewModelBase item, Predicate<ViewModelBase> filter, SKRect rect)
    {
        if (!filter(item))
        {
            return null;
        }

        switch (item)
        {
            case PointViewModel point:
            {
                var result = Intersects(point, rect);
                if (result)
                {
                    return point;
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