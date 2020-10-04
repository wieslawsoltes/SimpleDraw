using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using SimpleDraw.Avalonia;
using SimpleDraw.Controls;
using SimpleDraw.ViewModels.Containers;
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
            SimpleCanvas.App = new AvaloniaSimpleDrawApp();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var canvas = SimpleCanvas.App.Open("canvas.json") ?? SimpleCanvas.App.New();

                desktop.MainWindow = new MainWindow
                {
                    DataContext = canvas
                };

                desktop.Exit += (sender, e) =>
                {
#if true
                    SimpleCanvas.App.Save("canvas.json", desktop.MainWindow.DataContext as CanvasViewModel); 
#endif
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
