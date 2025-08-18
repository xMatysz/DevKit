using System.Diagnostics;
using DevKit.Api.Configuration;
using DevKit.Api.Logging;
using DevKit.Otel;
using Serilog;
using Serilog.Context;
using Serilog.Core.Enrichers;

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

app.UseSerilogRequestLogging();

app.MapGet("/test", () => "Hello World!");
app.MapPost("/test", (ILogger<Program> logger, object test) =>
{
    using var scope = Activity.Current.Source.CreateActivity("test", ActivityKind.Server).Start();
    var sc = LogContext.Push(
        new PropertyEnricher("requestId", Guid.NewGuid().ToString()),
        new PropertyEnricher("correlationId", Guid.NewGuid().ToString()));
    logger.LogTrace("Important note-0");
    logger.LogDebug("Important note0");
    logger.LogInformation("Important note");
    logger.LogWarning("Important note2");
    logger.LogError("Important note3");
    logger.LogCritical("Important note4");
    scope.Dispose();
    sc.Dispose();
    return "Hello World!";
});

await app.RunAsync();
