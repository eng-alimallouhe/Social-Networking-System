using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.Identity.SecuritySettings.MfaManagement.Queries.GetUserPasskeys;

public record GetUserPasskeysQuery() : IQuery<IEnumerable<PasskeyDto>>;
