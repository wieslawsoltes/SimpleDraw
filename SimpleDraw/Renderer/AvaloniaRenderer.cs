using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SimpleDraw.ViewModels;
using SimpleDraw.ViewModels.Containers;
using SimpleDraw.ViewModels.Media;
using SimpleDraw.ViewModels.Primitives;
using SimpleDraw.ViewModels.Shapes;
using A = Avalonia;
using AM = Avalonia.Media;
using AMI = Avalonia.Media.Immutable;
using FillRule = SimpleDraw.ViewModels.Shapes.FillRule;
using GradientSpreadMethod = SimpleDraw.ViewModels.Media.GradientSpreadMethod;
using PenLineCap = SimpleDraw.ViewModels.Media.PenLineCap;
using PenLineJoin = SimpleDraw.ViewModels.Media.PenLineJoin;
using RelativeUnit = SimpleDraw.ViewModels.Media.RelativeUnit;

namespace SimpleDraw.Renderer;

internal static class AvaloniaRenderer
{
    public static BrushViewModel PointBrush = new SolidColorBrushViewModel(new ColorViewModel(128, 255, 0, 0));

    public static PenViewModel PointPen = new PenViewModel(new SolidColorBrushViewModel(new ColorViewModel(128, 255, 0, 0)), 1);

    public static double PointRadius = 4;

    public static AM.Color ToColor(ColorViewModel color)
    {
        return new AM.Color(color.A, color.R, color.G, color.B);
    }

    public static AMI.ImmutableGradientStop ToGradientStop(GradientStopViewModel gradientStop)
    {
        if (gradientStop == null)
        {
            return default;
        }
        return new AMI.ImmutableGradientStop(gradientStop.Offset, ToColor(gradientStop.Color));
    }

    public static IReadOnlyList<AMI.ImmutableGradientStop> ToGradientStops(ObservableCollection<GradientStopViewModel> gradientStops)
    {
        var result = new List<AMI.ImmutableGradientStop>();
        foreach (var gradientStop in gradientStops)
        {
            result.Add(ToGradientStop(gradientStop));
        }
        return result;
    }

    public static AM.GradientSpreadMethod ToGradientSpreadMethod(GradientSpreadMethod spreadMethod)
    {
        return spreadMethod switch
        {
            GradientSpreadMethod.Reflect => AM.GradientSpreadMethod.Reflect,
            GradientSpreadMethod.Repeat => AM.GradientSpreadMethod.Repeat,
            _ => AM.GradientSpreadMethod.Pad,
        };
    }

    public static A.Point ToPoint(PointViewModel point)
    {
        if (point == null)
        {
            return default;
        }
        return new A.Point(point.X, point.Y);
    }

    public static A.RelativeUnit ToRelativeUnit(RelativeUnit relativeUnit)
    {
        return relativeUnit switch
        {
            RelativeUnit.Absolute => A.RelativeUnit.Absolute,
            _ => A.RelativeUnit.Relative,
        };
    }

    public static A.RelativePoint ToRelativePoint(RelativePointViewModel relativePoint)
    {
        if (relativePoint == null)
        {
            return default;
        }
        var point = ToPoint(relativePoint.Point);
        var unit = ToRelativeUnit(relativePoint.Unit);
        return new A.RelativePoint(point, unit);
    }

    public static AM.IBrush ToBrush(BrushViewModel brush)
    {
        switch (brush)
        {
            case SolidColorBrushViewModel solidColorBrush:
            {
                var color = ToColor(solidColorBrush.Color);
                return new AMI.ImmutableSolidColorBrush(color);
            }
            case LinearGradientBrushViewModel linearGradientBrush:
            {
                var gradientStops = ToGradientStops(linearGradientBrush.GradientStops);
                var spreadMethod = ToGradientSpreadMethod(linearGradientBrush.SpreadMethod);
                var startPoint = ToRelativePoint(linearGradientBrush.StartPoint);
                var endPoint = ToRelativePoint(linearGradientBrush.EndPoint);
                return new AMI.ImmutableLinearGradientBrush(gradientStops, 1, spreadMethod, startPoint, endPoint);
            }
            case RadialGradientBrushViewModel radialGradientBrush:
            {
                var gradientStops = ToGradientStops(radialGradientBrush.GradientStops);
                var spreadMethod = ToGradientSpreadMethod(radialGradientBrush.SpreadMethod);
                var center = ToRelativePoint(radialGradientBrush.Center);
                var gradientOrigin = ToRelativePoint(radialGradientBrush.GradientOrigin);
                return new AMI.ImmutableRadialGradientBrush(gradientStops, 1, spreadMethod, center, gradientOrigin, radialGradientBrush.Radius);
            }
            default:
                return default;
        }
    }

    public static AMI.ImmutableDashStyle ToDashStyle(DashStyleViewModel dashStyle)
    {
        if (dashStyle == null || dashStyle.Dashes == null)
        {
            return default;
        }
        return new AMI.ImmutableDashStyle(dashStyle.Dashes, dashStyle.Offset);
    }

    public static AM.PenLineCap ToPenLineCap(PenLineCap lineCap)
    {
        return lineCap switch
        {
            PenLineCap.Round => AM.PenLineCap.Round,
            PenLineCap.Square => AM.PenLineCap.Square,
            _ => AM.PenLineCap.Flat,
        };
    }

    public static AM.PenLineJoin ToPenLineJoin(PenLineJoin lineJoin)
    {
        return lineJoin switch
        {
            PenLineJoin.Bevel => AM.PenLineJoin.Bevel,
            PenLineJoin.Round => AM.PenLineJoin.Round,
            _ => AM.PenLineJoin.Miter,
        };
    }

    public static AMI.ImmutablePen ToPen(PenViewModel pen)
    {
        if (pen == null)
        {
            return default;
        }
        var brush = ToBrush(pen.Brush);
        var dashStyle = ToDashStyle(pen.DashStyle);
        var lineCap = ToPenLineCap(pen.LineCap);
        var lineJoin = ToPenLineJoin(pen.LineJoin);
        return new AMI.ImmutablePen(brush, pen.Thickness, dashStyle, lineCap, lineJoin, pen.MiterLimit);
    }

    public static A.Rect ToRect(RectangleShapeViewModel rectangleShape)
    {
        var x = Math.Min(rectangleShape.TopLeft.X, rectangleShape.BottomRight.X);
        var y = Math.Min(rectangleShape.TopLeft.Y, rectangleShape.BottomRight.Y);
        var width = Math.Abs(rectangleShape.TopLeft.X - rectangleShape.BottomRight.X);
        var height = Math.Abs(rectangleShape.TopLeft.Y - rectangleShape.BottomRight.Y);
        return new A.Rect(x, y, width, height);
    }

    public static A.Rect ToRect(EllipseShapeViewModel ellipseShape)
    {
        var x = Math.Min(ellipseShape.TopLeft.X, ellipseShape.BottomRight.X);
        var y = Math.Min(ellipseShape.TopLeft.Y, ellipseShape.BottomRight.Y);
        var width = Math.Abs(ellipseShape.TopLeft.X - ellipseShape.BottomRight.X);
        var height = Math.Abs(ellipseShape.TopLeft.Y - ellipseShape.BottomRight.Y);
        return new A.Rect(x, y, width, height);
    }

    public static AM.Geometry ToGeometry(CubicBezierShapeViewModel cubicBezierShape)
    {
        var geometry = new AM.StreamGeometry();
        using var geometryContext = geometry.Open();
        geometryContext.SetFillRule(AM.FillRule.EvenOdd);
        geometryContext.BeginFigure(ToPoint(cubicBezierShape.StartPoint), true);
        geometryContext.CubicBezierTo(
            ToPoint(cubicBezierShape.Point1),
            ToPoint(cubicBezierShape.Point2),
            ToPoint(cubicBezierShape.Point3));
        geometryContext.EndFigure(false);
        return geometry;
    }

    public static AM.Geometry ToGeometry(QuadraticBezierShapeViewModel quadraticBezierShape)
    {
        var geometry = new AM.StreamGeometry();
        using var geometryContext = geometry.Open();
        geometryContext.SetFillRule(AM.FillRule.EvenOdd);
        geometryContext.BeginFigure(ToPoint(quadraticBezierShape.StartPoint), true);
        geometryContext.QuadraticBezierTo(
            ToPoint(quadraticBezierShape.Control),
            ToPoint(quadraticBezierShape.EndPoint));
        geometryContext.EndFigure(false);
        return geometry;
    }

    public static AM.Geometry ToGeometry(PathShapeViewModel pathShape)
    {
        var geometry = new AM.StreamGeometry();

        using var geometryContext = geometry.Open();

        geometryContext.SetFillRule(pathShape.FillRule == FillRule.EvenOdd ? AM.FillRule.EvenOdd : AM.FillRule.NonZero);

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
                                geometryContext.BeginFigure(ToPoint(lineShape.StartPoint), true);
                            }
                            geometryContext.LineTo(ToPoint(lineShape.Point));
                        }
                            break;
                        case CubicBezierShapeViewModel cubicBezierShape:
                        {
                            if (i == 0)
                            {
                                geometryContext.BeginFigure(ToPoint(cubicBezierShape.StartPoint), true);
                            }
                            geometryContext.CubicBezierTo(
                                ToPoint(cubicBezierShape.Point1),
                                ToPoint(cubicBezierShape.Point2),
                                ToPoint(cubicBezierShape.Point3));
                        }
                            break;
                        case QuadraticBezierShapeViewModel quadraticBezierShape:
                        {
                            if (i == 0)
                            {
                                geometryContext.BeginFigure(ToPoint(quadraticBezierShape.StartPoint), true);
                            }
                            geometryContext.QuadraticBezierTo(
                                ToPoint(quadraticBezierShape.Control),
                                ToPoint(quadraticBezierShape.EndPoint));
                        }
                            break;
                    }
                }

                if (figure.IsClosed)
                {
                    geometryContext.EndFigure(figure.IsClosed);
                }
            }
        }

        return geometry;
    }

    public static AM.Geometry ToGeometry(RectangleShapeViewModel rectangleShape)
    {
        var rect = ToRect(rectangleShape);
        var geometry = new AM.RectangleGeometry(rect);
        return geometry;
    }

    public static AM.Geometry ToGeometry(EllipseShapeViewModel ellipseShape)
    {
        var rect = ToRect(ellipseShape);
        var geometry = new AM.EllipseGeometry(rect);
        return geometry;
    }

    public static void Render(AM.DrawingContext context, LineShapeViewModel lineShape)
    {
        if (lineShape.IsStroked)
        {
            var p1 = ToPoint(lineShape.StartPoint);
            var p2 = ToPoint(lineShape.Point);
            var pen = ToPen(lineShape.Pen);
            context.DrawLine(pen, p1, p2);
        }
    }

    public static void Render(AM.DrawingContext context, CubicBezierShapeViewModel cubicBezierShape)
    {
        if (cubicBezierShape.IsStroked || cubicBezierShape.IsFilled)
        {
            var geometry = ToGeometry(cubicBezierShape);
            var brush = ToBrush(cubicBezierShape.Brush);
            var pen = ToPen(cubicBezierShape.Pen);
            context.DrawGeometry(
                cubicBezierShape.IsFilled ? brush : default,
                cubicBezierShape.IsStroked ? pen : default(AM.IPen),
                geometry);
        }
    }

    public static void Render(AM.DrawingContext context, QuadraticBezierShapeViewModel quadraticBezierShape)
    {
        if (quadraticBezierShape.IsStroked || quadraticBezierShape.IsFilled)
        {
            var geometry = ToGeometry(quadraticBezierShape);
            var brush = ToBrush(quadraticBezierShape.Brush);
            var pen = ToPen(quadraticBezierShape.Pen);
            context.DrawGeometry(
                quadraticBezierShape.IsFilled ? brush : default,
                quadraticBezierShape.IsStroked ? pen : default(AM.IPen),
                geometry);
        }
    }

    public static void Render(AM.DrawingContext context, RectangleShapeViewModel rectangleShape)
    {
        if (rectangleShape.IsStroked || rectangleShape.IsFilled)
        {
            var rect = ToRect(rectangleShape);
            var brush = ToBrush(rectangleShape.Brush);
            var pen = ToPen(rectangleShape.Pen);
            context.DrawRectangle(
                rectangleShape.IsFilled ? brush : default,
                rectangleShape.IsStroked ? pen : default(AM.IPen),
                rect,
                rectangleShape.RadiusX,
                rectangleShape.RadiusY);
        }
    }

    public static void Render(AM.DrawingContext context, EllipseShapeViewModel ellipseShape)
    {
        if (ellipseShape.IsStroked || ellipseShape.IsFilled)
        {
            var geometry = ToGeometry(ellipseShape);
            var brush = ToBrush(ellipseShape.Brush);
            var pen = ToPen(ellipseShape.Pen);
            context.DrawGeometry(
                ellipseShape.IsFilled ? brush : default,
                ellipseShape.IsStroked ? pen : default(AM.IPen),
                geometry);
        }
    }

    public static void Render(AM.DrawingContext context, PathShapeViewModel pathShape)
    {
        if (pathShape.IsStroked || pathShape.IsFilled)
        {
            var geometry = ToGeometry(pathShape);
            var brush = ToBrush(pathShape.Brush);
            var pen = ToPen(pathShape.Pen);
            context.DrawGeometry(
                pathShape.IsFilled ? brush : default,
                pathShape.IsStroked ? pen : default(AM.IPen),
                geometry);
        }
    }

    public static void Render(AM.DrawingContext context, PointViewModel point)
    {
        var rect = new A.Rect(
            point.X - PointRadius,
            point.Y - PointRadius,
            PointRadius + PointRadius,
            PointRadius + PointRadius);
        var geometry = new AM.EllipseGeometry(rect);
        var brush = ToBrush(PointBrush);
        var pen = ToPen(PointPen);
        context.DrawGeometry(brush, pen, geometry);
    }

    public static void Render(AM.DrawingContext context, ShapeBaseViewModel shape)
    {
        switch (shape)
        {
            case LineShapeViewModel lineShape:
            {
                Render(context, lineShape);
            }
                break;
            case CubicBezierShapeViewModel cubicBezierShape:
            {
                Render(context, cubicBezierShape);
            }
                break;
            case QuadraticBezierShapeViewModel quadraticBezierShape:
            {
                Render(context, quadraticBezierShape);
            }
                break;
            case PathShapeViewModel pathShape:
            {
                Render(context, pathShape);
            }
                break;
            case RectangleShapeViewModel rectangleShape:
            {
                Render(context, rectangleShape);
            }
                break;
            case EllipseShapeViewModel ellipseShape:
            {
                Render(context, ellipseShape);
            }
                break;
            default:
                break;
        }
    }

    public static void Render(AM.DrawingContext context, ObservableCollection<ViewModelBase> items)
    {
        foreach (var item in items)
        {
            switch (item)
            {
                case PointViewModel point:
                {
                    Render(context, point);
                }
                    break;
                case GroupViewModel group:
                {
                    Render(context, group.Items);
                }
                    break;
                case ShapeBaseViewModel shape:
                {
                    Render(context, shape);
                }
                    break;
            }
        }
    }

    public static void RenderHovered(AM.DrawingContext context, ObservableCollection<ViewModelBase> items)
    {
        var points = new HashSet<PointViewModel>();

        CanvasViewModel.GetPoints(items, points);

        foreach (var point in points)
        {
            Render(context, point);
        }
    }

    public static void Render(AM.DrawingContext context, CanvasViewModel canvas)
    {
        Render(context, canvas.Items);
        RenderHovered(context, canvas.Hovered);
        Render(context, canvas.Decorators);
    }
}
