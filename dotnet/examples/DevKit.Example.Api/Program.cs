using DevKit.Api.Configuration;
using DevKit.Api.Exceptions;
using DevKit.Api.Logging;
using DevKit.Example.Application;
using DevKit.MediatR;
using DevKit.MediatR.Pipelines;
using DevKit.Otel;
using MediatR;
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
app.MapGet(
    "/query",
    async (ISender sender) => await sender.Send(
        new ExampleQuery
        {
            TodoId = "Query",
        }));

    app.MapHealthChecks("_health")
        .ShortCircuit();

app.MapGet(
    "/query-invalid",
    async (ISender sender) =>
    {
        var random = Random.Shared.Next(1, 4);
        var body = random switch
        {
            1 => string.Empty,
            2 => null,
            3 => " ",
        };

        return await sender.Send(
            new ExampleQuery
            {
                TodoId = body,
            });
    });

app.MapPost(
    "/test",
    async (ISender sender) => await sender.Send(
        new ExampleCommand
        {
            Test = "Command",
        }));

await app.RunAsync();
