using System;
using SkiaSharp;

namespace SimpleDraw.ViewModels
{
    internal static class SkiaRenderer
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
    }
}
