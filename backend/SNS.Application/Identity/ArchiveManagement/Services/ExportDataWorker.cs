using Microsoft.EntityFrameworkCore;
using SNS.Application.Identity.ArchiveManagement.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Domain.Identity.ArchiveManagement.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SNS.Application.Identity.ArchiveManagement.Services;


public sealed class ExportDataWorker : IExportDataWorker
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IRepository<ExportDataRequest> _exportDataRequestRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService; // تفعيل الخدمة السحابية ☁️

    public ExportDataWorker(
        IApplicationDbContext dbContext,
        IRepository<ExportDataRequest> exportDataRequestRepo,
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorageService) // 💎 تم إصلاح الحقن بالكونستركتور هنا
    {
        _dbContext = dbContext;
        _exportDataRequestRepo = exportDataRequestRepo;
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }

    public async Task ProcessExportAsync(Guid requestId, CancellationToken cancellationToken)
    {
        // 1️⃣ جلب طلب التصدير من قاعدة البيانات
        var exportRequest = await _exportDataRequestRepo.GetByIdAsync(requestId, cancellationToken);

        if (exportRequest == null) return;

        // تجهيز مسارات الملفات المؤقتة خارج الـ try لتنظيفها دائماً في الـ finally
        string jsonPath = string.Empty;
        string zipPath = string.Empty;

        try
        {
            // 2️⃣ تحويل حالة الطلب إلى Processing
            exportRequest.MoveToProcessing();
            await _unitOfWork.CompleteAsync(cancellationToken);

            var userId = exportRequest.UserId;

            // 3️⃣ حصد وحزم كافة بيانات المستخدم من سياق الـ Identity
            var userData = await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => new
                {
                    AccountInformation = new
                    {
                        u.Id,
                        u.UserProfile.FullName,
                        u.UserName,
                        u.Email,
                        u.CreatedAt
                    },
                    IdentityChangeHistory = _dbContext.IdentityArchives
                        .Where(ia => ia.UserId == userId)
                        .Select(ia => new { ia.OldUserIdentifier, ia.NewUserIdentifier, ia.Type, ia.CreatedAt })
                        .ToList(),
                    PasswordChangeHistory = _dbContext.PasswordArchives
                        .Where(pa => pa.UserId == userId)
                        .Select(pa => new { pa.CreatedAt })
                        .ToList(),
                    AccountStatusHistory = _dbContext.UserArchives
                        .Where(a => a.TargetId == userId)
                        .Select(a => new { a.Type, a.Reason, a.CreatedAt })
                        .ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (userData == null)
            {
                exportRequest.Fail();
                await _unitOfWork.CompleteAsync(cancellationToken);
                return;
            }

            // 4️⃣ تحويل الكائن بالكامل إلى نص JSON منسق مع إجبار الـ Enums لتكون نصوصاً صريحة ⚙️
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            jsonOptions.Converters.Add(new JsonStringEnumConverter()); // السحر هنا لتخرج نصوصاً بدل أرقام 🌟

            string jsonString = JsonSerializer.Serialize(userData, jsonOptions);

            // 5️⃣ صناعة ملف الـ ZIP التكتيكي في المجلد المؤقت للسيرفر
            string tempFolder = Path.Combine(Path.GetTempPath(), "SNS_GDPR_Exports");
            if (!Directory.Exists(tempFolder)) Directory.CreateDirectory(tempFolder);

            jsonPath = Path.Combine(tempFolder, $"data_{userId}.json");
            zipPath = Path.Combine(tempFolder, $"GDPR_Export_{userId}_{DateTime.UtcNow:yyyyMMddHHmmss}.zip");

            // كتابة الـ JSON للملف المؤقت
            await File.WriteAllTextAsync(jsonPath, jsonString, cancellationToken);

            // ضغط ملف الـ JSON داخل ملف ZIP نظيف
            if (File.Exists(zipPath)) File.Delete(zipPath);
            using (var zip = ZipFile.Open(zipPath, ZipArchiveMode.Create))
            {
                zip.CreateEntryFromFile(jsonPath, "my_personal_data.json");
            }

            // 6️⃣ الرفع الفعلي والذكي إلى Cloud Storage عبر الواجهة المحقونة 🚀
            string objectKey = $"gdpr-exports/{userId}/{Path.GetFileName(zipPath)}";
            string finalDownloadUrl = string.Empty;

            using (var fileStream = new FileStream(zipPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                // الرفع السحابي النظيف للـ Stream ☁️
                finalDownloadUrl = await _fileStorageService.UploadFileAsync(
                    fileStream,
                    "application/zip",
                    objectKey,
                    cancellationToken);
            }

            // 7️⃣ إغلاق الملف بالشمع الأحمر: تحويل الحالة إلى Completed وحفظ الرابط النهائي 🏁
            exportRequest.Complete(finalDownloadUrl);
            await _unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (Exception)
        {
            // صمام أمان: لو انهار السيرفر أو فشل الرفع السحابي، يتم وسم الـ Job بالفشل لمنع تعليق العميل
            exportRequest.Fail();
            await _unitOfWork.CompleteAsync(CancellationToken.None);
        }
        finally
        {
            // 🧹 حارس التطهير المحلي: مسح الملفات المؤقتة من هارد السيرفر فوراً لحفظ المساحة والأمان
            if (!string.IsNullOrEmpty(jsonPath) && File.Exists(jsonPath)) File.Delete(jsonPath);
            if (!string.IsNullOrEmpty(zipPath) && File.Exists(zipPath)) File.Delete(zipPath);
        }
    }
}