using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.Identity.SecuritySettings.Queries.GetUserPasskeys;

public record GetUserPasskeysQuery() : IQuery<IEnumerable<PasskeyDto>>;
