using Microsoft.Extensions.Options;

namespace DevKit.Maui.Services;

public sealed class AuthenticationService
{
    private const string UserCredentialsKey = "creds";

    private readonly ISecureStorage _secureStorage;
    private readonly IOptions<AuthenticationOptions> _options;

    public AuthenticationService(
        ISecureStorage secureStorage,
        IOptions<AuthenticationOptions> options)
    {
        _secureStorage = secureStorage;
        _options = options;
    }

    public async Task LoginAsync(string username, string password)
    {
        var result = await WebAuthenticator.Default.AuthenticateAsync(
            new WebAuthenticatorOptions
            {
                Url = new Uri(
                    "https://us-east-1ej7xmh53q.auth.us-east-1.amazoncognito.com/login/continue?client_id=7eh4qok0pqq649ppefmjgbnf5r&redirect_uri=myapp%3A%2F%2Fexample&response_type=code&scope=email+openid+phone"),
                CallbackUrl = new Uri("myapp://")
            });
        await _secureStorage.SetAsync(UserCredentialsKey, $"{username}:{password}");
    }

    public async Task<string?> CheckCreds() => await _secureStorage.GetAsync(UserCredentialsKey);

    public Task LogoutAsync()
    {
        _secureStorage.RemoveAll();
        return Shell.Current.GoToAsync(_options.Value.LoggoutFallback);
    }
}
