namespace LinkForge.API.Endpoints.Auth;

public static class AuthEndpointsSettings
{
    public static string[] Tags => [ "auth", ];
    
    public const string RegisterEndpointPattern = "auth/register";
    public const string RegisterEndpointName = "auth-register";
    
    public const string LoginEndpointPattern = "auth/login";
    public const string LoginEndpointName = "auth-login";
    
    public const string RefreshTokenEndpointPattern = "auth/refresh-token";
    public const string RefreshTokenEndpointName = "auth-refresh-token";
}