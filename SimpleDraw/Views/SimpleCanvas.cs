using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using SimpleDraw.ViewModels;

namespace SimpleDraw.Views
{
    public class SimpleCanvas : Canvas
    {
        private ToolPointerType ToToolPointerType(PointerUpdateKind pointerUpdateKind)
        {
            switch (pointerUpdateKind)
            {
                case PointerUpdateKind.LeftButtonPressed:
                case PointerUpdateKind.LeftButtonReleased:
                    return ToolPointerType.Left;
                case PointerUpdateKind.RightButtonPressed:
                case PointerUpdateKind.RightButtonReleased:
                    return ToolPointerType.Right;
                default:
                    return ToolPointerType.None;
            }
        }

        private Rect ToRect(RectangleShapeViewModel rectangleShape)
        {
            var x = Math.Min(rectangleShape.TopLeft.X, rectangleShape.BottomRight.X);
            var y = Math.Min(rectangleShape.TopLeft.Y, rectangleShape.BottomRight.Y);
            var width = Math.Abs(rectangleShape.TopLeft.X - rectangleShape.BottomRight.X);
            var height = Math.Abs(rectangleShape.TopLeft.Y - rectangleShape.BottomRight.Y);
            return new Rect(x, y, width, height);
        }

        private Color ToColor(ColorViewModel color)
        {
            return new Color(color.A, color.R, color.G, color.B);
        }

        private IBrush ToBrush(BrushViewModel brush)
        {
            switch (brush)
            {
                case SolidColorBrushViewModel solidColorBrush:
                    {
                        var color = ToColor(solidColorBrush.Color);
                        return new ImmutableSolidColorBrush(color);
                    }
                default:
                    return default;
            }
        }

        private ImmutableDashStyle ToDashStyle(DashStyleViewModel dashStyle)
        {
            if (dashStyle == null || dashStyle.Dashes == null)
            {
                return default;
            }
            return new ImmutableDashStyle(dashStyle.Dashes, dashStyle.Offset);
        }

        private PenLineCap ToPenLineCap(LineCap lineCap)
        {
            return lineCap switch
            {
                LineCap.Round => PenLineCap.Round,
                LineCap.Square => PenLineCap.Square,
                _ => PenLineCap.Flat,
            };
        }

        private PenLineJoin ToPenLineJoin(LineJoin lineJoin)
        {
            return lineJoin switch
            {
                LineJoin.Bevel => PenLineJoin.Bevel,
                LineJoin.Round => PenLineJoin.Round,
                _ => PenLineJoin.Miter,
            };
        }

        private ImmutablePen ToPen(PenViewModel pen)
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

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            if (DataContext is CanvasViewModel canvas)
            {
                var point = e.GetCurrentPoint(this);
                var type = point.Properties.PointerUpdateKind;
                canvas.Tool?.Pressed(canvas, point.Position.X, point.Position.Y, ToToolPointerType(type));
                InvalidateVisual();
            }
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);

            if (DataContext is CanvasViewModel canvas)
            {
                var point = e.GetCurrentPoint(this);
                var type = point.Properties.PointerUpdateKind;
                canvas.Tool?.Released(canvas, point.Position.X, point.Position.Y, ToToolPointerType(type));
                InvalidateVisual();
            }
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            base.OnPointerMoved(e);

            if (DataContext is CanvasViewModel canvas)
            {
                var point = e.GetCurrentPoint(this);
                var type = point.Properties.PointerUpdateKind;
                canvas.Tool?.Moved(canvas, point.Position.X, point.Position.Y, ToToolPointerType(type));
                InvalidateVisual();
            }
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (DataContext is CanvasViewModel canvas)
            {
                Render(context, canvas);
            }
        }

        private void Render(DrawingContext context, CanvasViewModel canvas)
        {
            foreach (var shape in canvas.Shapes)
            {
                switch (shape)
                {
                    case LineShapeViewModel lineShape:
                        {
                            if (lineShape.IsStroked)
                            {
                                var p1 = new Point(lineShape.Start.X, lineShape.Start.Y);
                                var p2 = new Point(lineShape.End.X, lineShape.End.Y);
                                var pen = ToPen(lineShape.Pen);
                                context.DrawLine(pen, p1, p2);
                            }
                        }
                        break;
                    case RectangleShapeViewModel rectangleShape:
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
                        break;
                }
            }
        }
    }
}
