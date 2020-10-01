using System.Collections.ObjectModel;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Newtonsoft.Json;
using SimpleDraw.ViewModels;
using SimpleDraw.Views;

namespace SimpleDraw
{
    public class App : Application
    {
        public static CanvasViewModel Open(string path)
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

        public static void Save(string path, CanvasViewModel canvas)
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

        public static CanvasViewModel Create()
        {
            var canvas = new CanvasViewModel()
            {
                Width = 840,
                Height = 600,
                Items = new ObservableCollection<ViewModelBase>()
            };

            canvas.Tools = new ObservableCollection<ToolBaseViewModel>()
            {
                new NoneToolViewModel(),
                new SelectionToolViewModel()
                {
                    HitRadius = 6
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
                    FillRule = PathFillRule.EvenOdd,
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
                    Brush = new SolidColorBrushViewModel(new ColorViewModel(255, 0, 0, 0)),
                    //Brush = new LinearGradientBrushViewModel(
                    //    new ObservableCollection<GradientStopViewModel>()
                    //    {
                    //        new GradientStopViewModel(new ColorViewModel(255, 0, 0, 0), 0),
                    //        new GradientStopViewModel(new ColorViewModel(255, 255, 255, 255), 1),
                    //    },
                    //    GradientSpreadMethod.Pad,
                    //    new RelativePointViewModel(0, 0, ViewModels.RelativeUnit.Relative),
                    //    new RelativePointViewModel(1, 1, ViewModels.RelativeUnit.Relative)),
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

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var canvas = Open("canvas.json") ?? Create();

                desktop.MainWindow = new MainWindow
                {
                    DataContext = canvas
                };

                desktop.Exit += (sender, e) =>
                {
                    Save("canvas.json", desktop.MainWindow.DataContext as CanvasViewModel);
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
