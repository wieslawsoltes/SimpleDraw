using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;
using SimpleDraw.ViewModels;
using SimpleDraw.ViewModels.Containers;
using SimpleDraw.ViewModels.Media;
using SimpleDraw.ViewModels.Shapes;
using SimpleDraw.ViewModels.Tools;
using SimpleDraw.ViewModels.Tools.Shape;

namespace SimpleDraw.Avalonia
{
    public class AvaloniaSimpleDrawApp : ISimpleDrawApplication
    {
        public CanvasViewModel New()
        {
            var canvas = new CanvasViewModel()
            {
                Width = 810,
                Height = 570,
                Items = new ObservableCollection<ViewModelBase>()
            };

            canvas.Tools = new ObservableCollection<ToolBaseViewModel>()
            {
                new NoneToolViewModel(),
                new SelectionToolViewModel()
                {
                    HitRadius = 6,
                    TryToConnect = true
                },
                new LineToolViewModel()
                {
                    ShapeTool = new LineShapeToolViewModel()
                    {
                        Pen = new PenViewModel(new SolidColorBrushViewModel(new ColorViewModel(255, 0, 0, 0)), 2),
                        IsStroked = true,
                        HitRadius = 6,
                        TryToConnect = true
                    }
                },
                new CubicBezierToolViewModel()
                {
                    ShapeTool = new CubicBezierShapeToolViewModel()
                    {
                        Brush = new SolidColorBrushViewModel(new ColorViewModel(255, 0, 0, 0)),
                        Pen = new PenViewModel(new SolidColorBrushViewModel(new ColorViewModel(255, 0, 0, 0)), 2),
                        IsStroked = true,
                        IsFilled = false,
                        HitRadius = 6,
                        TryToConnect = true
                    }
                },
                new QuadraticBezierToolViewModel()
                {
                    ShapeTool = new QuadraticBezierShapeToolViewModel()
                    {
                        Brush = new SolidColorBrushViewModel(new ColorViewModel(255, 0, 0, 0)),
                        Pen = new PenViewModel(new SolidColorBrushViewModel(new ColorViewModel(255, 0, 0, 0)), 2),
                        IsStroked = true,
                        IsFilled = false,
                        HitRadius = 6,
                        TryToConnect = true
                    }
                },
                new PathToolViewModel()
                {
                    Brush = new SolidColorBrushViewModel(new ColorViewModel(255, 0, 0, 0)),
                    Pen = new PenViewModel(new SolidColorBrushViewModel(new ColorViewModel(255, 0, 0, 0)), 2),
                    IsStroked = true,
                    IsFilled = true,
                    HitRadius = 6,
                    FillRule = FillRule.EvenOdd,
                    IsClosed = true,
                    PreviousMode = PathToolMode.Line,
                    Mode = PathToolMode.Line,
                    TryToConnect = true,
                    LineShapeTool = new LineShapeToolViewModel()
                    {
                        Pen = new PenViewModel(new SolidColorBrushViewModel(new ColorViewModel(255, 0, 0, 0)), 2),
                        IsStroked = true,
                        HitRadius = 6,
                        TryToConnect = true
                    },
                    CubicBezierShapeTool = new CubicBezierShapeToolViewModel()
                    {
                        Brush = new SolidColorBrushViewModel(new ColorViewModel(255, 0, 0, 0)),
                        Pen = new PenViewModel(new SolidColorBrushViewModel(new ColorViewModel(255, 0, 0, 0)), 2),
                        IsStroked = true,
                        IsFilled = false,
                        HitRadius = 6,
                        TryToConnect = true
                    },
                    QuadraticBezierShapeTool = new QuadraticBezierShapeToolViewModel()
                    {
                        Brush = new SolidColorBrushViewModel(new ColorViewModel(255, 0, 0, 0)),
                        Pen = new PenViewModel(new SolidColorBrushViewModel(new ColorViewModel(255, 0, 0, 0)), 2),
                        IsStroked = true,
                        IsFilled = false,
                        HitRadius = 6,
                        TryToConnect = true
                    }
                },
                new RectangleToolViewModel()
                {
#if true
                    Brush = new SolidColorBrushViewModel(new ColorViewModel(255, 0, 0, 0)),
#else
                    Brush = new LinearGradientBrushViewModel(
                        new ObservableCollection<GradientStopViewModel>()
                        {
                            new GradientStopViewModel(new ColorViewModel(255, 0, 255, 0), 0),
                            new GradientStopViewModel(new ColorViewModel(255, 0, 0, 255), 1),
                        },
                        GradientSpreadMethod.Pad,
                        new RelativePointViewModel(0, 0, RelativeUnit.Relative),
                        new RelativePointViewModel(1, 1, RelativeUnit.Relative)),
#endif
                    Pen = new PenViewModel(new SolidColorBrushViewModel(new ColorViewModel(255, 0, 0, 0)), 2),
                    IsStroked = true,
                    IsFilled = true,
                    RadiusX = 4,
                    RadiusY = 4,
                    HitRadius = 6,
                    TryToConnect = true
                },
                new EllipseToolViewModel()
                {
                    Brush = new SolidColorBrushViewModel(new ColorViewModel(255, 0, 0, 0)),
                    Pen = new PenViewModel(new SolidColorBrushViewModel(new ColorViewModel(255, 0, 0, 0)), 2),
                    IsStroked = true,
                    IsFilled = true,
                    HitRadius = 6,
                    TryToConnect = true
                }
            };

            canvas.Tool = canvas.Tools[1];

            return canvas;
        }

        public CanvasViewModel Open(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            var json = File.ReadAllText(path);

            var canvas = JsonConvert.DeserializeObject<CanvasViewModel>(json, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Objects,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                NullValueHandling = NullValueHandling.Ignore,
            });

            return canvas;
        }

        public void Save(string path, CanvasViewModel canvas)
        {
            var json = JsonConvert.SerializeObject(canvas, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Objects,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                NullValueHandling = NullValueHandling.Ignore,
            });

            File.WriteAllText(path, json);
        }
    }
}
