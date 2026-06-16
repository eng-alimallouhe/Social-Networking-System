using Microsoft.EntityFrameworkCore;
using SNS.Application.Identity.ArchiveManagement.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Identity.ArchiveManagement.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;

namespace SNS.Application.Identity.ArchiveManagement.Services;


public sealed class ArchiveCleanupWorker : IArchiveCleanupWorker
{
    private readonly IRepository<UserArchive> _userArchiveRepo;
    private readonly IRepository<PasswordArchive> _passwordArchiveRepo;
    private readonly IRepository<IdentityArchive> _identityArchiveRepo;
    private readonly IRepository<ExportDataRequest> _exportDataRequestRepo;

    public ArchiveCleanupWorker(
        IRepository<UserArchive> userArchiveRepo,
        IRepository<PasswordArchive> passwordArchiveRepo,
        IRepository<IdentityArchive> identityArchiveRepo,
        IRepository<ExportDataRequest> exportDataRequestRepo)
    {
        _userArchiveRepo = userArchiveRepo;
        _passwordArchiveRepo = passwordArchiveRepo;
        _identityArchiveRepo = identityArchiveRepo;
        _exportDataRequestRepo = exportDataRequestRepo;
    }

    public async Task CleanOldArchivesAsync(CancellationToken cancellationToken)
    {
        var thresholdDate = DateTime.UtcNow.AddDays(-365);
        try
        {
            await _identityArchiveRepo
                .ExecuteDeleteAsync(ia => ia.CreatedAt < thresholdDate, cancellationToken);
            
            // 2️⃣ جرف سجلات تغيير البأسورد القديمة (Password Archives)
            await _passwordArchiveRepo
                .ExecuteDeleteAsync(pa => pa.CreatedAt < thresholdDate, cancellationToken);

            // 3️⃣ جرف سجلات حركات الحساب والعقوبات القديمة (User Archives)
            await _userArchiveRepo
                .ExecuteDeleteAsync(ua => ua.CreatedAt < thresholdDate, cancellationToken);

            // 4️⃣ جرف طلبات الـ GDPR المكتملة أو الفاشلة القديمة حماية للخصوصية والمساحة
            await _exportDataRequestRepo
                .ExecuteDeleteAsync(er => er.CreatedAt < thresholdDate, cancellationToken);
        }
        catch (Exception)
        {
            throw;
        }
    }
}
