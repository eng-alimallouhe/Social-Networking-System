using SNS.Domain.Discussions.Problems.Enums;
using SNS.Domain.Discussions.Shared.Enums;

namespace SNS.Application.Search.Discussions.Problems.Queries;

public sealed record ProblemSearchQuery(
    string? SearchTerm,
    DateTime? MinCreatedAt,
    DateTime? MaxCreatedAt,
    DifficultyLevel? Level,
    ProblemStatus? Status,
    int Page = 1,
    int PageSize = 10);
