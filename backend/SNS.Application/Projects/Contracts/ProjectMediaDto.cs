namespace SNS.Application.Projects.Contracts;

public record ProjectMediaDto(
    Guid MediaId,
    string MediaUrl,
    string MediaType,
    int Order,
    DateTime CreatedAt
);
