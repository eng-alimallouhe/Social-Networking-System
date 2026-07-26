using SNS.Shared.Results;

namespace SNS.Application.Profiles.SocialGraph.Abstractions;

public interface ISocialPolicyService
{
    Task<Result> IsRelationshipAllowedAsync(Guid firstRelationshipPart, Guid secondRelationshipPart, CancellationToken cancellationToken = default);
}
