﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SimpleDraw.Views;

public class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}

