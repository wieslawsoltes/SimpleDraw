using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Headless;
using Avalonia.ReactiveUI;
using Avalonia.Threading;

namespace SimpleDraw
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                if (args[0] == "--vnc")
                {
                    BuildAvaloniaApp().StartWithHeadlessVncPlatform(null, 5901, args, ShutdownMode.OnMainWindowClose);
                }
                else if (args[0] == "--headless")
                {
                    BuildAvaloniaApp().UseHeadless(true).StartWithClassicDesktopLifetime(args);
                }
            }
            else
            {
                BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            }
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug()
                .UseReactiveUI();
    }
}
