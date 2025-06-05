using LinkForge.API;
using LinkForge.API.Endpoints;
using LinkForge.Application;
using LinkForge.Application.Settings;
using LinkForge.Infrastructure.PersistentStorage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.DefaultApiVersion = ApiVersions.V0;
    options.AssumeDefaultVersionWhenUnspecified = true;
});

builder.Services
    .Configure<DatabaseSettings>(builder.Configuration.GetSection("Database"))
    .Configure<HashingSettings>(builder.Configuration.GetSection("Hashing"));

builder.Services
    .AddPersistentStorageServices()
    .AddApplicationLayerServices();

var app = builder.Build();

app.UseHttpsRedirection();

var apiSet_v0 = app
    .NewApiVersionSet()
    .HasApiVersion(ApiVersions.V0)
    .ReportApiVersions()
    .Build();

var group_v0 = app
    .MapGroup("api/v{version:apiVersion}")
    .WithApiVersionSet(apiSet_v0);

group_v0
    .MapPostLinkEndpoint(ApiVersions.V0)
    .MapGetLinkEndpoint(ApiVersions.V0);

await app.Services.ConfigurePersistentStorageAsync();

app.Run();
