using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
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
                var canvas = new CanvasViewModel()
                {
                    Width = 840,
                    Height = 600,
                    Shapes = new ObservableCollection<ShapeBaseViewModel>()
                };

                canvas.Tools = new ObservableCollection<ToolBase>()
                {
                    new NoneTool(),
                    new LineTool()
                    {
                        Pen = new PenViewModel(new SolidColorBrushViewModel(new ColorViewModel(255, 0, 0, 0)), 2),
                        IsStroked = true
                    },
                    new RectangleTool()
                    {
                        Brush = new SolidColorBrushViewModel(new ColorViewModel(255, 0, 0, 0)),
                        Pen = new PenViewModel(new SolidColorBrushViewModel(new ColorViewModel(255, 0, 0, 0)), 2),
                        IsStroked = true,
                        IsFilled = true,
                        RadiusX = 4,
                        RadiusY = 4
                    }
                };
                canvas.Tool = canvas.Tools[2];

                desktop.MainWindow = new MainWindow
                {
                    DataContext = canvas
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
