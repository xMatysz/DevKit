using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevKit.Maui;
using DevKit.Maui.Services;

namespace DevKit.Example.Maui.Pages;

public partial class LoginViewModel(AuthenticationService authService) : DevKitViewModel
{
    public override string Title => "Login";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string? _login;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string? _password;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private bool _isLoginValid;

    private bool CanLoginAsync()
    {
        return !string.IsNullOrWhiteSpace(Login) &&
               IsLoginValid &&
               !string.IsNullOrWhiteSpace(Password);
    }

    [RelayCommand(CanExecute = nameof(CanLoginAsync))]
    private async Task LoginAsync()
    {
        await authService.LoginAsync(Login!, Password!);
        await Shell.Current.GoToAsync("//TodoListPage");
    }

    [RelayCommand]
    private async Task GetCredsAsync()
    {
        await Shell.Current.DisplayAlert("CREDENTIALS", await authService.CheckCreds() ?? "NULL", "Ok");
    }
}
