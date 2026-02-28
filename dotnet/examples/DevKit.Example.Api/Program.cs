using DevKit.Api.Configuration;
using DevKit.Api.Exceptions;
using DevKit.Api.Logging;
using DevKit.Example.Api.Endpoints.DocsEndpoints;
using DevKit.Example.Application;
using DevKit.MediatR;
using DevKit.MediatR.Pipelines;
using DevKit.Otel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
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

builder.Services.AddOpenApi();
builder.WebHost.UseKestrelCore();

// TODO: why it's not working with docker if not specified?
builder.WebHost.ConfigureKestrel(cfg => cfg.ListenAnyIP(3000));
builder.Services.AddRoutingCore();

builder.Host.UseDefaultServiceProvider(sp =>
{
    sp.ValidateOnBuild = true;
    sp.ValidateScopes = true;
});

builder.UseDevKitConfiguration();

builder.Services.AddDevKitOtel(
    builder.Configuration,
    traceBuilder: trace => { trace.AddSource(ApplicationDiagnostics.ActivitySourceName); });

builder.UseDevKitLogging();

builder.Services.AddDevKitMediatR(typeof(ExampleQuery).Assembly);
builder.Services.AddDbContextPool<IDevKitDbContext, TestDbContext>((sp, dbOptions) =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    dbOptions.UseNpgsql(connectionString);
});

builder.Services.AddDevKitExceptionHandlers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.Configure<RouteOptions>(options => options.SetParameterPolicy<RegexInlineRouteConstraint>("regex"));

builder.Services.AddSwaggerGen(swag =>
{
    swag.AddSecurityDefinition(
        JwtBearerDefaults.AuthenticationScheme,
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "JWT Authorization header using the Bearer scheme.",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            BearerFormat = "JWT",
        });

    swag.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme, doc)] = [],
    });
});

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI();

app.MapOpenApi();
var group = app.MapGroup("v1");

group
    .MapGroup("/devkit-endpoint")
    .MapGetEmpty();

await app.RunAsync();
