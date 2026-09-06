namespace SNS.API.Helpers;

public static class CookieFactory
{
    public const string RefreshTokenCookieName = "RefreshToken";
    public static CookieOptions CreateRefreshTokenCookie(bool rememberMe)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,

            IsEssential = true,

            Expires = DateTimeOffset.UtcNow.AddDays(7)
        };
    }
}
