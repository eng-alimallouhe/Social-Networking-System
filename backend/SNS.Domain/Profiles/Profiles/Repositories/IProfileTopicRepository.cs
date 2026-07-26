using SNS.Domain.ContentManagement.Posts.Entities;
using SNS.Domain.Profiles.Profiles.Relations;
using System.Linq.Expressions;

namespace SNS.Domain.Profiles.Profiles.Repositories;

public interface IProfileTopicRepository
{
    Task IncrementScoreAsync(Expression<Func<ProfileTopic, bool>> predicate, double value, CancellationToken cancellationToken = default);
}
