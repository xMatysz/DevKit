using DevKit.Maui.Services;

namespace DevKit.Maui;

// All the code in this file is included in all platforms.
public static class MauiAppBuilderExtensions
{
    public static MauiAppBuilder UseDevKit(
        this MauiAppBuilder builder,
        Action<AuthenticationOptions> authenticationOptions)
    {
        builder.Services.Configure(authenticationOptions);
        builder.Services.AddSingleton(SecureStorage.Default);
        builder.Services.AddSingleton<AuthenticationService>();
        return builder;
    }
}
