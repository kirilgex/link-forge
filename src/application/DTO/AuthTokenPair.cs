namespace LinkForge.Application.DTO;

public class AuthTokenPair(string accessToken, string refreshToken)
{
    public string AccessToken { get; init; } = accessToken ?? throw new ArgumentNullException(nameof(accessToken));
    public string RefreshToken { get; init; } = refreshToken ?? throw new ArgumentNullException(nameof(refreshToken));
}
