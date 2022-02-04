﻿using System;
using Avalonia;
using Avalonia.ReactiveUI;

namespace SimpleDraw.Desktop
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseSkia()
                .UseReactiveUI()
                .LogToTrace();
    }
}
