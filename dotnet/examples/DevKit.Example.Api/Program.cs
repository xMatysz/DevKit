using DevKit.Api.Configuration;
using DevKit.Api.Logging;
using DevKit.Otel;

var builder = WebApplication.CreateEmptyBuilder(new WebApplicationOptions
{
    Args = args,
});

var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
if (!string.IsNullOrEmpty(envName))
{
    builder.Environment.EnvironmentName = envName;
}

builder.WebHost.UseKestrelCore();

// TODO: why it's not working with docker if not specified?
builder.WebHost.ConfigureKestrel(cfg => cfg.ListenAnyIP(3000));
builder.Services.AddRoutingCore();

builder.Configuration.AddDevKitConfiguration(builder.Environment.EnvironmentName);
builder.Services.AddDevKitOtel(builder.Configuration);
builder.UseDevKitLogging();

var app = builder.Build();

app.UseHttpLogging();

app.MapGet("/test", () => "Hello World!");
app.MapPost("/test", (object test) => "Hello World!");

await app.RunAsync();
