namespace SNS.Application.DTOs.Authentication.Responses;

public class AuthenticationResultDto
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
