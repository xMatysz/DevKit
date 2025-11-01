using DevKit.Api.Configuration;
using DevKit.Api.Exceptions;
using DevKit.Api.Logging;
using DevKit.Example.Application;
using DevKit.MediatR;
using DevKit.MediatR.Pipelines;
using DevKit.Otel;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateEmptyBuilder(
    new WebApplicationOptions
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

builder.Configuration.AddDevKitConfiguration("DevKit", builder.Environment.EnvironmentName);
builder.Services.AddDevKitOtel(builder.Configuration);
builder.UseDevKitLogging();
builder.Services.AddOpenTelemetry().WithTracing(tr => tr.AddSource(ApplicationDiagnostics.ActivitySourceName));
builder.Services.AddDevKitMediatR(typeof(ExampleQuery).Assembly);
builder.Services.AddDbContextPool<IDevKitDbContext, TestDbContext>((sp, dbOptions) =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    dbOptions.UseNpgsql(connectionString);
});

builder.Services.AddDevKitExceptionHandlers();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseExceptionHandler();

await app.RunAsync();
