using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using SimpleDraw.ViewModels;

namespace SimpleDraw.Renderer
{
    internal static class AvaloniaRenderer
    {
        public static Color ToColor(ColorViewModel color)
        {
            return new Color(color.A, color.R, color.G, color.B);
        }

        public static ImmutableGradientStop ToGradientStop(GradientStopViewModel gradientStop)
        {
            if (gradientStop == null)
            {
                return default;
            }
            return new ImmutableGradientStop(gradientStop.Offset, ToColor(gradientStop.Color));
        }

        public static IReadOnlyList<ImmutableGradientStop> ToGradientStops(ObservableCollection<GradientStopViewModel> gradientStops)
        {
            var result = new List<ImmutableGradientStop>();
            foreach (var gradientStop in gradientStops)
            {
                result.Add(ToGradientStop(gradientStop));
            }
            return result;
        }

        public static Avalonia.Media.GradientSpreadMethod ToGradientSpreadMethod(ViewModels.GradientSpreadMethod spreadMethod)
        {
            return spreadMethod switch
            {
                ViewModels.GradientSpreadMethod.Reflect => Avalonia.Media.GradientSpreadMethod.Reflect,
                ViewModels.GradientSpreadMethod.Repeat => Avalonia.Media.GradientSpreadMethod.Repeat,
                _ => Avalonia.Media.GradientSpreadMethod.Pad,
            };
        }

        public static Point ToPoint(PointViewModel point)
        {
            if (point == null)
            {
                return default;
            }
            return new Point(point.X, point.Y);
        }

        public static Avalonia.RelativeUnit ToRelativeUnit(ViewModels.RelativeUnit relativeUnit)
        {
            return relativeUnit switch
            {
                ViewModels.RelativeUnit.Absolute => Avalonia.RelativeUnit.Absolute,
                _ => Avalonia.RelativeUnit.Relative,
            };
        }

        public static RelativePoint ToRelativePoint(RelativePointViewModel relativePoint)
        {
            if (relativePoint == null)
            {
                return default;
            }
            var point = ToPoint(relativePoint.Point);
            var unit = ToRelativeUnit(relativePoint.Unit);
            return new RelativePoint(point, unit);
        }

        public static IBrush ToBrush(BrushViewModel brush)
        {
            switch (brush)
            {
                case SolidColorBrushViewModel solidColorBrush:
                    {
                        var color = ToColor(solidColorBrush.Color);
                        return new ImmutableSolidColorBrush(color);
                    }
                case LinearGradientBrushViewModel linearGradientBrush:
                    {
                        var gradientStops = ToGradientStops(linearGradientBrush.GradientStops);
                        var spreadMethod = ToGradientSpreadMethod(linearGradientBrush.SpreadMethod);
                        var startPoint = ToRelativePoint(linearGradientBrush.StartPoint);
                        var endPoint = ToRelativePoint(linearGradientBrush.EndPoint);
                        return new ImmutableLinearGradientBrush(gradientStops, 1, spreadMethod, startPoint, endPoint);
                    }
                case RadialGradientBrushViewModel radialGradientBrush:
                    {
                        var gradientStops = ToGradientStops(radialGradientBrush.GradientStops);
                        var spreadMethod = ToGradientSpreadMethod(radialGradientBrush.SpreadMethod);
                        var center = ToRelativePoint(radialGradientBrush.Center);
                        var gradientOrigin = ToRelativePoint(radialGradientBrush.GradientOrigin);
                        return new ImmutableRadialGradientBrush(gradientStops, 1, spreadMethod, center, gradientOrigin, radialGradientBrush.Radius);
                    }
                default:
                    return default;
            }
        }

        public static ImmutableDashStyle ToDashStyle(DashStyleViewModel dashStyle)
        {
            if (dashStyle == null || dashStyle.Dashes == null)
            {
                return default;
            }
            return new ImmutableDashStyle(dashStyle.Dashes, dashStyle.Offset);
        }

        public static Avalonia.Media.PenLineCap ToPenLineCap(ViewModels.PenLineCap lineCap)
        {
            return lineCap switch
            {
                ViewModels.PenLineCap.Round => Avalonia.Media.PenLineCap.Round,
                ViewModels.PenLineCap.Square => Avalonia.Media.PenLineCap.Square,
                _ => Avalonia.Media.PenLineCap.Flat,
            };
        }

        public static Avalonia.Media.PenLineJoin ToPenLineJoin(ViewModels.PenLineJoin lineJoin)
        {
            return lineJoin switch
            {
                ViewModels.PenLineJoin.Bevel => Avalonia.Media.PenLineJoin.Bevel,
                ViewModels.PenLineJoin.Round => Avalonia.Media.PenLineJoin.Round,
                _ => Avalonia.Media.PenLineJoin.Miter,
            };
        }

        public static ImmutablePen ToPen(PenViewModel pen)
        {
            if (pen == null)
            {
                return default;
            }
            var brush = ToBrush(pen.Brush);
            var dashStyle = ToDashStyle(pen.DashStyle);
            var lineCap = ToPenLineCap(pen.LineCap);
            var lineJoin = ToPenLineJoin(pen.LineJoin);
            return new ImmutablePen(brush, pen.Thickness, dashStyle, lineCap, lineJoin, pen.MiterLimit);
        }

        public static Rect ToRect(RectangleShapeViewModel rectangleShape)
        {
            var x = Math.Min(rectangleShape.TopLeft.X, rectangleShape.BottomRight.X);
            var y = Math.Min(rectangleShape.TopLeft.Y, rectangleShape.BottomRight.Y);
            var width = Math.Abs(rectangleShape.TopLeft.X - rectangleShape.BottomRight.X);
            var height = Math.Abs(rectangleShape.TopLeft.Y - rectangleShape.BottomRight.Y);
            return new Rect(x, y, width, height);
        }

        public static Geometry ToGeometry(CubicBezierShapeViewModel cubicBezierShape)
        {
            var geometry = new StreamGeometry();
            using var geometryContext = geometry.Open();
            geometryContext.SetFillRule(FillRule.EvenOdd);
            geometryContext.BeginFigure(ToPoint(cubicBezierShape.StartPoint), true);
            geometryContext.CubicBezierTo(
                ToPoint(cubicBezierShape.Point1),
                ToPoint(cubicBezierShape.Point2),
                ToPoint(cubicBezierShape.Point3));
            geometryContext.EndFigure(false);
            return geometry;
        }

        public static Geometry ToGeometry(QuadraticBezierShapeViewModel quadraticBezierShape)
        {
            var geometry = new StreamGeometry();
            using var geometryContext = geometry.Open();
            geometryContext.SetFillRule(FillRule.EvenOdd);
            geometryContext.BeginFigure(ToPoint(quadraticBezierShape.StartPoint), true);
            geometryContext.QuadraticBezierTo(
                ToPoint(quadraticBezierShape.Control),
                ToPoint(quadraticBezierShape.EndPoint));
            geometryContext.EndFigure(false);
            return geometry;
        }

        public static Geometry ToGeometry(PathShapeViewModel pathShape)
        {
            var geometry = new StreamGeometry();

            using var geometryContext = geometry.Open();

            geometryContext.SetFillRule(pathShape.FillRule == PathFillRule.EvenOdd ? FillRule.EvenOdd : FillRule.NonZero);

            foreach (var figure in pathShape.Figures)
            {
                if (figure.Segments.Count > 0)
                {
                    var firstSegment = figure.Segments[0];

                    switch (firstSegment)
                    {
                        case LineShapeViewModel lineShape:
                            {
                                geometryContext.BeginFigure(ToPoint(lineShape.StartPoint), true);
                            }
                            break;
                        case CubicBezierShapeViewModel cubicBezierShape:
                            {
                                geometryContext.BeginFigure(ToPoint(cubicBezierShape.StartPoint), true);
                            }
                            break;
                        case QuadraticBezierShapeViewModel quadraticBezierShape:
                            {
                                geometryContext.BeginFigure(ToPoint(quadraticBezierShape.StartPoint), true);
                            }
                            break;
                    }

                    for (int i = 1; i < figure.Segments.Count; i++)
                    {
                        var nextSegment = figure.Segments[i];

                        switch (nextSegment)
                        {
                            case LineShapeViewModel lineShape:
                                {
                                    geometryContext.LineTo(ToPoint(lineShape.Point));
                                }
                                break;
                            case CubicBezierShapeViewModel cubicBezierShape:
                                {
                                    geometryContext.CubicBezierTo(
                                        ToPoint(cubicBezierShape.Point1),
                                        ToPoint(cubicBezierShape.Point2),
                                        ToPoint(cubicBezierShape.Point3));
                                }
                                break;
                            case QuadraticBezierShapeViewModel quadraticBezierShape:
                                {
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

        public static void Render(DrawingContext context, LineShapeViewModel lineShape)
        {
            if (lineShape.IsStroked)
            {
                var p1 = new Point(lineShape.StartPoint.X, lineShape.StartPoint.Y);
                var p2 = new Point(lineShape.Point.X, lineShape.Point.Y);
                var pen = ToPen(lineShape.Pen);
                context.DrawLine(pen, p1, p2);
            }
        }

        public static void Render(DrawingContext context, CubicBezierShapeViewModel cubicBezierShape)
        {
            if (cubicBezierShape.IsStroked || cubicBezierShape.IsFilled)
            {
                var geometry = ToGeometry(cubicBezierShape);
                var brush = ToBrush(cubicBezierShape.Brush);
                var pen = ToPen(cubicBezierShape.Pen);
                context.DrawGeometry(
                    cubicBezierShape.IsFilled ? brush : default,
                    cubicBezierShape.IsStroked ? pen : default(IPen),
                    geometry);
            }
        }

        public static void Render(DrawingContext context, QuadraticBezierShapeViewModel quadraticBezierShape)
        {
            if (quadraticBezierShape.IsStroked || quadraticBezierShape.IsFilled)
            {
                var geometry = ToGeometry(quadraticBezierShape);
                var brush = ToBrush(quadraticBezierShape.Brush);
                var pen = ToPen(quadraticBezierShape.Pen);
                context.DrawGeometry(
                    quadraticBezierShape.IsFilled ? brush : default,
                    quadraticBezierShape.IsStroked ? pen : default(IPen),
                    geometry);
            }
        }

        public static void Render(DrawingContext context, RectangleShapeViewModel rectangleShape)
        {
            if (rectangleShape.IsStroked || rectangleShape.IsFilled)
            {
                var rect = ToRect(rectangleShape);
                var brush = ToBrush(rectangleShape.Brush);
                var pen = ToPen(rectangleShape.Pen);
                context.DrawRectangle(
                    rectangleShape.IsFilled ? brush : default,
                    rectangleShape.IsStroked ? pen : default(IPen),
                    rect,
                    rectangleShape.RadiusX,
                    rectangleShape.RadiusY);
            }
        }

        public static void Render(DrawingContext context, PathShapeViewModel pathShape)
        {
            if (pathShape.IsStroked || pathShape.IsFilled)
            {
                var geometry = ToGeometry(pathShape);
                var brush = ToBrush(pathShape.Brush);
                var pen = ToPen(pathShape.Pen);
                context.DrawGeometry(
                    pathShape.IsFilled ? brush : default,
                    pathShape.IsStroked ? pen : default(IPen),
                    geometry);
            }
        }

        public static void Render(DrawingContext context, ShapeBaseViewModel shape)
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
                case RectangleShapeViewModel rectangleShape:
                    {
                        Render(context, rectangleShape);
                    }
                    break;
                case PathShapeViewModel pathShape:
                    {
                        Render(context, pathShape);
                    }
                    break;
                default:
                    break;
            }
        }

        public static void Render(DrawingContext context, ObservableCollection<ViewModelBase> items)
        {
            foreach (var item in items)
            {
                switch (item)
                {
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

        public static void Render(DrawingContext context, CanvasViewModel canvas)
        {
            Render(context, canvas.Items);
            Render(context, canvas.Decorators);
        }
    }
}
