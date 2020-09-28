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
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var canvas = default(CanvasViewModel);

                if (File.Exists("canvas.json"))
                {
                    var json = File.ReadAllText("canvas.json");

                    canvas = JsonConvert.DeserializeObject<CanvasViewModel>(json, new JsonSerializerSettings()
                    {
                        Formatting = Formatting.Indented,
                        TypeNameHandling = TypeNameHandling.Objects,
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                        ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                        NullValueHandling = NullValueHandling.Ignore,
                    });
                }

                if (canvas == null)
                {
                    canvas = new CanvasViewModel()
                    {
                        Width = 840,
                        Height = 600,
                        Items = new ObservableCollection<ViewModelBase>(),
                        Selected = new ObservableCollection<ViewModelBase>(),
                        Decorators = new ObservableCollection<ViewModelBase>()
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
                            Pen = new PenViewModel(new SolidColorBrushViewModel(new ColorViewModel(255, 0, 0, 0)), 2),
                            IsStroked = true,
                            HitRadius = 6,
                            TryToConnect = true
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
                        }
                    };
                    canvas.Tool = canvas.Tools[3];
                }

                desktop.MainWindow = new MainWindow
                {
                    DataContext = canvas
                };

                desktop.Exit += (sender, e) =>
                {
                    var json = JsonConvert.SerializeObject(canvas, new JsonSerializerSettings()
                    {
                        Formatting = Formatting.Indented,
                        TypeNameHandling = TypeNameHandling.Objects,
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                        ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                        NullValueHandling = NullValueHandling.Ignore,
                    });

                    File.WriteAllText("canvas.json", json);
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
