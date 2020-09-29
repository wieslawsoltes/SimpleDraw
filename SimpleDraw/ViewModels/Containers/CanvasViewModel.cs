﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ReactiveUI;

namespace SimpleDraw.ViewModels
{
    public delegate void InvalidateEventHandler(object sender, EventArgs e);

    [DataContract(IsReference = true)]
    public class CanvasViewModel : ViewModelBase
    {
        private double _width;
        private double _height;
        private ObservableCollection<ViewModelBase> _items;
        private ObservableCollection<ViewModelBase> _selected;
        private ObservableCollection<ViewModelBase> _decorators;
        private ToolBaseViewModel _tool;
        private ObservableCollection<ToolBaseViewModel> _tools;
        private ObservableCollection<ViewModelBase> _copy;
        private RectangleShapeViewModel _rectangleSelection;
        private RectangleShapeViewModel _rectangleBounds;

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

        private void GetPoints(IList<ViewModelBase> items, HashSet<PointViewModel> points)
        {
            foreach (var item in items)
            {
                switch (item)
                {
                    case PointViewModel point:
                        {
                            points.Add(point);
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
                            points.Add(lineShape.Start);
                            points.Add(lineShape.End);
                        }
                        break;
                    case RectangleShapeViewModel rectangleShape:
                        {
                            points.Add(rectangleShape.TopLeft);
                            points.Add(rectangleShape.BottomRight);
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

        public void ShowSelection(double x, double y)
        {
            _decorators.Add(_rectangleSelection);
            _rectangleSelection.TopLeft.X = x;
            _rectangleSelection.TopLeft.Y = y;
            _rectangleSelection.BottomRight.X = x;
            _rectangleSelection.BottomRight.Y = y;
        }

        public void MoveSelection(double x, double y)
        {
            _rectangleSelection.BottomRight.X = x;
            _rectangleSelection.BottomRight.Y = y;
        }

        public void RemoveSelection()
        {
            _decorators.Remove(_rectangleSelection);
        }

        public void UpdateBounds()
        {
            if (_selected.Count > 0)
            {
                var bounds = HitTest.GetBounds(_selected);
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

        public void RemoveBounds()
        {
            _decorators.Remove(_rectangleBounds);
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

            _selected.Clear();

            UpdateBounds();
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

            UpdateBounds();
            Invalidate();
        }

        public void Paste()
        {
            var shared = new Dictionary<ViewModelBase, ViewModelBase>();

            _selected.Clear();

            foreach (var item in _copy)
            {
                var clone = item.Clone(shared);
                _items.Add(clone);
                _selected.Add(clone);
            }

            UpdateBounds();
            Invalidate();
        }

        public void Delete()
        {
            foreach (var item in _selected)
            {
                _items.Remove(item);
            }

            _selected.Clear();

            UpdateBounds();
            Invalidate();
        }

        public void SelectAll()
        {
            _selected.Clear();

            foreach (var item in _items)
            {
                _selected.Add(item);
            }

            UpdateBounds();
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

            _selected.Clear();
            _selected.Add(group);

            UpdateBounds();
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
            }

            if (update)
            {
                _selected.Clear();

                foreach (var item in ungrouped)
                {
                    _items.Add(item);
                    _selected.Add(item);
                }

                UpdateBounds();
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
}