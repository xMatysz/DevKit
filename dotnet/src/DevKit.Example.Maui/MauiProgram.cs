using CommunityToolkit.Maui;
using DevKit.Example.Maui.Pages;
using DevKit.Example.Maui.Pages.Login;
using DevKit.Example.Maui.Pages.TodoList;
using DevKit.Maui;
using Microsoft.Extensions.Logging;

namespace DevKit.Example.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseDevKit(auth => auth.LoggoutFallback = "//LoginPage")
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<LoginViewModel>();

        builder.Services.AddTransient<TodoListPage>();
        builder.Services.AddTransient<TodoListViewModel>();

        builder.Logging.AddDebug();
        return builder.Build();
    }
}
