using DevKit.Api.Logging;
using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateEmptyBuilder(new WebApplicationOptions
{
    Args = args,
});

var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
if (!string.IsNullOrEmpty(envName))
{
    builder.Environment.EnvironmentName = envName;
}

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: false)
    .AddEnvironmentVariables();

builder.WebHost.UseKestrelCore();

// TODO: why it's not working with docker if not specified?
builder.WebHost.ConfigureKestrel(cfg => cfg.ListenAnyIP(3000));
builder.Services.AddRoutingCore();

builder.UseDevKitLogging(devKitLoggerOptionsAction: devKitOpt =>
{
    devKitOpt.ResourceAttributes
        .Add("service.name", "DevKitApp");
});

const HttpLoggingFields httpLoggingFields = HttpLoggingFields.ResponseStatusCode |
                                            HttpLoggingFields.Duration |
                                            HttpLoggingFields.RequestBody |
                                            HttpLoggingFields.RequestQuery |
                                            HttpLoggingFields.RequestMethod |
                                            HttpLoggingFields.RequestPath;

builder.Services.AddHttpLogging(opt => { opt.LoggingFields = httpLoggingFields; });

var app = builder.Build();

app.UseHttpLogging();

app.MapGet("/test", () => "Hello World!");
app.MapPost("/test", (object test) => "Hello World!");

await app.RunAsync();
