using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Identity.Users.Enums;
using Microsoft.EntityFrameworkCore;

namespace SNS.Infrastructure.Shared.BackgroundJobs;

public sealed class UserHardDeletionBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<UserHardDeletionBackgroundService> _logger;

    public UserHardDeletionBackgroundService(
        IServiceProvider _serviceProvider,
        ILogger<UserHardDeletionBackgroundService> logger)
    {
        _serviceProvider = _serviceProvider;
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
                var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

                // 3️⃣ تحديد نقطة الحسم (تاريخ اليوم ناقص 60 يوماً)
                var thresholdDate = DateTime.UtcNow.AddDays(-60);

                // 4️⃣ ضربة الحسم النفاثة $O(1)$ بقوة الـ ExecuteDeleteAsync 💥
                // حذف الحسابات المعطلة التي تجاوزت فترة النعمة (60 يوماً) مباشرة من الـ DB
                int deletedUsersCount = await dbContext.Users
                    .Where(u => u.Status != UserStatus.Active && u.DeactivatedAt.HasValue && u.DeactivatedAt.Value <= thresholdDate)
                    .ExecuteDeleteAsync(stoppingToken);

                _logger.LogInformation("Successfully hard deleted {Count} deactivated users from the system.", deletedUsersCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while executing user hard deletion.");
            }
        }
    }
}