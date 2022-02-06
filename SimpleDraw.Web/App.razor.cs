using Avalonia.ReactiveUI;
using Avalonia.Web.Blazor;

namespace SimpleDraw.Web;

public partial class App
{
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        WebAppBuilder.Configure<SimpleDraw.App>()
            .UseReactiveUI()
            .SetupWithSingleViewLifetime();
    }
}
