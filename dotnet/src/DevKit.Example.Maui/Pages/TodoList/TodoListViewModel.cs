using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevKit.Maui;
using DevKit.Maui.Services;

namespace DevKit.Example.Maui.Pages.TodoList;

public sealed partial class TodoListViewModel(AuthenticationService authenticationService) : DevKitViewModel
{
    public override string Title => "Todos";

    [ObservableProperty]
    private List<TodoModel> _todoModels =
    [
        new TodoModel(1, true, "Work"),
        new TodoModel(2, false, "Sleep")
    ];

    [RelayCommand]
    private Task LogoutAsync()
    {
        return authenticationService.LogoutAsync();
    }
}
