using System.Text;

using LinkForge.API.Endpoints.Auth;
using LinkForge.API.Endpoints.Links;
using LinkForge.Application;
using LinkForge.Application.Auth.Settings;
using LinkForge.Application.Links.Settings;
using LinkForge.Infrastructure.PersistentStorage;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using Scalar.AspNetCore;

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

builder.Services
    .Configure<DatabaseSettings>(builder.Configuration.GetSection("Database"))
    .Configure<HashingSettings>(builder.Configuration.GetSection("Hashing"))
    .Configure<AuthSettings>(builder.Configuration.GetSection("Auth"));

builder.Services
    .AddPersistentStorageServices()
    .AddApplicationLayerServices();

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app
    .MapRegisterEndpoint()
    .MapLoginEndpoint()
    .MapRefreshTokenEndpoint()
    .MapPostLinkEndpoint()
    .MapGetLinkEndpoint();

await app.Services.ConfigurePersistentStorageAsync();

app.Run();
