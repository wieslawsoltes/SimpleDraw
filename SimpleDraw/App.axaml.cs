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

            var canvas = SimpleCanvas.App.Open("canvas.json") ?? SimpleCanvas.App.New();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = canvas
                };

                desktop.Exit += (sender, e) =>
                {
                    SimpleCanvas.App.Save("canvas.json", desktop.MainWindow.DataContext as CanvasViewModel);
                };
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime single)
            {
                single.MainView = new MainView
                {
                    DataContext = canvas
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
