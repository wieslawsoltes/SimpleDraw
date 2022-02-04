using System;

namespace SimpleDraw.ViewModels.Tools;

[Flags]
public enum ToolKeyModifiers
{
    None = 0,
    Alt = 1,
    Control = 2,
    Shift = 4,
    Meta = 8
}