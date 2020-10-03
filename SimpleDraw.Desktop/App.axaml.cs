using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
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
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var canvas = AvaloniaApp.Open("canvas.json") ?? AvaloniaApp.Create();

                desktop.MainWindow = new MainWindow
                {
                    DataContext = canvas
                };

                desktop.Exit += (sender, e) =>
                {
#if true
                    AvaloniaApp.Save("canvas.json", desktop.MainWindow.DataContext as CanvasViewModel); 
#endif
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
