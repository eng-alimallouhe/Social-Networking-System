namespace SNS.Application.Projects.Contracts;

public record ProjectRatingDto(
    Guid RatingId,
    int RatingValue,
    string? Comment,
    DateTime CreatedAt,
    Guid ProfileId,
    string DisplayName,
    string? Specialization,
    string? ProfileImageUrl
);
