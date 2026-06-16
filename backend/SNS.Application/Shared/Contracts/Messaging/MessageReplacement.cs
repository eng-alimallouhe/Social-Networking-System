using SNS.Domain.Identity.Shared.Enums;

namespace SNS.Application.Shared.Contracts.Messaging;

public sealed record MessageReplacement(
    ReplacementKey Key,
    string Value);
