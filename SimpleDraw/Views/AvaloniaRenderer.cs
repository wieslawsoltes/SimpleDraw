using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using SimpleDraw.ViewModels;

namespace SimpleDraw.Views
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

        public static void Render(DrawingContext context, LineShapeViewModel lineShape)
        {
            if (lineShape.IsStroked)
            {
                var p1 = new Point(lineShape.Start.X, lineShape.Start.Y);
                var p2 = new Point(lineShape.End.X, lineShape.End.Y);
                var pen = ToPen(lineShape.Pen);
                context.DrawLine(pen, p1, p2);
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

        public static void Render(DrawingContext context, ShapeBaseViewModel shape)
        {
            switch (shape)
            {
                case LineShapeViewModel lineShape:
                    {
                        Render(context, lineShape);
                    }
                    break;
                case RectangleShapeViewModel rectangleShape:
                    {
                        Render(context, rectangleShape);
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
                    case PointViewModel point:
                        {
                            // TODO:
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
