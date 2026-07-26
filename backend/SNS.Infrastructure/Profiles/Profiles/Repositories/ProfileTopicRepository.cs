using Microsoft.EntityFrameworkCore;
using SNS.Domain.ContentManagement.Posts.Entities;
using SNS.Domain.Profiles.Profiles.Relations;
using SNS.Domain.Profiles.Profiles.Repositories;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;
using System.Linq.Expressions;

namespace SNS.Infrastructure.Repositories.ProfileContext;

public class ProfileTopicRepository 
    : Repository<ProfileTopic>, 
    IProfileTopicRepository
{
    public ProfileTopicRepository(SNSDbContext context) : base(context) 
    { 
    }

    public async Task IncrementScoreAsync(Expression<Func<ProfileTopic, bool>> predicate, double value, CancellationToken cancellationToken = default)
    {
        await _dbSet
            .Where(predicate)
            .ExecuteUpdateAsync(s => s.SetProperty(pt => pt.Score, pt => pt.Score + value), cancellationToken);
    }
}
