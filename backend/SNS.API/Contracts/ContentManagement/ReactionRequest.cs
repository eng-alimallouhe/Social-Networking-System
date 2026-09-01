using SNS.Domain.ContentManagement.Shared.Enums;

namespace SNS.API.Contracts.ContentManagement;

public sealed record ReactionRequest(ReactionType Type);
