using System.Text;

using LinkForge.API;
using LinkForge.API.Endpoints;
using LinkForge.Application;
using LinkForge.Application.Settings;
using LinkForge.Infrastructure.PersistentStorage;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Auth:Issuer"],
            ValidAudience = builder.Configuration["Auth:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Auth:AccessToken:SecretKey"]!)),
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.DefaultApiVersion = ApiVersions.V0;
    options.AssumeDefaultVersionWhenUnspecified = true;
});

builder.Services
    .Configure<DatabaseSettings>(builder.Configuration.GetSection("Database"))
    .Configure<HashingSettings>(builder.Configuration.GetSection("Hashing"))
    .Configure<AuthSettings>(builder.Configuration.GetSection("Auth"));

builder.Services
    .AddPersistentStorageServices()
    .AddApplicationLayerServices();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

var apiSet_v0 = app
    .NewApiVersionSet()
    .HasApiVersion(ApiVersions.V0)
    .ReportApiVersions()
    .Build();

var group_v0 = app
    .MapGroup("api/v{version:apiVersion}")
    .WithApiVersionSet(apiSet_v0);

group_v0
    .MapRegisterEndpoint(ApiVersions.V0)
    .MapLoginEndpoint(ApiVersions.V0)
    .MapRefreshTokenEndpoint(ApiVersions.V0)
    .MapPostLinkEndpoint(ApiVersions.V0)
    .MapGetLinkEndpoint(ApiVersions.V0);

await app.Services.ConfigurePersistentStorageAsync();

app.Run();
