using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SNS.Domain.Identity.Users.Constants;
using SNS.Domain.Identity.Users.Enums;
using SNS.Infrastructure.Persistence;
using SNS.Shared.Exceptions;

namespace SNS.Infrastructure.Shared.BackgroundJobs;

public sealed class UserHardDeletionBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<UserHardDeletionBackgroundService> _logger;

    public UserHardDeletionBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<UserHardDeletionBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("User Hard Deletion Background Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            // 1️⃣ حساب الوقت المتبقي حتى الساعة 3 صباحاً
            var now = DateTime.UtcNow;
            var nextRun = DateTime.UtcNow.Date.AddHours(3); // الساعة 3 صباحاً اليوم

            if (now > nextRun)
            {
                nextRun = nextRun.AddDays(1); // إذا عبرنا الساعة 3، ننتظر لـ 3 صباحاً الغد
            }

            var delay = nextRun - now;
            _logger.LogInformation("Next cleanup execution scheduled at: {Time}. Delaying for {Delay}", nextRun, delay);

            // الانتظار الآمن حتى يحين الوقت أو يتم إيقاف السيرفر
            await Task.Delay(delay, stoppingToken);

            try
            {
                _logger.LogInformation("Executing scheduled user hard deletion...");

                // 2️⃣ إنشاء Scoped Context للتعامل مع قاعدة البيانات بنقاء
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<SNSDbContext>();

                var thresholdDate = DateTime.UtcNow.AddDays(-60);

                var usersToDelet = await dbContext.Users
                    .Where(u => u.Status != UserStatus.Active && u.DeactivatedAt.HasValue && u.DeactivatedAt.Value <= thresholdDate)
                    .Select(u => new
                    {
                        u.Id,
                        u.PurgeAllContentOnHardDelete
                    })
                    .ToListAsync(stoppingToken);

                if (!usersToDelet.Any())
                {
                    _logger.LogInformation("There is not users to delete!");
                }

                var userToPurgeAllContentIds = usersToDelet.Where(
                    u => u.PurgeAllContentOnHardDelete).Select(u => u.Id).ToList();

                var usersTransferOfOwnershipIds = usersToDelet.Where(
                    u => !u.PurgeAllContentOnHardDelete).Select(u => u.Id).ToList();

                await TransferOfOwnershipAsync(usersTransferOfOwnershipIds, dbContext);
                await PurgeUsersContentAsync(userToPurgeAllContentIds, dbContext);

                _logger.LogInformation("Successfully hard deleted {Count} deactivated users from the system.", usersToDelet.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while executing user hard deletion.");
            }
        }
    }

    private async Task PurgeUsersContentAsync(
        List<Guid> usersIds, 
        SNSDbContext dbContext, 
        CancellationToken cancellationToken = default)
    {
        await dbContext.UserArchives.Where(u => usersIds.Contains(u.TargetId))
            .ExecuteDeleteAsync(cancellationToken);
        
        var profilesIds = await dbContext.Profiles
            .Where(p => usersIds.Contains(p.UserId))
            .Select(p => p.Id)
            .ToListAsync();

        if (!profilesIds.Any())
        {
            return;
        }

        // Profile Context:
        await dbContext.SavedProfiles.Where(ps => profilesIds.Contains(ps.SaverId) || profilesIds.Contains(ps.SavedId))
            .ExecuteDeleteAsync(cancellationToken);
        
        await dbContext.ProfileViews.Where(ps => profilesIds.Contains(ps.ViewedId) || profilesIds.Contains(ps.ViewerId))
            .ExecuteDeleteAsync(cancellationToken);
        
        await dbContext.Blocks.Where(ps => profilesIds.Contains(ps.BlockedId) || profilesIds.Contains(ps.BlockerId))
            .ExecuteDeleteAsync(cancellationToken);
        
        
        await dbContext.Follows.Where(ps => profilesIds.Contains(ps.FollowerId) || profilesIds.Contains(ps.FollowingId))
            .ExecuteDeleteAsync(cancellationToken);
        
        await dbContext.ReputationLedgers.Where(ps => profilesIds.Contains(ps.ProfileId))
            .ExecuteDeleteAsync(cancellationToken);


        // Content Managment Context
        await dbContext.Posts.Where(p => profilesIds.Contains(p.AuthorId))
            .ExecuteDeleteAsync(cancellationToken);

        var commentsWithRepliesIds = await dbContext.Comments
            .Where(c => profilesIds.Contains(c.AuthorId))
            .Where(c => dbContext.Comments.Any(r => r.ParentCommentId == c.Id))
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        await dbContext.Comments
            .Where(c => profilesIds.Contains(c.AuthorId))
            .Where(c => !commentsWithRepliesIds.Contains(c.Id))
            .ExecuteDeleteAsync(cancellationToken);

        await dbContext.Comments
            .Where(c => commentsWithRepliesIds.Contains(c.Id))
            .ExecuteUpdateAsync(s => s
                .SetProperty(c => c.IsActive, false)
                .SetProperty(c => c.Content, "[Comment deleted]"));


        await dbContext.CommunityInvitations.Where(ci => profilesIds.Contains(ci.InviteeId) || profilesIds.Contains(ci.InviterId))
            .ExecuteDeleteAsync(cancellationToken);

        await dbContext.Communities.Where(p => profilesIds.Contains(p.OwnerId))
            .ExecuteDeleteAsync(cancellationToken);


        // Discussions Context
        await dbContext.Discussions.Where(d => profilesIds.Contains(d.AuthorId))
            .ExecuteDeleteAsync(cancellationToken);


        //

        //
    }

    private async Task TransferOfOwnershipAsync(List<Guid> usersIds, SNSDbContext dbContext)
    {
        var defualtProfile = await dbContext.Profiles
            .FirstOrDefaultAsync(u => u.Id == SystemProfiles.GhostProfileId);

        if (defualtProfile == null)
        {
            throw new ResourceNotFoundException("Default Profile Is Not Founded");
        }

    }
}