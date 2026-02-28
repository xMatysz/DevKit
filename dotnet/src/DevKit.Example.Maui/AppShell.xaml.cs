using DevKit.Example.Maui.Pages.Login;
using DevKit.Example.Maui.Pages.TodoList;

namespace DevKit.Example.Maui;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute($"//{nameof(LoginPage)}", typeof(LoginPage));
        Routing.RegisterRoute($"//{nameof(TodoListPage)}", typeof(TodoListPage));
    }
}
