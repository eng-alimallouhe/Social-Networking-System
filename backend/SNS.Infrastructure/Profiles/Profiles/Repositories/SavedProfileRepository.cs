using Microsoft.EntityFrameworkCore;
using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Profiles.Profiles.Repositories;

public class SavedProfileRepository : Repository<SavedProfile>
{
    public SavedProfileRepository(SNSDbContext context) : base(context)
    {
    }
}
