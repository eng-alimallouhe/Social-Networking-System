using Quartz;
using SNS.Application.Shared.Abstractions.BackgroundJobs;

namespace SNS.Infrastructure.Shared.BackgroundJobs;

public class QuartzJobSchedulerService : IJobSchedulerService
{
    private readonly ISchedulerFactory _schedulerFactory;

    public QuartzJobSchedulerService(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    public async Task TriggerExportJobAsync(Guid requestId)
    {
        var scheduler = await _schedulerFactory.GetScheduler();

        var job = JobBuilder.Create<GdprExportJob>()
            .WithIdentity($"GdprExport_{requestId}", "GDPR")
            .UsingJobData("RequestId", requestId) // تمرير المعاملات بأمان 📦
            .Build();

        // 2️⃣ إنشاء Trigger يطلق الـ Job "فوراً" ولمرة واحدة فقط
        var trigger = TriggerBuilder.Create()
            .WithIdentity($"GdprExportTrigger_{requestId}", "GDPR")
            .StartNow() // التنفيذ فوراً في الخلفية
            .Build();

        // 3️⃣ جدولة وتشغيل الـ Job داخل محرك Quartz الفولاذي
        await scheduler.ScheduleJob(job, trigger);
    }
}
