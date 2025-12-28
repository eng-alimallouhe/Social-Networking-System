namespace SNS.Application.Abstractions.Authentication;

public interface ITokenReaderService
{
    string? GetEmail(string accessToken);

    Guid? GetUserIdFromToken(string accessToken);

    Guid? GetSessionIdFromToken(string accessToken);
}
