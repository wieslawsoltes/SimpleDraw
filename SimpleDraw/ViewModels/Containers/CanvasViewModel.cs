﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ReactiveUI;
using SimpleDraw.Skia;
using SimpleDraw.ViewModels.Media;
using SimpleDraw.ViewModels.Primitives;
using SimpleDraw.ViewModels.Shapes;
using SimpleDraw.ViewModels.Tools;

namespace SimpleDraw.ViewModels.Containers;

public delegate void InvalidateEventHandler(object sender, EventArgs e);

[DataContract(IsReference = true)]
public class CanvasViewModel : ViewModelBase, IItemsCanvas
{
    private double _width;
    private double _height;
    private ObservableCollection<ViewModelBase> _items;
    private ObservableCollection<ViewModelBase> _hovered;
    private ObservableCollection<ViewModelBase> _selected;
    private ObservableCollection<ViewModelBase> _decorators;
    private ToolBaseViewModel _tool;
    private ObservableCollection<ToolBaseViewModel> _tools;
    private readonly ObservableCollection<ViewModelBase> _copy;
    private readonly RectangleShapeViewModel _rectangleSelection;
    private readonly RectangleShapeViewModel _rectangleBounds;

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public double Width
    {
        get => _width;
        set => this.RaiseAndSetIfChanged(ref _width, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public double Height
    {
        get => _height;
        set => this.RaiseAndSetIfChanged(ref _height, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public ObservableCollection<ViewModelBase> Items
    {
        get => _items;
        set => this.RaiseAndSetIfChanged(ref _items, value);
    }

    [IgnoreDataMember]
    public ObservableCollection<ViewModelBase> Hovered
    {
        get => _hovered;
        set => this.RaiseAndSetIfChanged(ref _hovered, value);
    }

    [IgnoreDataMember]
    public ObservableCollection<ViewModelBase> Selected
    {
        get => _selected;
        set => this.RaiseAndSetIfChanged(ref _selected, value);
    }

    [IgnoreDataMember]
    public ObservableCollection<ViewModelBase> Decorators
    {
        get => _decorators;
        set => this.RaiseAndSetIfChanged(ref _decorators, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public ToolBaseViewModel Tool
    {
        get => _tool;
        set => this.RaiseAndSetIfChanged(ref _tool, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public ObservableCollection<ToolBaseViewModel> Tools
    {
        get => _tools;
        set => this.RaiseAndSetIfChanged(ref _tools, value);
    }

    public event InvalidateEventHandler InvalidateCanvas;

    public CanvasViewModel()
    {
        _hovered = new ObservableCollection<ViewModelBase>();

        _selected = new ObservableCollection<ViewModelBase>();

        _decorators = new ObservableCollection<ViewModelBase>();

        _copy = new ObservableCollection<ViewModelBase>();

        _rectangleSelection = new RectangleShapeViewModel()
        {
            TopLeft = new PointViewModel(0, 0),
            BottomRight = new PointViewModel(0, 0),
            IsStroked = true,
            IsFilled = true,
            RadiusX = 0,
            RadiusY = 0,
            Brush = new SolidColorBrushViewModel(new ColorViewModel(80, 0, 0, 255)),
            Pen = new PenViewModel(new SolidColorBrushViewModel(new ColorViewModel(160, 0, 0, 255)), 2)
        };

        _rectangleBounds = new RectangleShapeViewModel()
        {
            TopLeft = new PointViewModel(0, 0),
            BottomRight = new PointViewModel(0, 0),
            IsStroked = true,
            IsFilled = true,
            RadiusX = 0,
            RadiusY = 0,
            Brush = new SolidColorBrushViewModel(new ColorViewModel(0, 0, 255, 255)),
            Pen = new PenViewModel(new SolidColorBrushViewModel(new ColorViewModel(255, 0, 255, 255)), 2)
        };
    }

    public void Invalidate()
    {
        InvalidateCanvas?.Invoke(this, new EventArgs());
    }

    public CanvasViewModel CloneSelf(Dictionary<ViewModelBase, ViewModelBase> shared)
    {
        if (shared.TryGetValue(this, out var value))
        {
            return value as CanvasViewModel;
        }

        var items = new ObservableCollection<ViewModelBase>();

        foreach (var item in _items)
        {
            items.Add(item.Clone(shared));
        }

        var selected = new ObservableCollection<ViewModelBase>();

        foreach (var item in _selected)
        {
            selected.Add(item.Clone(shared));
        }

        var decorators = new ObservableCollection<ViewModelBase>();

        foreach (var item in _decorators)
        {
            decorators.Add(item.Clone(shared));
        }

        var tools = new ObservableCollection<ToolBaseViewModel>();

        foreach (var item in _tools)
        {
            tools.Add(item.CloneSelf(shared));
        }

        var copy = new CanvasViewModel()
        {
            Width = _width,
            Height = _height,
            Items = items,
            Selected = selected,
            Decorators = decorators,
            Tool = _tool?.CloneSelf(shared),
            Tools = tools
        };

        shared[this] = copy;
        return copy;
    }

    public override ViewModelBase Clone(Dictionary<ViewModelBase, ViewModelBase> shared)
    {
        return CloneSelf(shared);
    }

    public static void GetPoints(IList<ViewModelBase> items, HashSet<PointViewModel> points)
    {
        foreach (var item in items)
        {
            switch (item)
            {
                case PointViewModel point:
                {
                    points.Add(point);
                }
                    break;
                case GroupViewModel group:
                {
                    GetPoints(group.Items, points);
                }
                    break;
                case LineShapeViewModel lineShape:
                {
                    points.Add(lineShape.StartPoint);
                    points.Add(lineShape.Point);
                }
                    break;
                case CubicBezierShapeViewModel cubicBezierShape:
                {
                    points.Add(cubicBezierShape.StartPoint);
                    points.Add(cubicBezierShape.Point1);
                    points.Add(cubicBezierShape.Point2);
                    points.Add(cubicBezierShape.Point3);
                }
                    break;
                case QuadraticBezierShapeViewModel quadraticBezierShape:
                {
                    points.Add(quadraticBezierShape.StartPoint);
                    points.Add(quadraticBezierShape.Control);
                    points.Add(quadraticBezierShape.EndPoint);
                }
                    break;
                case PathShapeViewModel pathShape:
                {
                    foreach (var figure in pathShape.Figures)
                    {
                        GetPoints(figure.Segments, points);
                    }
                }
                    break;
                case RectangleShapeViewModel rectangleShape:
                {
                    points.Add(rectangleShape.TopLeft);
                    points.Add(rectangleShape.BottomRight);
                }
                    break;
                case EllipseShapeViewModel ellipseShape:
                {
                    points.Add(ellipseShape.TopLeft);
                    points.Add(ellipseShape.BottomRight);
                }
                    break;
            }
        }
    }

    public void MoveSelected(double deltaX, double deltaY)
    {
        var points = new HashSet<PointViewModel>();

        GetPoints(_selected, points);

        foreach (var point in points)
        {
            point.X += deltaX;
            point.Y += deltaY;
        }
    }

    public void ShowSelectionDecorator(double x, double y)
    {
        _decorators.Add(_rectangleSelection);
        _rectangleSelection.TopLeft.X = x;
        _rectangleSelection.TopLeft.Y = y;
        _rectangleSelection.BottomRight.X = x;
        _rectangleSelection.BottomRight.Y = y;
    }

    public void MoveSelectionDecorator(double x, double y)
    {
        _rectangleSelection.BottomRight.X = x;
        _rectangleSelection.BottomRight.Y = y;
    }

    public void RemoveSelectionDecorator()
    {
        _decorators.Remove(_rectangleSelection);
    }

    public void UpdateSelectionBounds()
    {
        if (_selected.Count > 0)
        {
            var bounds = SkiaHitTest.GetBounds(_selected);
            if (!bounds.IsEmpty)
            {
                if (!_decorators.Contains(_rectangleBounds))
                {
                    _decorators.Add(_rectangleBounds);
                }
                _rectangleBounds.TopLeft.X = bounds.Left;
                _rectangleBounds.TopLeft.Y = bounds.Top;
                _rectangleBounds.BottomRight.X = bounds.Right;
                _rectangleBounds.BottomRight.Y = bounds.Bottom;
            }
            else
            {
                if (_decorators.Contains(_rectangleBounds))
                {
                    _decorators.Remove(_rectangleBounds);
                }
            }
        }
        else
        {
            if (_decorators.Contains(_rectangleBounds))
            {
                _decorators.Remove(_rectangleBounds);
            }
        }
    }

    public void RemoveSelectionBounds()
    {
        _decorators.Remove(_rectangleBounds);
    }

    public bool ConnectPoint(ObservableCollection<ViewModelBase> items, PointViewModel selectedPoint, PointViewModel hitTestPoint)
    {
        foreach (var item in items)
        {
            switch (item)
            {
                case GroupViewModel group:
                {
                    if (ConnectPoint(group.Items, selectedPoint, hitTestPoint))
                    {
                        return true;
                    }
                }
                    break;
                case LineShapeViewModel lineShape:
                {
                    if (lineShape.StartPoint == hitTestPoint
                        && lineShape.Point != selectedPoint)
                    {
                        lineShape.StartPoint = selectedPoint;
                        return true;
                    }

                    if (lineShape.Point == hitTestPoint
                        && lineShape.StartPoint != selectedPoint)
                    {
                        lineShape.Point = selectedPoint;
                        return true;
                    }
                }
                    break;
                case CubicBezierShapeViewModel cubicBezierShape:
                {
                    if (cubicBezierShape.StartPoint == hitTestPoint
                        && cubicBezierShape.Point1 != selectedPoint
                        && cubicBezierShape.Point2 != selectedPoint
                        && cubicBezierShape.Point3 != selectedPoint)
                    {
                        cubicBezierShape.StartPoint = selectedPoint;
                        return true;
                    }

                    if (cubicBezierShape.Point1 == hitTestPoint
                        && cubicBezierShape.StartPoint != selectedPoint
                        && cubicBezierShape.Point2 != selectedPoint
                        && cubicBezierShape.Point3 != selectedPoint)
                    {
                        cubicBezierShape.Point1 = selectedPoint;
                        return true;
                    }

                    if (cubicBezierShape.Point2 == hitTestPoint
                        && cubicBezierShape.StartPoint != selectedPoint
                        && cubicBezierShape.Point1 != selectedPoint
                        && cubicBezierShape.Point3 != selectedPoint)
                    {
                        cubicBezierShape.Point2 = selectedPoint;
                        return true;
                    }

                    if (cubicBezierShape.Point3 == hitTestPoint
                        && cubicBezierShape.StartPoint != selectedPoint
                        && cubicBezierShape.Point1 != selectedPoint
                        && cubicBezierShape.Point2 != selectedPoint)
                    {
                        cubicBezierShape.Point3 = selectedPoint;
                        return true;
                    }
                }
                    break;
                case QuadraticBezierShapeViewModel quadraticBezierShape:
                {
                    if (quadraticBezierShape.StartPoint == hitTestPoint
                        && quadraticBezierShape.Control != selectedPoint
                        && quadraticBezierShape.EndPoint != selectedPoint)
                    {
                        quadraticBezierShape.StartPoint = selectedPoint;
                        return true;
                    }

                    if (quadraticBezierShape.Control == hitTestPoint
                        && quadraticBezierShape.StartPoint != selectedPoint
                        && quadraticBezierShape.EndPoint != selectedPoint)
                    {
                        quadraticBezierShape.Control = selectedPoint;
                        return true;
                    }

                    if (quadraticBezierShape.EndPoint == hitTestPoint
                        && quadraticBezierShape.StartPoint != selectedPoint
                        && quadraticBezierShape.Control != selectedPoint)
                    {
                        quadraticBezierShape.EndPoint = selectedPoint;
                        return true;
                    }
                }
                    break;
                case PathShapeViewModel pathShape:
                {
                    foreach (var figure in pathShape.Figures)
                    {
                        if (ConnectPoint(figure.Segments, selectedPoint, hitTestPoint))
                        {
                            return true;
                        }
                    }
                }
                    break;
                case RectangleShapeViewModel rectangleShape:
                {
                    if (rectangleShape.TopLeft == hitTestPoint
                        && rectangleShape.BottomRight != selectedPoint)
                    {
                        rectangleShape.TopLeft = selectedPoint;
                        return true;
                    }

                    if (rectangleShape.BottomRight == hitTestPoint
                        && rectangleShape.TopLeft != selectedPoint)
                    {
                        rectangleShape.BottomRight = selectedPoint;
                        return true;
                    }
                }
                    break;
                case EllipseShapeViewModel ellipseShape:
                {
                    if (ellipseShape.TopLeft == hitTestPoint
                        && ellipseShape.BottomRight != selectedPoint)
                    {
                        ellipseShape.TopLeft = selectedPoint;
                        return true;
                    }

                    if (ellipseShape.BottomRight == hitTestPoint
                        && ellipseShape.TopLeft != selectedPoint)
                    {
                        ellipseShape.BottomRight = selectedPoint;
                        return true;
                    }
                }
                    break;
            }
        }

        return false;
    }

    public void GetConnected(ObservableCollection<ViewModelBase> items, PointViewModel selectedPoint, List<ViewModelBase> connected)
    {
        foreach (var item in items)
        {
            switch (item)
            {
                case GroupViewModel group:
                {
                    GetConnected(group.Items, selectedPoint, connected);
                }
                    break;
                case LineShapeViewModel lineShape:
                {
                    if (lineShape.StartPoint == selectedPoint)
                    {
                        connected.Add(lineShape);
                        break;
                    }

                    if (lineShape.Point == selectedPoint)
                    {
                        connected.Add(lineShape);
                        break;
                    }
                }
                    break;
                case CubicBezierShapeViewModel cubicBezierShape:
                {
                    if (cubicBezierShape.StartPoint == selectedPoint)
                    {
                        connected.Add(cubicBezierShape);
                        break;
                    }

                    if (cubicBezierShape.Point1 == selectedPoint)
                    {
                        connected.Add(cubicBezierShape);
                        break;
                    }

                    if (cubicBezierShape.Point2 == selectedPoint)
                    {
                        connected.Add(cubicBezierShape);
                        break;
                    }

                    if (cubicBezierShape.Point3 == selectedPoint)
                    {
                        connected.Add(cubicBezierShape);
                        break;
                    }
                }
                    break;
                case QuadraticBezierShapeViewModel quadraticBezierShape:
                {
                    if (quadraticBezierShape.StartPoint == selectedPoint)
                    {
                        connected.Add(quadraticBezierShape);
                        break;
                    }

                    if (quadraticBezierShape.Control == selectedPoint)
                    {
                        connected.Add(quadraticBezierShape);
                        break;
                    }

                    if (quadraticBezierShape.EndPoint == selectedPoint)
                    {
                        connected.Add(quadraticBezierShape);
                        break;
                    }
                }
                    break;
                case PathShapeViewModel pathShape:
                {
                    foreach (var figure in pathShape.Figures)
                    {
                        GetConnected(figure.Segments, selectedPoint, connected);
                    }
                }
                    break;
                case RectangleShapeViewModel rectangleShape:
                {
                    if (rectangleShape.TopLeft == selectedPoint)
                    {
                        connected.Add(rectangleShape);
                        break;
                    }

                    if (rectangleShape.BottomRight == selectedPoint)
                    {
                        connected.Add(rectangleShape);
                        break;
                    }
                }
                    break;
                case EllipseShapeViewModel ellipseShape:
                {
                    if (ellipseShape.TopLeft == selectedPoint)
                    {
                        connected.Add(ellipseShape);
                        break;
                    }

                    if (ellipseShape.BottomRight == selectedPoint)
                    {
                        connected.Add(ellipseShape);
                        break;
                    }
                }
                    break;
            }
        }
    }

    public PointViewModel DisconnectPoint(ObservableCollection<ViewModelBase> items, PointViewModel selectedPoint)
    {
        foreach (var item in items)
        {
            switch (item)
            {
                case GroupViewModel group:
                {
                    var result = DisconnectPoint(group.Items, selectedPoint);
                    if (result != null)
                    {
                        return result;
                    }
                }
                    break;
                case LineShapeViewModel lineShape:
                {
                    if (lineShape.StartPoint == selectedPoint)
                    {
                        lineShape.StartPoint = new PointViewModel(selectedPoint.X, selectedPoint.Y);
                        return lineShape.StartPoint;
                    }

                    if (lineShape.Point == selectedPoint)
                    {
                        lineShape.Point = new PointViewModel(selectedPoint.X, selectedPoint.Y);
                        return lineShape.Point;
                    }
                }
                    break;
                case CubicBezierShapeViewModel cubicBezierShape:
                {
                    if (cubicBezierShape.StartPoint == selectedPoint)
                    {
                        cubicBezierShape.StartPoint = new PointViewModel(selectedPoint.X, selectedPoint.Y);
                        return cubicBezierShape.StartPoint;
                    }

                    if (cubicBezierShape.Point1 == selectedPoint)
                    {
                        cubicBezierShape.Point1 = new PointViewModel(selectedPoint.X, selectedPoint.Y);
                        return cubicBezierShape.Point1;
                    }

                    if (cubicBezierShape.Point2 == selectedPoint)
                    {
                        cubicBezierShape.Point2 = new PointViewModel(selectedPoint.X, selectedPoint.Y);
                        return cubicBezierShape.Point2;
                    }

                    if (cubicBezierShape.Point3 == selectedPoint)
                    {
                        cubicBezierShape.Point3 = new PointViewModel(selectedPoint.X, selectedPoint.Y);
                        return cubicBezierShape.Point3;
                    }
                }
                    break;
                case QuadraticBezierShapeViewModel quadraticBezierShape:
                {
                    if (quadraticBezierShape.StartPoint == selectedPoint)
                    {
                        quadraticBezierShape.StartPoint = new PointViewModel(selectedPoint.X, selectedPoint.Y);
                        return quadraticBezierShape.StartPoint;
                    }

                    if (quadraticBezierShape.Control == selectedPoint)
                    {
                        quadraticBezierShape.Control = new PointViewModel(selectedPoint.X, selectedPoint.Y);
                        return quadraticBezierShape.Control;
                    }

                    if (quadraticBezierShape.EndPoint == selectedPoint)
                    {
                        quadraticBezierShape.EndPoint = new PointViewModel(selectedPoint.X, selectedPoint.Y);
                        return quadraticBezierShape.EndPoint;
                    }
                }
                    break;
                case PathShapeViewModel pathShape:
                {
                    foreach (var figure in pathShape.Figures)
                    {
                        var result = DisconnectPoint(figure.Segments, selectedPoint);
                        {
                            return result;
                        }
                    }
                }
                    break;
                case RectangleShapeViewModel rectangleShape:
                {
                    if (rectangleShape.TopLeft == selectedPoint)
                    {
                        rectangleShape.TopLeft = new PointViewModel(selectedPoint.X, selectedPoint.Y);
                        return rectangleShape.TopLeft;
                    }

                    if (rectangleShape.BottomRight == selectedPoint)
                    {
                        rectangleShape.BottomRight = new PointViewModel(selectedPoint.X, selectedPoint.Y);
                        return rectangleShape.BottomRight;
                    }
                }
                    break;
                case EllipseShapeViewModel ellipseShape:
                {
                    if (ellipseShape.TopLeft == selectedPoint)
                    {
                        ellipseShape.TopLeft = new PointViewModel(selectedPoint.X, selectedPoint.Y);
                        return ellipseShape.TopLeft;
                    }

                    if (ellipseShape.BottomRight == selectedPoint)
                    {
                        ellipseShape.BottomRight = new PointViewModel(selectedPoint.X, selectedPoint.Y);
                        return ellipseShape.BottomRight;
                    }
                }
                    break;
            }
        }

        return null;
    }

    public void Cut()
    {
        var shared = new Dictionary<ViewModelBase, ViewModelBase>();

        _copy.Clear();

        foreach (var item in _selected)
        {
            _copy.Add(item.Clone(shared));
        }

        foreach (var item in _selected)
        {
            _items.Remove(item);
        }

        _hovered.Clear();
        _selected.Clear();

        UpdateSelectionBounds();
        Invalidate();
    }

    public void Copy()
    {
        var shared = new Dictionary<ViewModelBase, ViewModelBase>();

        _copy.Clear();

        foreach (var item in _selected)
        {
            _copy.Add(item.Clone(shared));
        }

        UpdateSelectionBounds();
        Invalidate();
    }

    public void Paste()
    {
        var shared = new Dictionary<ViewModelBase, ViewModelBase>();

        _hovered.Clear();
        _selected.Clear();

        foreach (var item in _copy)
        {
            var clone = item.Clone(shared);
            _items.Add(clone);
            _selected.Add(clone);
        }

        UpdateSelectionBounds();
        Invalidate();
    }

    public void Delete()
    {
        foreach (var item in _selected)
        {
            _items.Remove(item);
        }

        _hovered.Clear();
        _selected.Clear();

        UpdateSelectionBounds();
        Invalidate();
    }

    public void SelectAll()
    {
        _hovered.Clear();
        _selected.Clear();

        foreach (var item in _items)
        {
            _selected.Add(item);
        }

        UpdateSelectionBounds();
        Invalidate();
    }

    public void Group()
    {
        var group = new GroupViewModel()
        {
            Items = new ObservableCollection<ViewModelBase>()
        };

        foreach (var item in _selected)
        {
            group.Items.Add(item);
        }

        foreach (var item in _selected)
        {
            _items.Remove(item);
        }

        _items.Add(group);

        _hovered.Clear();
        _selected.Clear();
        _selected.Add(group);

        UpdateSelectionBounds();
        Invalidate();
    }

    public void Ungroup()
    {
        var update = false;
        var ungrouped = new List<ViewModelBase>();

        foreach (var item in _selected)
        {
            if (item is GroupViewModel group)
            {
                foreach (var groupItem in group.Items)
                {
                    ungrouped.Add(groupItem);
                }

                _items.Remove(group);
                update = true;
            }

            if (item is PathShapeViewModel pathShape)
            {
                foreach (var figure in pathShape.Figures)
                {
                    foreach (var segment in figure.Segments)
                    {
                        ungrouped.Add(segment);
                    }
                }

                _items.Remove(pathShape);
                update = true;
            }
        }

        if (update)
        {
            _hovered.Clear();
            _selected.Clear();

            foreach (var item in ungrouped)
            {
                _items.Add(item);
                _selected.Add(item);
            }

            UpdateSelectionBounds();
            Invalidate(); 
        }
    }

    public void SetTool(string name)
    {
        foreach (var tool in _tools)
        {
            if (tool.Name == name)
            {
                Tool = tool;
            }
        }

        Invalidate();
    }
}