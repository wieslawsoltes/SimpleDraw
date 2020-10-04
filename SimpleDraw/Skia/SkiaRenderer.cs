using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SimpleDraw.ViewModels;
using SimpleDraw.ViewModels.Containers;
using SimpleDraw.ViewModels.Media;
using SimpleDraw.ViewModels.Primitives;
using SimpleDraw.ViewModels.Shapes;
using SkiaSharp;

namespace SimpleDraw.Skia
{
    internal static class SkiaRenderer
    {
        public static SKColor ToSKColor(ColorViewModel color)
        {
            return new SKColor(color.R, color.G, color.B, color.A);
        }

        public static SKColor[] ToSKColors(ObservableCollection<GradientStopViewModel> gradientStops)
        {
            var result = new SKColor[gradientStops.Count];
            for (int i = 0; i < gradientStops.Count; i++)
            {
                var gradientStop = gradientStops[i];
                result[i] = ToSKColor(gradientStop.Color);
            }
            return result;
        }

        public static float[] ToSKColorPos(ObservableCollection<GradientStopViewModel> gradientStops)
        {
            var result = new float[gradientStops.Count];
            for (int i = 0; i < gradientStops.Count; i++)
            {
                var gradientStop = gradientStops[i];
                result[i] = (float)gradientStop.Offset;
            }
            return result;
        }

        public static SKShaderTileMode ToSKShaderTileMode(GradientSpreadMethod spreadMethod)
        {
            return spreadMethod switch
            {
                GradientSpreadMethod.Reflect => SKShaderTileMode.Mirror,
                GradientSpreadMethod.Repeat => SKShaderTileMode.Repeat,
                _ => SKShaderTileMode.Clamp,
            };
        }

        public static SKPoint ToSKPoint(PointViewModel point)
        {
            if (point == null)
            {
                return default;
            }
            return new SKPoint((float)point.X, (float)point.Y);
        }

        public static SKPoint ToSKPoint(RelativePointViewModel relativePoint)
        {
            if (relativePoint == null)
            {
                return default;
            }
            var point = ToSKPoint(relativePoint.Point);
            return relativePoint.Unit switch
            {
                RelativeUnit.Absolute => point,
                _ => point, // TODO:
            };
        }

        public static SKShader ToSKShader(BrushViewModel brush)
        {
            switch (brush)
            {
                case SolidColorBrushViewModel solidColorBrush:
                    {
                        var color = ToSKColor(solidColorBrush.Color);
                        return SKShader.CreateColor(color);
                    }
                case LinearGradientBrushViewModel linearGradientBrush:
                    {
                        var colors = ToSKColors(linearGradientBrush.GradientStops);
                        var colorPos = ToSKColorPos(linearGradientBrush.GradientStops);
                        var spreadMethod = ToSKShaderTileMode(linearGradientBrush.SpreadMethod);
                        var startPoint = ToSKPoint(linearGradientBrush.StartPoint);
                        var endPoint = ToSKPoint(linearGradientBrush.EndPoint);
                        return SKShader.CreateLinearGradient(startPoint, endPoint, colors, colorPos, spreadMethod);
                    }
                case RadialGradientBrushViewModel radialGradientBrush:
                    {
                        var colors = ToSKColors(radialGradientBrush.GradientStops);
                        var colorPos = ToSKColorPos(radialGradientBrush.GradientStops);
                        var spreadMethod = ToSKShaderTileMode(radialGradientBrush.SpreadMethod);
                        var center = ToSKPoint(radialGradientBrush.Center);
                        var gradientOrigin = ToSKPoint(radialGradientBrush.GradientOrigin); // TODO:
                        return SKShader.CreateRadialGradient(center, (float)radialGradientBrush.Radius, colors, colorPos, spreadMethod);
                    }
                default:
                    return default;
            }
        }

        public static SKPaint ToSKPaint(BrushViewModel brush)
        {
            if (brush == null)
            {
                return default;
            }
            var shader = ToSKShader(brush);
            return new SKPaint()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Shader = shader
            };
        }

        public static float[] ToIntevals(ObservableCollection<double> dashes)
        {
            var result = new float[dashes.Count];
            for (int i = 0; i < dashes.Count; i++)
            {
                result[i] = (float)dashes[i];
            }
            return result;
        }

        public static SKPathEffect ToSKPathEffect(DashStyleViewModel dashStyle)
        {
            if (dashStyle == null || dashStyle.Dashes == null)
            {
                return default;
            }
            var intervals = ToIntevals(dashStyle.Dashes);
            return SKPathEffect.CreateDash(intervals, (float)dashStyle.Offset);
        }

        public static SKStrokeCap ToSKStrokeCap(PenLineCap lineCap)
        {
            return lineCap switch
            {
                PenLineCap.Round => SKStrokeCap.Round,
                PenLineCap.Square => SKStrokeCap.Square,
                _ => SKStrokeCap.Butt,
            };
        }

        public static SKStrokeJoin ToSKStrokeJoin(PenLineJoin lineJoin)
        {
            return lineJoin switch
            {
                PenLineJoin.Bevel => SKStrokeJoin.Bevel,
                PenLineJoin.Round => SKStrokeJoin.Round,
                _ => SKStrokeJoin.Miter,
            };
        }

        public static SKPaint ToSKPaint(PenViewModel pen)
        {
            if (pen == null)
            {
                return default;
            }
            var shader = ToSKShader(pen.Brush);
            var pathEffect = ToSKPathEffect(pen.DashStyle);
            var lineCap = ToSKStrokeCap(pen.LineCap);
            var lineJoin = ToSKStrokeJoin(pen.LineJoin);
            return new SKPaint()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Shader = shader,
                PathEffect = pathEffect,
                StrokeWidth = (float)pen.Thickness,
                StrokeCap = lineCap,
                StrokeJoin = lineJoin,
                StrokeMiter = (float)pen.MiterLimit
            };
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

        public static SKPath ToSKPath(LineShapeViewModel lineShape)
        {
            var path = new SKPath() { FillType = SKPathFillType.Winding };
            path.MoveTo(new SKPoint((float)lineShape.StartPoint.X, (float)lineShape.StartPoint.Y));
            path.LineTo(new SKPoint((float)lineShape.Point.X, (float)lineShape.Point.Y));
            return path;
        }

        public static SKPath ToSKPath(CubicBezierShapeViewModel cubicBezierShape)
        {
            var path = new SKPath() { FillType = SKPathFillType.Winding };
            path.MoveTo(new SKPoint((float)cubicBezierShape.StartPoint.X, (float)cubicBezierShape.StartPoint.Y));
            path.CubicTo(
                new SKPoint((float)cubicBezierShape.Point1.X, (float)cubicBezierShape.Point1.Y),
                new SKPoint((float)cubicBezierShape.Point2.X, (float)cubicBezierShape.Point2.Y),
                new SKPoint((float)cubicBezierShape.Point3.X, (float)cubicBezierShape.Point3.Y));
            return path;
        }

        public static SKPath ToSKPath(QuadraticBezierShapeViewModel quadraticBezierShape)
        {
            var path = new SKPath() { FillType = SKPathFillType.Winding };
            path.MoveTo(new SKPoint((float)quadraticBezierShape.StartPoint.X, (float)quadraticBezierShape.StartPoint.Y));
            path.QuadTo(
                new SKPoint((float)quadraticBezierShape.Control.X, (float)quadraticBezierShape.Control.Y),
                new SKPoint((float)quadraticBezierShape.EndPoint.X, (float)quadraticBezierShape.EndPoint.Y));
            return path;
        }

        public static SKPath ToSKPath(PathShapeViewModel pathShape)
        {
            var path = new SKPath() { FillType = pathShape.FillRule == FillRule.EvenOdd ? SKPathFillType.EvenOdd : SKPathFillType.Winding };

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

        public static SKPath ToSKPath(RectangleShapeViewModel rectangleShape)
        {
            var rect = ToSKRect(rectangleShape);
            var path = new SKPath() { FillType = SKPathFillType.Winding };
            path.AddRect(rect);
            return path;
        }

        public static SKPath ToSKPath(EllipseShapeViewModel ellipseShape)
        {
            var rect = ToSKRect(ellipseShape);
            var path = new SKPath() { FillType = SKPathFillType.Winding };
            path.AddOval(rect);
            return path;
        }

        public static void Render(SKCanvas context, LineShapeViewModel lineShape)
        {
            if (lineShape.IsStroked)
            {
                var p1 = ToSKPoint(lineShape.StartPoint);
                var p2 = ToSKPoint(lineShape.Point);
                var pen = ToSKPaint(lineShape.Pen);
                context.DrawLine(p1, p2, pen);
            }
        }

        public static void Render(SKCanvas context, CubicBezierShapeViewModel cubicBezierShape)
        {
            if (cubicBezierShape.IsStroked || cubicBezierShape.IsFilled)
            {
                var path = ToSKPath(cubicBezierShape);
                var brush = ToSKPaint(cubicBezierShape.Brush);
                var pen = ToSKPaint(cubicBezierShape.Pen);
                if (cubicBezierShape.IsFilled)
                {
                    context.DrawPath(path, brush);
                }
                if (cubicBezierShape.IsStroked)
                {
                    context.DrawPath(path, pen);
                }
            }
        }

        public static void Render(SKCanvas context, QuadraticBezierShapeViewModel quadraticBezierShape)
        {
            if (quadraticBezierShape.IsStroked || quadraticBezierShape.IsFilled)
            {
                var path = ToSKPath(quadraticBezierShape);
                var brush = ToSKPaint(quadraticBezierShape.Brush);
                var pen = ToSKPaint(quadraticBezierShape.Pen);
                if (quadraticBezierShape.IsFilled)
                {
                    context.DrawPath(path, brush);
                }
                if (quadraticBezierShape.IsStroked)
                {
                    context.DrawPath(path, pen);
                }
            }
        }

        public static void Render(SKCanvas context, RectangleShapeViewModel rectangleShape)
        {
            if (rectangleShape.IsStroked || rectangleShape.IsFilled)
            {
                var rect = ToSKRect(rectangleShape);
                var brush = ToSKPaint(rectangleShape.Brush);
                var pen = ToSKPaint(rectangleShape.Pen);
                if (rectangleShape.IsFilled)
                {
                    context.DrawRoundRect(rect, (float)rectangleShape.RadiusX, (float)rectangleShape.RadiusY, brush);
                }
                if (rectangleShape.IsStroked)
                {
                    context.DrawRoundRect(rect, (float)rectangleShape.RadiusX, (float)rectangleShape.RadiusY, pen);
                }
            }
        }

        public static void Render(SKCanvas context, EllipseShapeViewModel ellipseShape)
        {
            if (ellipseShape.IsStroked || ellipseShape.IsFilled)
            {
                var path = ToSKPath(ellipseShape);
                var brush = ToSKPaint(ellipseShape.Brush);
                var pen = ToSKPaint(ellipseShape.Pen);
                if (ellipseShape.IsFilled)
                {
                    context.DrawPath(path, brush);
                }
                if (ellipseShape.IsStroked)
                {
                    context.DrawPath(path, pen);
                }
            }
        }

        public static void Render(SKCanvas context, PathShapeViewModel pathShape)
        {
            if (pathShape.IsStroked || pathShape.IsFilled)
            {
                var path = ToSKPath(pathShape);
                var brush = ToSKPaint(pathShape.Brush);
                var pen = ToSKPaint(pathShape.Pen);
                if (pathShape.IsFilled)
                {
                    context.DrawPath(path, brush);
                }
                if (pathShape.IsStroked)
                {
                    context.DrawPath(path, pen);
                }
            }
        }

        public static void Render(SKCanvas context, ShapeBaseViewModel shape)
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

        public static void Render(SKCanvas context, ObservableCollection<ViewModelBase> items)
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

        public static void Render(SKCanvas context, CanvasViewModel canvas)
        {
            Render(context, canvas.Items);
            Render(context, canvas.Decorators);
        }
    }
}
