namespace SNS.Application.Identity.SecuritySessions.Login.Contracts;

public sealed record OtpChallengeDto(
    Guid UserId,
    string ChallengeToken
);